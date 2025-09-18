import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { IBank } from '../../interfaces/bank/bank.interface';

@Injectable({
  providedIn: 'root'
})
export class BankService {
  http = inject(HttpClient);
  baseUrl ="https://localhost:7280/api/Bank";

  getBank(){
    return this.http.get<IBank>(this.baseUrl);
  }

  testRequest(){
    return this.http.post('https://localhost:7280/api/Bank', "");
  }
}
