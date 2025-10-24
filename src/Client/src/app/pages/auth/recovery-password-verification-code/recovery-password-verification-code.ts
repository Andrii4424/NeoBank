import { Component, EventEmitter, inject, Input, Output, signal } from '@angular/core';
import { SharedService } from '../../../data/services/shared-service';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../data/services/auth/auth-service';
import { ProfileService } from '../../../data/services/auth/profile-service';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { IRecoveryPassword } from '../../../data/interfaces/auth/recovery-password';

@Component({
  selector: 'app-recovery-password-verification-code',
  imports: [RouterLink, ReactiveFormsModule],
  templateUrl: './recovery-password-verification-code.html',
  styleUrl: './recovery-password-verification-code.scss'
})
export class RecoveryPasswordVerificationCode {
  sharedService = inject(SharedService);
  authService = inject(AuthService);
  router = inject(Router);
  displayResetError = signal<boolean>(false);
  resetFormErrorMessage :string[] =[];
  profileService = inject(ProfileService);
  @Output() codeValidated = new EventEmitter<void>();
  @Output() cancelValidation = new EventEmitter<void>();
  @Input() email: string | null = null;

  resetForm = new FormGroup({
    email: new FormControl<string| null>(null, [Validators.required]),
    refreshCode: new FormControl<string| null>(null, [Validators.required]),
    newPassword: new FormControl<string| null>(null)
  });

  ngOnInit(){
    this.resetForm.patchValue({
      email: this.email,
      refreshCode: null,
      newPassword: null
    })
  }

  constructor(private activatedRoute: ActivatedRoute){

  }

  onSubmit(){
    this.resetFormErrorMessage = [];
    if(this.resetForm.valid){
      const formValue = this.resetForm.value as IRecoveryPassword;
      this.authService.validateRefreshCode(formValue).subscribe({
        next:()=>{
          this.codeValidated.emit();
        },
        error:(err)=>{
          this.displayResetError.set(true);
          this.resetFormErrorMessage.push(this.sharedService.serverResponseErrorToArray(err)[0]);
        }
      })
    }
    else{
      this.displayResetError.set(true);
      this.resetFormErrorMessage.push("Code field has to be provided");
    }
  }
}
