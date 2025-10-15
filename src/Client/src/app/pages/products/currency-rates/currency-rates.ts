import { AsyncPipe } from '@angular/common';
import { CurrencyService } from './../../../data/services/bank/bank-products/currency-service';
import { Component, inject, signal } from '@angular/core';
import { Loading } from "../../../common-ui/loading/loading";
import { ExchangeCurrencyWindow } from "../../../common-ui/exchange-currency-window/exchange-currency-window";

@Component({
  selector: 'app-currency-rates',
  imports: [AsyncPipe, Loading, ExchangeCurrencyWindow],
  templateUrl: './currency-rates.html',
  styleUrl: './currency-rates.scss'
})
export class CurrencyRates {
  currencyService = inject(CurrencyService);
  currencies$ = this.currencyService.getCurrencies();
  openExchangeWindow = signal<boolean>(false);

}
