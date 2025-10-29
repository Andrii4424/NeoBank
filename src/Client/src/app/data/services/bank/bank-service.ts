import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { IBank } from '../../interfaces/bank/bank.interface';

@Injectable({
  providedIn: 'root'
})
export class BankService {
  http = inject(HttpClient);
  baseUrl ="/api/bank/Bank";

  getBank(){
    return this.http.get<IBank>(this.baseUrl);
  }

  updateBank(bank: IBank){
    return this.http.put<IBank>(`${this.baseUrl}`, bank);
  }
}
