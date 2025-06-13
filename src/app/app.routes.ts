import { Routes } from '@angular/router';
import { RegisterComponent } from '../Components/Register/Register.component';
import { LoginComponent } from '../Components/Login/Login.component';
import { LiveChatComponent } from '../Components/LiveChat/LiveChat.component';

export const routes: Routes = [
  { path: 'adduser', component: RegisterComponent },
  { path: 'login', component: LoginComponent },
  { path: '', component: LiveChatComponent },
];
