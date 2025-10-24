import { Component, EventEmitter, inject, Output, signal } from '@angular/core';
import { SharedService } from '../../../data/services/shared-service';
import { AuthService } from '../../../data/services/auth/auth-service';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ProfileService } from '../../../data/services/auth/profile-service';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { IRecoveryPassword } from '../../../data/interfaces/auth/recovery-password';

@Component({
  selector: 'app-recovery-password-email',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './recovery-password-email.html',
  styleUrl: './recovery-password-email.scss'
})
export class RecoveryPasswordEmail {
  sharedService = inject(SharedService);
  authService = inject(AuthService);
  router = inject(Router);
  displayResetError = signal<boolean>(false);
  resetFormErrorMessage :string[] =[];
  profileService = inject(ProfileService);
  @Output() refreshCodeSended = new EventEmitter<string>();

  constructor(private activatedRoute: ActivatedRoute){

  }

  resetForm = new FormGroup({
    email: new FormControl<string| null>(null, [Validators.required, Validators.email]),
    refreshCode: new FormControl<string| null>(null),
    newPassword: new FormControl<string| null>(null)
  });

  onSubmit(){
    this.resetFormErrorMessage = [];
    if(this.resetForm.valid){
      const formValue = this.resetForm.value as IRecoveryPassword;
      this.authService.sendRefreshCode(formValue).subscribe({
        next:()=>{
          this.refreshCodeSended.emit(formValue.email!);
        },
        error:(err)=>{
          this.displayResetError.set(true);
          this.resetFormErrorMessage.push(this.sharedService.serverResponseErrorToArray(err)[0]);
        }
      })
    }
    else{
      this.displayResetError.set(true);
      this.resetFormErrorMessage.push("Please enter the field in email format");
    }
  }

}
