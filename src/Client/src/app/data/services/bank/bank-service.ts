import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { IBank } from '../../interfaces/bank/bank.interface';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class BankService {
  http = inject(HttpClient);
  baseUrl = `${environment.apiUrl}/Bank`;

  getBank(){
    return this.http.get<IBank>(this.baseUrl);
  }

  updateBank(bank: IBank){
    return this.http.put<IBank>(`${this.baseUrl}`, bank);
  }
}
