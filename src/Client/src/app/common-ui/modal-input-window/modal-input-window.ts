import { Currency } from './../../data/enums/currency';
import { UserCardsService } from './../../data/services/bank/bank-products/user-cards-service';
import { CurrencyService } from './../../data/services/bank/bank-products/currency-service';
import { ChangeDetectorRef, Component, ElementRef, EventEmitter, inject, Input, Output, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { IExchangeCurrency } from '../../data/interfaces/bank/bank-products/cards/exchange-currency-interface';
import { tap } from 'rxjs';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-modal-input-window',
  imports: [FormsModule, TranslateModule],
  templateUrl: './modal-input-window.html',
  styleUrl: './modal-input-window.scss'
})
export class ModalInputWindow {
  userCardsService = inject(UserCardsService);
  @Input() headerMessage: string ="";
  @Input() subTittleMessage: string ="";
  @Input() isAddFunds: boolean = false;
  @Input() isChangePin: boolean = false;
  @Input() isChangeCreditLimit: boolean = false;
  @Input() creditLimitParams?: IExchangeCurrency;
  @Input() startCreditLimitValue?: number;

  @Output() submitValue = new EventEmitter<number>();
  @Output() submitPin = new EventEmitter<string>();
  @Output() submitCreditLimit = new EventEmitter<number>();
  @Output() closeWindow = new EventEmitter<void>();

  currencyName?: string;
  maxCreditLimitAmount? : number;
  amount : number = 0;
  pin: string ="";

  constructor(private cdr: ChangeDetectorRef){}

  ngOnInit(){
    switch(this.creditLimitParams?.to){
      case (Currency.UAH):{
        this.currencyName="₴";
        break;
      }
      case (Currency.USD):{
        this.currencyName="$";
        break;
      }
      case (Currency.EUR):{
        this.currencyName="€";
        break;
      }
      default:{
        this.currencyName="";
        break;
      }
    }
    
    if(this.startCreditLimitValue) this.amount=this.startCreditLimitValue;
    
    this.checkAndExchangeCreditLimitCurrency();
    this.cdr.detectChanges();
  }

  checkAndExchangeCreditLimitCurrency(){
    if(this.isChangeCreditLimit){
      if(this.creditLimitParams?.to===Currency.UAH){
        this.maxCreditLimitAmount=this.creditLimitParams.amount;
      }
      else{
        this.userCardsService.exchangeCurrency(this.creditLimitParams!).subscribe({
          next: (val)=> {
            this.maxCreditLimitAmount = val;
            this.cdr.detectChanges();
          }
        });
      }
    }
  }

  onModalInputChange(value: number){
    if (value < 0) {
      this.amount = 0;
    } 
    else if(value > 200000000){
      setTimeout(() => this.amount = 200000000);
    }
    else {
      this.amount = value;
    }
  }

  closeModalWindow(){
    this.closeWindow.emit();
  }

  onSubmit(){
    this.submitValue.emit(this.amount);
  }

  onPinSubmit(){
    this.submitPin.emit(this.pin);
  }

  onCreditLimitSubmit(){
    this.submitCreditLimit.emit(this.amount);
  }

  onPinInput(event: Event) {
    const input = (event.target as HTMLInputElement).value;
    let pinInput ='';
    pinInput = input.replace(/\D/g, '').slice(0, 4);
    this.pin= pinInput;
  }
}
