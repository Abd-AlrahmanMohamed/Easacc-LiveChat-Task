import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { GenericApiHandlerService } from '../../Service/generic-apihandler.service';
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { CommonModule } from '@angular/common';
import { environment } from '../../enviroments/environment';
import { IUser } from '../../ViewModels/iuser';

@Component({
  selector: 'app-Register',
  templateUrl: './Register.component.html',
  styleUrls: ['./Register.component.css'],
  imports: [ReactiveFormsModule, FormsModule, CommonModule],
})
export class RegisterComponent implements OnInit {
  constructor(
    private hClient: HttpClient,
    private gService: GenericApiHandlerService
  ) {}

  addUserForm = new FormGroup({
    fullname: new FormControl('', [
      Validators.required,
      Validators.pattern('^[a-zA-Z ]{9,}$'),
    ]),
    username: new FormControl('', [Validators.required, Validators.pattern('[a-zA-Z0-9]{9,}')]),
    email: new FormControl(
      '',[Validators.required,
      Validators.pattern('^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$')]
    ),
    password: new FormControl('',[Validators.required, Validators.pattern('^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d).{9,}$')]

),
    contact: new FormControl('',[Validators.required, Validators.pattern('[01]\\d{9}$')]),
  });

  fullname() {
    return this.addUserForm.get('fullname');
  }
  username() {
    return this.addUserForm.get('username');
  }
  email() {
    return this.addUserForm.get('email');
  }
  password() {
    return this.addUserForm.get('password');
  }
  contact() {
    return this.addUserForm.get('contact');
  }

  // onSubmit() {
  //   // console.log(this.addUserForm);

  //   // if (this.addUserForm.invalid) {
  //   //   return;
  //   // }
  //   this.addUser();
  //   console.log(this.addUserForm);

  // }



  // addUser(): Observable<IUserRegister[]> {
  //   const userda: IUserRegister = this.addUserForm.value as IUserRegister;
  // // localStorage.setItem('userId', userda);
  // //   localStorage.setItem('fullName', userda.fullname);
  //   return this.hClient.post<IUserRegister[]>(
  //     `${environment.apiUrl}User/add-user`,
  //     userda

  //   );

  //   //  return this.gService.createData('User/add-user', userda).subscribe((data) => {
  //   // console.log('User added successfully', data.messages);
  //   //   console.log(userda.id);



  //   // })
  // }
  onSubmit() {
  if (this.addUserForm.valid) {
    const formData = this.addUserForm.value;

    this.hClient.post<IUser>(`${environment.apiUrl}User/add-user`, formData)
      .subscribe({
        next: (response) => {
          console.log('✅ Backend response:', response);
          const v = response;
          localStorage.setItem('name' ,response.name);
          localStorage.setItem('id' ,response.id);

        },
        error: (error) => {
          console.error('❌ Backend error:', error);
          // Handle error here (e.g., show error message to user)
        }
      });
  } else {
    console.warn('⚠️ Form is invalid');
    this.addUserForm.markAllAsTouched(); // Highlight invalid fields
  }
}


  ngOnInit() {}
}
