import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/shared/services/auth.service';
import { NotificationService } from 'src/app/shared/services/notification.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {

  registrationFinished = false;

  constructor( private formBuilder: FormBuilder, private userServicve:AuthService, private notificationService: NotificationService,
    private router: Router) { }

  registerForm = this.formBuilder.group({    
    email:new FormControl("",[Validators.required, Validators.email]),
    password:new FormControl("",[Validators.required, Validators.minLength(4)]),   
  });

  ngOnInit(): void {

  }

  onSubmit(): void {
    // Process checkout data here
    console.log(this.registerForm.value);
    
    this.userServicve.register(this.registerForm.value).subscribe({next: r =>{
      this.registrationFinished = true;
     // this.registerForm.reset();
    }, 
    error: e => {
      this.notificationService.showNotification("Registrierung konnte nicht durchgeführt werden. Möglicherweise ist die E-Mail-Adresse schon vergeben.")
      console.error(e);
    }})
  }

}
