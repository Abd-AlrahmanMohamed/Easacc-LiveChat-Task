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
  private hubConnection!: signalR.HubConnection;

  userId: string = localStorage.getItem('id') || '';
  senderId: string = '';
  receivedId: string = '';
  chatId: string = '';
  content: string = '';
  type: number = 0;

  message: string = '';
  previewUrl: string | null = null;
  selectedFile: File | null = null;

  messages: IMessage[] = [];
  allMessages: IMessage[] = [];
  allUsers: IUserData[] = [];
  onlineUsers: string[] = [];
  selectedUserId: string | null = null;

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
        },
        error: (err) => {
          console.error('Failed to load messages:', err);
        },
      });
  }

  getUserName(userId: string): string {
    const user = this.allUsers.find((u) => u.id === userId);
    return user ? user.fullName : userId;
  }

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
  }

  generateChatId(user1: string, user2: string): string {
    return [user1, user2].sort().join('-');
  }

  sendMessage(): void {
    if ((!this.message?.trim() && !this.selectedFile) || !this.selectedUserId)
      return;

    let type = 0;
    if (this.selectedFile) {
      const fileType = this.selectedFile.type;

      if (fileType.startsWith('image/')) {
        type = 1;
      } else if (
        fileType === 'application/pdf' ||
        fileType === 'application/msword' ||
        fileType.includes('document')
      ) {
        type = 2;
      } else if (fileType.startsWith('audio/')) {
        type = 3;
      }
    }

    const formData = new FormData();
    formData.append('SenderId', this.userId);
    formData.append('ReceiverId', this.selectedUserId);
    formData.append('Content', this.message || '');
    formData.append('Type', type.toString());

    if (this.selectedFile) {
      formData.append('FileUrl', this.selectedFile, this.selectedFile.name);
    }

    this.http
      .post<IMessage>('https://localhost:7067/api/Message/send', formData)
      .subscribe({
        next: (sentMessage) => {
          this.messages.push(sentMessage);
          this.getAllMessageInChat();

          this.hubConnection
            .invoke('SendMessage', sentMessage)
            .catch((err) => console.error('SendMessage error:', err));

          this.message = '';
          this.previewUrl = null;
          this.selectedFile = null;
          console.log('Message sent successfully', sentMessage);
        },
        error: (err) => {
          console.error('HTTP message send failed:', err);
        },
      });
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];

      const reader = new FileReader();
      reader.onload = () => {
        this.previewUrl = reader.result as string;
      };
      reader.readAsDataURL(this.selectedFile);
    } else {
      this.selectedFile = null;
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

  signOut(): void {
    localStorage.removeItem('id');
    localStorage.removeItem('name');
  }

  getFullFileUrl(file: string): string {
    return `https://localhost:7067${file}`;
  }
}
