import { Component, ElementRef, Host, inject, ViewChild } from '@angular/core';
import { SharedService } from '../../data/services/shared-service';

@Component({
  selector: 'app-footer',
  imports: [],
  templateUrl: './footer.html',
  styleUrl: './footer.scss'
})
export class Footer {
  private sharedService = inject(SharedService);

  copyToClipboard(text:string) {
    this.sharedService.copyText(text).then(() =>{
      alert("Copied to clipboard: " + text);
    }).catch(() =>{
      alert("Failed to copy to clipboard");
    });
  }
}