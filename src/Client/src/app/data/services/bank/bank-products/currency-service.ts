import { Currency } from './../../../enums/currency';
import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { ICurrency } from '../../../interfaces/bank/bank-products/currency-interface';
import { environment } from '../../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CurrencyService {
  http = inject(HttpClient);
  baseUrl =`${environment.apiUrl}/Currency/`;
  
  getCurrencies(){
    return this.http.get<ICurrency[]>(`${this.baseUrl}GetCurrencyRates`);
  }
  
}
