import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class SharedService {
  copyText(text:string){
    return navigator.clipboard.writeText(text);
  }
}
