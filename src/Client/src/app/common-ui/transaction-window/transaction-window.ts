import { TransactionService } from './../../data/services/bank/bank-products/transaction-service';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Currency } from '../../data/enums/currency';
import { IUserCards } from './../../data/interfaces/bank/bank-products/cards/user-cards-interface';
import { ChangeDetectorRef, Component, EventEmitter, inject, Input, Output, signal } from '@angular/core';
import { debounceTime, distinctUntilChanged, filter, Subscription, switchMap } from 'rxjs';
import { DecimalPipe } from '@angular/common';
import { ITransaction } from '../../data/interfaces/bank/bank-products/cards/transaction-interface';
import { TransactionType } from '../../data/enums/transaction-type';
import { CardStatus } from '../../data/enums/card-status';
import { OnlyNumbers } from "../../data/directives/only-numbers";

@Component({
  selector: 'app-transaction-window',
  imports: [ReactiveFormsModule, DecimalPipe, OnlyNumbers],
  templateUrl: './transaction-window.html',
  styleUrl: './transaction-window.scss'
})
export class TransactionWindow {
  transactionService = inject(TransactionService);
  @Input() cards?: IUserCards[];
  @Output() cancelTransaction = new EventEmitter<void>();
  chosenCardRate: number | null = null;
  chosenCurrencySymbol: string | null= null;
  showCommission = signal<boolean>(false);
  showValidationError = signal<boolean>(false);
  commissionRate: number = 0;
  commission: number = 0;
  errorText: string | null = null;
  private senderCardIdSub!:Subscription;
  private amountSub!:Subscription;
  @Output() makeTransaction = new EventEmitter<ITransaction>;

  transactionForm = new FormGroup({
    senderCardId: new FormControl<string | null>("", [Validators.required]),
    getterCardId: new FormControl<string | null>(null, [Validators.required]),
    amount: new FormControl<number | null>(null, [Validators.required]),
  })

  constructor(private cdr: ChangeDetectorRef){};

  ngOnInit(){
    this.senderCardIdSub=this.transactionForm.get('senderCardId')!.valueChanges.subscribe(selectedCardId=>{
      const chosenCard = this.cards?.find(c=> c.id===selectedCardId);
      if(chosenCard !=null){
        this.commissionRate = chosenCard.cardTariffs.p2PInternalCommission ==null? 0:chosenCard.cardTariffs.p2PInternalCommission;
        this.chosenCurrencySymbol= this.getCurrencySymbol(chosenCard.chosenCurrency);
        this.updateComission();
      }
    });
    this.amountSub= this.transactionForm.get('amount')!.valueChanges.subscribe(amount=>{
      this.updateComission();
    });

  }

  ngOnDestroy(){
    if(this.senderCardIdSub){
      this.senderCardIdSub.unsubscribe();

    }
    if(this.amountSub){
      this.amountSub.unsubscribe();
    }
  }

  onSubmit(){
    if(this.transactionForm.valid){
      if(this.transactionForm.get('senderCardId')?.value === ""){
        this.showError("The card from which the shipment will be sent must be selected");
        return;
      }
      this.transactionForm.get('getterCardId')?.setValue(this.transactionForm.get('getterCardId')!.value!.replace(/\D/g, ''));
      if(this.transactionForm.get('getterCardId')?.value?.length !==16){
        this.showError("The recipient's card must contain 16 characters");
        return ;
      }
      const chosenCard = this.cards?.find(c=>c.id == this.transactionForm.get('senderCardId')?.value);
      const availableBalance = chosenCard!.balance + chosenCard!.creditLimit;
      if(availableBalance < this.transactionForm.get('amount')!.value!){
        this.showError("The balance is insufficient to complete the transaction, please top up the card");
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
      
      this.transactionService.getCardIdByNumber(this.transactionForm.get('getterCardId')?.value!).subscribe({
        next:(value)=>{
          this.makeTransaction.emit({senderCardId: this.transactionForm.get('senderCardId')!.value!, 
          getterCardId: value, amount: this.transactionForm.get('amount')!.value!,
          type: TransactionType.P2P})
        },
        error:()=>{
          this.showError("The recipient's card didn't found");
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

  deleteError(){
    this.errorText = null;
    this.showValidationError.set(false);
  }

  updateComission(){
    const amount= this.transactionForm.get('amount')?.value;
    this.showCommission.set(amount != null);
    this.commission = amount ==null? 0: amount * (this.commissionRate /100);
    this.cdr.detectChanges();
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

  cancel(){
    this.cancelTransaction.emit();
  }
}
