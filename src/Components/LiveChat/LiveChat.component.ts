import { Component, OnInit } from '@angular/core';
import { environment } from '../../enviroments/environment';
import * as signalR from '@microsoft/signalr';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { IUserData } from '../../ViewModels/IUserData';
import { IMessage } from '../../ViewModels/imessage';

@Component({
  selector: 'app-LiveChat',
  templateUrl: './LiveChat.component.html',
  styleUrls: ['./LiveChat.component.css'],
  imports: [FormsModule, CommonModule],
})
export class LiveChatComponent implements OnInit {
  // constructor() { }

  // ngOnInit() {
  // }
  private hubConnection!: signalR.HubConnection;
  userId: string = localStorage.getItem('id') || '';
  senderId: string = '';
  receivedId: string = '';
  chatId: string = '';
  content: string = '';
  type: number = 0;
  fileUrl?: File | null = null;
  message: string = '';
  allMessages: IMessage[] = [];
  messages: IMessage[] = [];
  allUsers: IUserData[] = [];
  onlineUsers: string[] = [];
  selectedUserId: string | null = null;
  previewUrl: string | null = null;


  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.loadAllUsers();
    this.startConnection();
  }

  ngOnDestroy(): void {
    this.hubConnection?.stop();
  }

  loadAllUsers(): void {
    this.http
      .get<IUserData[]>('https://localhost:7067/Api/User/get-all-users')
      .subscribe({
        next: (users) => {
          this.allUsers = users.filter((user) => user.id !== this.userId);
          console.log('Users:', users);
          console.log('Online Users:', this.onlineUsers);
        },
        error: (err) => console.error('Failed to load users:', err),
      });
  }
  getAllMessageInChat(): void {
    // if (!this.userId || !this.receivedId) {
    //   console.warn('Sender or receiver ID missing');
    //   return;
    // }

    // const url = `https://localhost:7067/api/Message/get-all?SenderId=${this.userId}&ReceiverId=${this.receivedId}`;

    this.http
      .get<IMessage[]>(
        `https://localhost:7067/api/Message/get-all?SenderId=${this.userId}&ReceiverId=${this.selectedUserId}`
      )
      .subscribe({
        next: (messagesChat) => {
          this.allMessages = messagesChat;
          console.log('Messages:', this.allMessages); // Check here if messages loaded
        },
        error: (err) => {
          console.error('Failed to load messages:', err);
        },
      });
  }

  getUserName(userId: string): string {
    const user = this.allUsers.find((u) => u.id === userId);
    return user ? user.fullName : userId; // fallback to userId if not found
  }

  // https://localhost:7067/api/Message/get-all?SenderId=04a1a45b-6843-455c-a945-5fcc20391ff9&ReceiverId=8d9fa8a9-fffc-4c59-a617-494866811c28

  startConnection(): void {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:7067/api/livechatHub')
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('SignalR connected'))
      .catch((err) => console.error('SignalR error:', err));

    this.hubConnection.on('ReceiveMessage', (msg: any) => {
      if (msg.chatId === this.chatId) {
        this.messages.push(msg);
      }
    });

    this.hubConnection.on('ReceiveMessagesHistory', (msgs: IMessage[]) => {
      this.messages = msgs;
    });

    this.hubConnection.on('UserOnline', (userId: string) => {
      if (!this.onlineUsers.includes(userId) && userId !== this.userId) {
        this.onlineUsers.push(userId);
      }
    });

    this.hubConnection.on('UserOffline', (userId: string) => {
      this.onlineUsers = this.onlineUsers.filter((id) => id !== userId);
    });
  }

  selectUser(id: string): void {
    this.selectedUserId = id;
    this.chatId = this.generateChatId(this.userId, id);
    this.messages = [];
    this.getAllMessageInChat();

    this.hubConnection
      .invoke('GetMessages', this.userId, this.selectedUserId)
      .catch((err) => console.error('Failed to fetch messages:', err));
    console.log(this.selectedUserId);
  }

  generateChatId(user1: string, user2: string): string {
    return [user1, user2].sort().join('-');
  }

 sendMessage(): void {
  if ((!this.message && !this.fileUrl) || !this.selectedUserId) return;

  const isFile = !!this.fileUrl;
  let type = 0;

  if (isFile) {
    const fileType = this.fileUrl!.type;
    if (fileType.startsWith('image/')) type = 1;
    else if (fileType === 'application/pdf' || fileType.includes('document')) type = 2;
    else if (fileType.startsWith('audio/')) type = 3;
  }

  const sendPayload = (base64File: string | null = null) => {
    const payload = {
      SenderId: this.userId,
      ReceiverId: this.selectedUserId,
      ChatId: this.chatId,
      Content: this.message,
      Type: type,
      FileUrl: this.fileUrl?.name,
    };
    console.log(payload)
    console.log('sendPayload', this.fileUrl);
    this.message = '';
    this.fileUrl = null;
    this.previewUrl = null;

    this.hubConnection
      .invoke('SendMessage', payload)
      .then(() => this.getAllMessageInChat())
      .catch((err) => console.error('SendMessage error:', err));

    // this.message = '';
    // this.fileUrl = null;
    // this.previewUrl = null;

    this.hubConnection
      .invoke('GetMessages', this.userId, this.selectedUserId)
      .catch((err) => console.error('Failed to fetch messages:', err));
  };

  if (isFile) {
    const reader = new FileReader();
    reader.onload = () => sendPayload(reader.result as string);
    reader.readAsDataURL(this.fileUrl!);
  } else {
    sendPayload(null);
  }
}


onFileSelected(event: Event): void {
  const target = event.target as HTMLInputElement;
  if (target.files && target.files.length > 0) {
    this.fileUrl = target.files[0];

    // Generate image preview if it's an image
    if (this.fileUrl.type.startsWith('image/')) {
      const reader = new FileReader();
      reader.onload = () => {
        this.previewUrl = reader.result as string;
      };
      reader.readAsDataURL(this.fileUrl);
    } else {
      this.previewUrl = null;
    }
  }
}

  hideChat(): void {
    this.selectedUserId = null;
    this.chatId = '';
    this.messages = [];
  }

  isUserOnline(userId: string): boolean {
    return this.onlineUsers.includes(userId);
  }
   signOut() {
  localStorage.removeItem('id');
  localStorage.removeItem('name');
}
}
