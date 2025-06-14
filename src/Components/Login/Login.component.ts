import { Component, OnInit } from '@angular/core';
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { environment } from '../../enviroments/environment';
import { IUser } from '../../ViewModels/iuser';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-Login',
  templateUrl: './Login.component.html',
  styleUrls: ['./Login.component.css'],
  imports: [ReactiveFormsModule, FormsModule, CommonModule],
})
export class LoginComponent implements OnInit {
  constructor(private hClient: HttpClient, private router: Router) {}

  loginUserForm = new FormGroup({
    email: new FormControl('', Validators.required),
    password: new FormControl('', Validators.required),
  });

  email() {
    return this.loginUserForm.get('email');
  }
  password() {
    return this.loginUserForm.get('password');
  }

  onSubmit() {
    if (this.loginUserForm.valid) {
      const formData = this.loginUserForm.value;

      this.hClient
        .post<IUser>(`${environment.apiUrl}User/login`, formData)
        .subscribe({
          next: (response) => {
            console.log('✅ Backend response:', response);
            localStorage.setItem('name', response.name);
            localStorage.setItem('id', response.id);

            setTimeout(() => {
              this.router.navigate(['/']);
            }, 2000);
          },
          error: (error) => {
            console.error(' Backend error:', error);
          },
        });
    } else {
      console.warn(' Form is invalid');
      this.loginUserForm.markAllAsTouched();
    }
  }

  ngOnInit() {}
}
