import { Component, inject, Input, signal } from '@angular/core';
import { AuthService } from '../../../data/services/auth/auth-service';
import { SharedService } from '../../../data/services/shared-service';
import { ProfileService } from '../../../data/services/auth/profile-service';
import { Router } from '@angular/router';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RegisterDto } from '../register/register';
import { IRecoveryPassword } from '../../../data/interfaces/auth/recovery-password';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-recovery-password-form',
  imports: [ReactiveFormsModule, TranslateModule],
  templateUrl: './recovery-password-form.html',
  styleUrl: './recovery-password-form.scss'
})
export class RecoveryPasswordForm {
  authService = inject(AuthService);
  sharedService = inject(SharedService);
  profileService = inject(ProfileService);
  router = inject(Router);
  displayResetError = signal<boolean>(false);
  @Input() email: string | null = null;
  @Input() refreshCode: string | null = null;
  
  resetFormErrorMessage :string[] = [];

  resetForm = new FormGroup({
    email: new FormControl<string| null>(null, [Validators.required, Validators.email]),
    refreshCode: new FormControl<string| null>(null, [Validators.required]),
    newPassword: new FormControl<string| null>(null, [Validators.required]),
    newPasswordConfirm: new FormControl<string| null>(null, [Validators.required])
  })

  ngOnInit(){
    this.resetForm.patchValue({
      email: this.email,
      refreshCode: this.refreshCode
    })
  }

  onSubmit(){
    this.resetFormErrorMessage =[];
    if(this.resetForm.valid){
      if(this.resetForm.get('newPassword')?.value !== this.resetForm.get('newPasswordConfirm')?.value){
        this.displayResetError.set(true);
        this.resetFormErrorMessage.push("Password fields must must match");
      }
      else{
        const changeParams: IRecoveryPassword = { 
          email: this.resetForm.get('email')!.value,
          refreshCode: this.resetForm.get('refreshCode')!.value,
          newPassword: this.resetForm.get('newPassword')!.value
        };
        this.authService.updatePassword(changeParams).subscribe({
          next:()=>{
            this.authService.login({email: changeParams.email!, password: changeParams.newPassword!}).subscribe({
              error:(err)=>{
                this.displayResetError.set(true);
                this.resetFormErrorMessage.push(`Password has been changed, but login is unsuccessfull, reason 
                  ${this.sharedService.serverResponseErrorToArray(err)[0]}. Please try login manually, or contact bank admin`);
              },
              complete:()=>{
                this.router.navigate(['/']);
              }
            })
          },
          error:(err)=>{
            this.displayResetError.set(true);
            this.resetFormErrorMessage.push(this.sharedService.serverResponseErrorToArray(err)[0]);
          }
        })

      }
    }
    else{
        const changeParams: IRecoveryPassword = { 
          email: this.resetForm.get('email')!.value,
          refreshCode: this.resetForm.get('refreshCode')!.value,
          newPassword: this.resetForm.get('newPassword')!.value
        };
        console.log(changeParams);
      this.displayResetError.set(true);
      this.resetFormErrorMessage.push("All login fields must be filled in");
    }
  }
}
