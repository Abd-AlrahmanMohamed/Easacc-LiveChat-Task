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
  fileUrl?: string | null = null;
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

  // sendNewMessage(): void {

  //   this.http
  //     .get<IMessage[]>(
  //       `https://localhost:7067/api/Message/send, `)
  //     .subscribe({
  //       next: (messagesChat) => {
  //         this.allMessages = messagesChat;
  //         console.log('Messages:', this.allMessages); // Check here if messages loaded
  //       },
  //       error: (err) => {
  //         console.error('Failed to load messages:', err);
  //       },
  //     });
  // }

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
    if ((!this.message?.trim() && !this.fileUrl) || !this.selectedUserId)
      return;

   let type = 0;
if (this.fileUrl) {
  const fileType = this.fileUrl;

  if (fileType.startsWith('data:image/')) {
    type = 1; 
  } else if (
    fileType.startsWith('data:application/pdf') ||
    fileType.startsWith('data:application/msword') ||
    fileType.includes('document')
  ) {
    type = 2;
  } else if (fileType.startsWith('data:audio/')) {
    type = 3;
  }
}


    const formData = new FormData();
    formData.append('SenderId', this.userId);
    formData.append('ReceiverId', this.selectedUserId);
    formData.append('Content', this.message);
    formData.append('Type', type.toString());

    if (this.fileUrl) {
      formData.append('FileUrl', this.fileUrl);
    }

    // Send message via HTTP
    const res = this.http
      .post<IMessage>('https://localhost:7067/api/Message/send', formData)
      .subscribe({
        next: (sentMessage) => {
          console.log('Message sent:', sentMessage);
          this.messages.push(sentMessage); // Optional: append immediately
         this.getAllMessageInChat()
          // Real-time notification via SignalR
          this.hubConnection
            .invoke(
              'SendMessage',
              (sentMessage)
            )
            // .then(() => this.getAllMessageInChat())
            .catch((err) => console.error('SendMessage error:', err));
          console.log(res);
          console.log('Message sent:', sentMessage);

          // (Optional) refresh messages if needed
          this.hubConnection
            .invoke('GetMessages', this.userId, this.selectedUserId)
            .catch((err) => console.error('Failed to fetch messages:', err));
        },
        error: (err) => {
          console.error('HTTP message send failed:', err);
        },
      });
  }

 onFileSelected(event: Event): void {
  const input = event.target as HTMLInputElement;
  if (input.files && input.files.length > 0) {
    const file = input.files[0];

    const reader = new FileReader();
    reader.onload = () => {
      this.previewUrl = reader.result as string;
      this.fileUrl = this.previewUrl;
    };
    reader.readAsDataURL(file);
  } else {
    this.fileUrl = null;
    this.previewUrl = null;
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
