import { ChangeDetectorRef, Component, EventEmitter, inject, Input, Output, signal } from '@angular/core';
import { SharedService } from '../../../data/services/shared-service';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../data/services/auth/auth-service';
import { ProfileService } from '../../../data/services/auth/profile-service';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { IRecoveryPassword } from '../../../data/interfaces/auth/recovery-password';
import { interval, take, tap } from 'rxjs';
import { SuccessMessage } from "../../../common-ui/success-message/success-message";

@Component({
  selector: 'app-recovery-password-verification-code',
  imports: [RouterLink, ReactiveFormsModule, SuccessMessage],
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
  @Output() codeValidated = new EventEmitter<string>();
  @Output() cancelValidation = new EventEmitter<void>();
  @Input() email: string | null = null;
  readyToResendCode = signal<boolean>(false);
  showSuccessMessage = signal<boolean>(false);
  countdown?: number;

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
    this.startCooldownSimple();
  }

  constructor(private activatedRoute: ActivatedRoute, private cdr: ChangeDetectorRef){

  }

  onSubmit(){
    this.resetFormErrorMessage = [];
    if(this.resetForm.valid){
      const formValue = this.resetForm.value as IRecoveryPassword;
      this.authService.validateRefreshCode(formValue).subscribe({
        next:()=>{
          this.codeValidated.emit(formValue.refreshCode!);
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

  resendCode(){
    const formValue = this.resetForm.value as IRecoveryPassword;

    this.authService.sendRefreshCode(formValue).subscribe({
      next:()=>{
        this.showSuccessMessage.set(true)
        setTimeout(()=> this.showSuccessMessage.set(false),3000);
        this.startCooldownSimple();
      },
      error:(err)=>{
        this.displayResetError.set(true);
        this.resetFormErrorMessage.push(this.sharedService.serverResponseErrorToArray(err)[0]);
      }
    });
  }


  startCooldownSimple() {
    this.readyToResendCode.set(false);
    this.countdown = 60;

    interval(1000)
      .pipe(
        take(this.countdown),
        tap(() => {
          this.countdown!--;
          this.cdr.detectChanges();
        })
      )
      .subscribe({
        complete: () => {
          this.readyToResendCode.set(true);
        }
      });
  }   

}
