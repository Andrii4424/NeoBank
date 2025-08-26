import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class BankService {
  http = inject(HttpClient);
  urlTemplate = "https://localhost:7280/api"; 

  getBankInfo(){
    return this.http.get(`${this.urlTemplate}/Bank`);
  }
}
