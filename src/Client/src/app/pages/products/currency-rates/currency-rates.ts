import { AsyncPipe } from '@angular/common';
import { CurrencyService } from './../../../data/services/bank/bank-products/currency-service';
import { Component, inject, signal } from '@angular/core';
import { Loading } from "../../../common-ui/loading/loading";
import { ExchangeCurrencyWindow } from "../../../common-ui/exchange-currency-window/exchange-currency-window";
import { SuccessMessage } from "../../../common-ui/success-message/success-message";
import { ErrorMessage } from "../../../common-ui/error-message/error-message";

@Component({
  selector: 'app-currency-rates',
  imports: [AsyncPipe, Loading, ExchangeCurrencyWindow, SuccessMessage, ErrorMessage],
  templateUrl: './currency-rates.html',
  styleUrl: './currency-rates.scss'
})
export class CurrencyRates {
  currencyService = inject(CurrencyService);
  currencies$ = this.currencyService.getCurrencies();
  openExchangeWindow = signal<boolean>(false);
  errorMessage: string | null = null;
  showSuccessWindow = signal<boolean>(false);
  showErrorWindow = signal<boolean>(false);

  successExchange(){
    this.openExchangeWindow.set(false);
    this.showSuccessWindow.set(true);
    setTimeout(()=> this.showSuccessWindow.set(false), 3000);
  }
  
  errorExchange(errorMsg: string){
    this.errorMessage =errorMsg;
    this.openExchangeWindow.set(false);
    this.showErrorWindow.set(true);
    setTimeout(()=> this.showErrorWindow.set(false), 3000);

  }
}
