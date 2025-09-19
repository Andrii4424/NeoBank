import { SharedService } from './../../../data/services/shared-service';
import { Component, ContentChild, ElementRef, inject, signal, ViewChild } from '@angular/core';
import { Footer } from "../../../common-ui/footer/footer";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../../data/services/auth/auth-service';
import { RouterLink } from "@angular/router";
import { ProfileService } from '../../../data/services/auth/profile-service';

@Component({
  selector: 'app-login',
  imports: [Footer, ReactiveFormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class Login {
  sharedService = inject(SharedService);
  authService = inject(AuthService);
  router = inject(Router);
  displaySessionError = signal<boolean>(false);
  displayLoginError = signal<boolean>(false);
  loginFormErrorMessage :string[] =[];
  profileService = inject(ProfileService);

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
        next:()=>{
          this.router.navigate([''])          
        }, 
        error: (err)=>{
          this.displayLoginError.set(true);
          this.loginFormErrorMessage=this.sharedService.serverResponseErrorToArray(err);
        },
        complete: ()=>{
          this.profileService.updateRole();
        }
      });
    }
    else{
      this.displayLoginError.set(true);
      this.loginFormErrorMessage.push("All login fields must be filled in");
    }
  }
}
