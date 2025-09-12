import { Component, inject, signal } from '@angular/core';
import { Footer } from "../../../common-ui/footer/footer";
import { Router, RouterLink } from "@angular/router";
import { FormControl, FormGroup, ReactiveFormsModule, Validators, ɵInternalFormsSharedModule } from '@angular/forms';
import { AuthService } from '../../../data/services/auth/auth-service';
import { SharedService } from '../../../data/services/shared-service';

export type RegisterDto = {
  email: string |null;
  password: string |null;
  confirmPassword: string |null;
};

@Component({
  selector: 'app-register',
  imports: [Footer, RouterLink, ɵInternalFormsSharedModule, ReactiveFormsModule],
  templateUrl: './register.html',
  styleUrl: './register.scss'
})

export class Register {
  authService = inject(AuthService);
  sharedService = inject(SharedService)
  router = inject(Router);
  displayRegisterError = signal<boolean>(false);
  registerErrorMessage :string[] = [];

  registerForm = new FormGroup({
    email: new FormControl(null, Validators.required),
    password: new FormControl(null, Validators.required),
    confirmPassword: new FormControl(null, Validators.required)
  })

  onSubmit(){
    this.registerErrorMessage =[];
    if(this.registerForm.valid){
      const payload: RegisterDto  = this.registerForm.getRawValue();
      this.authService.register(payload).subscribe({
        next:()=>{
          this.router.navigate(['']);
        },
        error:(err)=>{
          this.displayRegisterError.set(true);
          this.registerErrorMessage=this.sharedService.serverResponseErrorToArray(err);
        }
      })
    }
    else{
      this.displayRegisterError.set(true);
      this.registerErrorMessage.push("All login fields must be filled in");
    }
  }
}
