import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { bank } from '../interfaces/bank/bank.interface';

@Injectable({
  providedIn: 'root'
})
export class BankService {
  http = inject(HttpClient);
  urlTemplate = "https://localhost:7280/api"; 

  getBankInfo(){
    return this.http.get<bank>(`${this.urlTemplate}/Bank`);
  }
}
