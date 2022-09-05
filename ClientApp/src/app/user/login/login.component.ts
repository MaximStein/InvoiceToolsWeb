import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'src/app/shared/services/auth.service';
import { NotificationService } from 'src/app/shared/services/notification.service';

import { AuthResponseDto } from 'src/types/response/authResponseDto';
import { UserLoginDto } from 'src/types/user/userLoginDto';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  
  private returnUrl: string;
  errorMessage: string = '';
  showError: boolean;
  
  loginForm:FormGroup = new FormGroup({
    email: new FormControl("", [Validators.required]),
    password: new FormControl("", [Validators.required])
  });
  
  
  constructor(private _notificationService: NotificationService, private formBuilder: FormBuilder, private authService:AuthService, private router: Router, private route: ActivatedRoute) { }


  ngOnInit(): void {
   
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/profile';
  }

  validateControl = (controlName: string) => {
    return this.loginForm.get(controlName)?.invalid && this.loginForm.get(controlName)?.touched
  }

  hasError = (controlName: string, errorName: string) => {
    return this.loginForm.get(controlName)?.hasError(errorName)
  }

  loginUser = () => {
    this.showError = false;
    const login = {... this.loginForm.value };
    const userForAuth: UserLoginDto = {
      email: login.email,
      password: login.password
    }
    this.authService.login(userForAuth)
    .subscribe({
      next: (res:AuthResponseDto) => {
       localStorage.setItem("token", res.token);
       //this.authService.sendAuthStateChangeNotification(res.isAuthSuccessful);
       this.router.navigate([this.returnUrl]);
    },
    error: (err: HttpErrorResponse) => {
      this.errorMessage = err.message;
      this.showError = true;
      console.log(err);
      this._notificationService.showNotification(err.message ? err.message : "Login konnte nicht durchgeführt werden");
    }})
  }

}
