export interface IUser {
   id: string;
  name: string;
  email: string;
  token: string;
  userName: string;
  isAuthenticated: boolean;
  roles: string[];
  expiresOn: string;
  message: string;

}
