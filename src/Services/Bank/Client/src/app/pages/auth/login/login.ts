import { Component, inject } from '@angular/core';
import { Footer } from "../../../common-ui/footer/footer";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../../services/auth/auth-service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  imports: [Footer, ReactiveFormsModule],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class Login {
  authService = inject(AuthService);
  router = inject(Router);

  loginForm = new FormGroup({
    email: new FormControl(null, Validators.required),
    password: new FormControl(null, Validators.required)
  });

  onSubmit(){
    if(this.loginForm.valid){
      //@ts-ignore
      this.authService.login(this.loginForm.value).subscribe({
        next:(res)=>{
          console.log("Success!", res)
          this.router.navigate([''])          
        }, 
        error: (err)=>{
          console.error('Login failed', err);
        }

      })
    }
    else{
      console.log("Not valid form")
    }
  }

}
