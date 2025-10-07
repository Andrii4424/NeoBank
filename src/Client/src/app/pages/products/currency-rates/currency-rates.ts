import { AsyncPipe } from '@angular/common';
import { CurrencyService } from './../../../data/services/bank/bank-products/currency-service';
import { Component, inject } from '@angular/core';

@Component({
  selector: 'app-currency-rates',
  imports: [AsyncPipe],
  templateUrl: './currency-rates.html',
  styleUrl: './currency-rates.scss'
})
export class CurrencyRates {
  currencyService = inject(CurrencyService);
  currencies$ = this.currencyService.getCurrencies();

}
