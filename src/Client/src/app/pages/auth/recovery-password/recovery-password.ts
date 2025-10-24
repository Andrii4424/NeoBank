import { Component, signal } from '@angular/core';
import { RecoveryPasswordEmail } from "../recovery-password-email/recovery-password-email";
import { Footer } from "../../../common-ui/footer/footer";
import { RecoveryPasswordVerificationCode } from "../recovery-password-verification-code/recovery-password-verification-code";
import { RecoveryPasswordForm } from "../recovery-password-form/recovery-password-form";

@Component({
  selector: 'app-recovery-password',
  imports: [RecoveryPasswordEmail, Footer, RecoveryPasswordVerificationCode, RecoveryPasswordForm],
  templateUrl: './recovery-password.html',
  styleUrl: './recovery-password.scss'
})
export class RecoveryPassword {
  refreshCodeSended = signal<boolean>(false);
  refreshCodeVerified = signal<boolean>(false);
  refreshEmail: string | null = null;

  onRefreshCodeSended(email: string){
    this.refreshCodeSended.set(true);
    this.refreshEmail = email;
  }

  onCancelValication(){
    this.refreshCodeSended.set(false);
    this.refreshEmail = null;
  }
}
