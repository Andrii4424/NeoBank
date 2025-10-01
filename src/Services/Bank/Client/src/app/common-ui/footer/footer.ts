import { Component, ElementRef, Host, inject, signal, ViewChild } from '@angular/core';
import { SharedService } from '../../data/services/shared-service';
import { ErrorMessage } from "../error-message/error-message";
import { SuccessMessage } from "../success-message/success-message";

@Component({
  selector: 'app-footer',
  imports: [ErrorMessage, SuccessMessage],
  templateUrl: './footer.html',
  styleUrl: './footer.scss'
})
export class Footer {
  private sharedService = inject(SharedService);
  showErrorMessage = signal<boolean>(false);
  showSuccessMessage = signal<boolean>(false);


  copyToClipboard(text:string) {
    this.showSuccessMessage.set(false);
    this.sharedService.copyText(text).then(() =>{
      this.showSuccessMessage.set(true);
      setTimeout(() => this.showSuccessMessage.set(false), 3000); 
    }).catch(() =>{
      this.showErrorMessage.set(true);
      setTimeout(() => this.showErrorMessage.set(false), 3000); 
    });
  }
}