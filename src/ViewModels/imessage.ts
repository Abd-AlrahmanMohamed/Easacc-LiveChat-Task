export interface IMessage {
  id: string;
  chatId: string;
  senderId: string;
  receivedId: string;
  content: string;
  fileUrl?: string | null;
  type: number;
  sentAt: string;
  seen: boolean;
}

