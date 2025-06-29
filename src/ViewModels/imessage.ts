export interface IMessage {
  id: string;
  senderId: string;
  sender: string;
  receivedId: string;
  receiver:string;
  chatId: string;
  chat: string;
  content: string;
  fileUrl?: string | null;
  type: number;
  sentAt: string;
  seen: boolean;
}
