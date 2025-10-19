import { SharedService } from './../../data/services/shared-service';
import { CurrencyService } from './../../data/services/bank/bank-products/currency-service';
import { Currency } from './../../data/enums/currency';
import { UserCardsService } from './../../data/services/bank/bank-products/user-cards-service';
import { ChangeDetectorRef, Component, EventEmitter, inject, Input, Output, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { IUserCards } from '../../data/interfaces/bank/bank-products/cards/user-cards-interface';
import { TransactionService } from '../../data/services/bank/bank-products/transaction-service';
import { from, Subscription } from 'rxjs';
import { ITransaction } from '../../data/interfaces/bank/bank-products/cards/transaction-interface';
import { ICurrency } from '../../data/interfaces/bank/bank-products/currency-interface';
import { DecimalPipe } from '@angular/common';
import { CardStatus } from '../../data/enums/card-status';
import { TransactionType } from '../../data/enums/transaction-type';

@Component({
  selector: 'app-exchange-currency-window',
  imports: [ReactiveFormsModule, DecimalPipe],
  templateUrl: './exchange-currency-window.html',
  styleUrl: './exchange-currency-window.scss'
})
export class ExchangeCurrencyWindow {
  transactionService = inject(TransactionService);
  userCardsService = inject(UserCardsService);
  currencyService = inject(CurrencyService);
  sharedService = inject(SharedService);

  cards?: IUserCards[];
  @Output() closeWindow = new EventEmitter<void>();
  chosenCardRate: number | null = null;
  chosenCurrencySymbol: string | null= null;
  currencyRates?: ICurrency[];
  showGetAmount= signal<boolean>(false);
  showCommission= signal<boolean>(false);
  getAmount: number | null= null;
  commission: number | null= null;
  getterCurrency: string | null = null;
  senderCurrency: string | null = null;
  showValidationError = signal<boolean>(false);
  errorText: string | null = null;
  private senderCardIdSub!:Subscription;
  private getterCardIdSub!:Subscription;
  private amountSub!:Subscription;
  @Output() success = new EventEmitter<void>;
  @Output() error = new EventEmitter<string>;

  constructor(private cdr: ChangeDetectorRef) {
    
  }
  ngOnInit(){
    this.userCardsService.getMyCards({}).subscribe({
      next:(val)=>{
        this.cards = val.items;
      },
      error:()=>{

      }
    });
    this.currencyService.getCurrencies().subscribe({
      next:(val)=>{
        this.currencyRates = val;
      }
    });

    this.senderCardIdSub=this.transactionForm.get('senderCardId')!.valueChanges.subscribe(selectedCardId=>{
      const chosenCard = this.cards?.find(c=> c.id===selectedCardId);
      if(chosenCard !=null){
        this.updateGetAmount();
      }
    });
    this.getterCardIdSub=this.transactionForm.get('getterCardId')!.valueChanges.subscribe(selectedCardId=>{
      const chosenCard = this.cards?.find(c=> c.id===selectedCardId);
      if(chosenCard !=null){
        this.updateGetAmount();
      }
    });
    this.amountSub=this.transactionForm.get('amount')!.valueChanges.subscribe(amount=>{
      if(amount !== null && amount < 0){
        this.transactionForm.patchValue({
          amount: null,
        });
      }
      else{
        this.updateGetAmount();
      }
    });
  }

  ngOnDestroy(){
    if(this.senderCardIdSub){
      this.senderCardIdSub.unsubscribe();
    }
    if(this.getterCardIdSub){
      this.getterCardIdSub.unsubscribe();
    }
    if(this.amountSub){
      this.amountSub.unsubscribe();
    }
  }

  transactionForm = new FormGroup({
    senderCardId: new FormControl<string>("", [Validators.required]),
    getterCardId: new FormControl<string>("", [Validators.required]),
    amount: new FormControl<number | null>(null, [Validators.required]),
  })

  switchCards(){
    const fromValue = this.transactionForm.get('senderCardId')?.value;
    const toValue = this.transactionForm.get('getterCardId')?.value;

    this.transactionForm.patchValue({
      senderCardId: toValue,
      getterCardId: fromValue
    })
  }

  onSubmit(){
    if(this.transactionForm.valid){
      if(this.transactionForm.get('senderCardId')?.value === ""){
        this.showError("The card to be withdrawn must be selected");
        return;
      }
      if(this.transactionForm.get('senderCardId')?.value === ""){
        this.showError("The card for enrollment must be selected");
        return;
      }
      const chosenCard = this.cards?.find(c=>c.id == this.transactionForm.get('senderCardId')?.value);
      const availableBalance = chosenCard!.balance + chosenCard!.creditLimit;
      if(availableBalance < this.transactionForm.get('amount')!.value!){
        this.showError("The balance is insufficient to complete the exchange, please top up the card");
        return ;
      }
      if(chosenCard?.status !== CardStatus.Active){
        this.showError("You can only send money from an active card. Please reissue or unblock the card to send.");
        return ;
      }
      if(chosenCard.cardNumber === this.transactionForm.get('getterCardId')?.value!){
        this.showError("The sender's card and the recipient's card must not match.");
        return ;
      }
      this.transactionService.makeTransaction(
        {senderCardId: this.transactionForm.get('senderCardId')!.value!, 
          getterCardId: this.transactionForm.get('getterCardId')!.value!, amount: this.transactionForm.get('amount')!.value!,
          type: TransactionType.CurrencyExchange}
      ).subscribe({
        next:()=>{
          this.success.emit()
        },
        error:(err)=>{
          this.error.emit(this.sharedService.serverResponseErrorToArray(err)[0])
        }
      });
    }
    else{
      this.showError("All fields has to be provided");
    }
  }

  showError(errorText : string){
    this.errorText = errorText;
    this.showValidationError.set(true);
    this.cdr.detectChanges();
  }

  cancel(){
    this.closeWindow.emit();
  }

  updateGetAmount(){
    if(this.transactionForm.get('senderCardId')?.value !=="" && this.transactionForm.get('getterCardId')?.value !=="" && 
    this.transactionForm.get('amount')?.value !==null){
      const senderCard = this.cards?.find(c=>c.id === this.transactionForm.get('senderCardId')?.value);
      const getterCard = this.cards?.find(c=>c.id === this.transactionForm.get('getterCardId')?.value);
      let amount :number = Number(this.transactionForm.get('amount')?.value);
      let fromCource: number = this.getCurrencyBuyRate(senderCard?.chosenCurrency!);
      let toCource: number = this.getCurrencySellRate(getterCard?.chosenCurrency!);
      if(senderCard?.chosenCurrency !==Currency.UAH && getterCard?.chosenCurrency !==Currency.UAH){
        amount = amount * fromCource;
        fromCource =1;
      }
      this.getAmount=(fromCource/toCource)* amount;
      this.getterCurrency = this.getCurrencySymbol(getterCard?.chosenCurrency!);
      this.senderCurrency = this.getCurrencySymbol(senderCard?.chosenCurrency!);
      this.updateComission(senderCard?.cardTariffs.p2PInternalCommission!, this.transactionForm.get('amount')?.value!);
      this.showGetAmount.set(true);
    }
    else{
      this.getAmount=null;
      this.senderCurrency=null;
      this.getterCurrency = null;
      this.commission= null;
      this.showGetAmount.set(false);
      this.showCommission.set(false);
    }
  }

  updateComission(commissionRate : number, amount: number){  
    this.commission = amount ==null? 0: amount * (commissionRate /100);
    this.showCommission.set(true);
  }


  getCurrencyBuyRate(currency: Currency):number{
    switch(currency){
      case(Currency.UAH):{
        return 1;
      }
      case(Currency.USD):{
        return (this.currencyRates?.find(cur=> cur.cc=="USD"))?.neoBankBuyCource!;
      }
      case(Currency.EUR):{
        return (this.currencyRates?.find(cur=> cur.cc=="EUR"))?.neoBankBuyCource!;
      }
      default:{
        return 0;
      }
    }
  }

  getCurrencySellRate(currency: Currency){
    switch(currency){
      case(Currency.UAH):{
        return 1;
      }
      case(Currency.USD):{
        return (this.currencyRates?.find(cur=> cur.cc=="USD"))?.neoBankSellCource!;
      }
      case(Currency.EUR):{
        return (this.currencyRates?.find(cur=> cur.cc=="EUR"))?.neoBankSellCource!;
      }
      default:{
        return 0;
      }
    }
  }

  deleteError(){

  }

  getSenderCurrency(){
    if(this.transactionForm.get('getterCardId')?.value !== ""){
      const chosenCard = this.cards?.find(c=>c.id=== this.transactionForm.get('getterCardId')?.value);
      return this.cards?.filter(c=>c.chosenCurrency !== chosenCard?.chosenCurrency);
    }
    return this.cards;
  }
  
  getGetterCurrency(){
    if(this.transactionForm.get('senderCardId')?.value !== ""){
      const chosenCard = this.cards?.find(c=>c.id=== this.transactionForm.get('senderCardId')?.value);
      return this.cards?.filter(c=>c.chosenCurrency !== chosenCard?.chosenCurrency);
    }
    return this.cards;
  }


  getCurrencySymbol(cardCurrency : Currency | null){
    switch (cardCurrency) {
        case (Currency.UAH): {
          return "₴";
        }
        case (Currency.USD): {
          return "$";
        }
        case (Currency.EUR): {
          return "€";
        }
        default: {
          return "";
        }
      }
  }
}
