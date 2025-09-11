import { Component, inject, signal } from '@angular/core';
import { Footer } from "../../../common-ui/footer/footer";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../../data/services/auth/auth-service';

@Component({
  selector: 'app-login',
  imports: [Footer, ReactiveFormsModule],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class Login {
  authService = inject(AuthService);
  router = inject(Router);
  displaySessionError = signal<boolean>(false)
  constructor(private activatedRoute: ActivatedRoute){
    
  }

  ngOnInit(){
    this.activatedRoute.queryParams.subscribe(params =>{
      if(params["error"]==="sessionExpired"){
        this.displaySessionError.set(true);
      }
    });
  }


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
