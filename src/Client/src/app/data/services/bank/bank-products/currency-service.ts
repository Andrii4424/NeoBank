import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { ICurrency } from '../../../interfaces/bank/bank-products/currency-interface';

@Injectable({
  providedIn: 'root'
})
export class CurrencyService {
  http = inject(HttpClient);
  baseUrl : string = 'https://localhost:7280/api/Currency/';

  getCurrencies(){
    return this.http.get<ICurrency[]>(`${this.baseUrl}GetCurrencyRates`);
  }
}
