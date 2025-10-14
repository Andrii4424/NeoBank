import { TransactionService } from './../../data/services/bank/bank-products/transaction-service';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Currency } from '../../data/enums/currency';
import { IUserCards } from './../../data/interfaces/bank/bank-products/cards/user-cards-interface';
import { ChangeDetectorRef, Component, EventEmitter, inject, Input, Output, signal } from '@angular/core';
import { debounceTime, distinctUntilChanged, filter, Subscription, switchMap } from 'rxjs';
import { DecimalPipe } from '@angular/common';

@Component({
  selector: 'app-transaction-window',
  imports: [ReactiveFormsModule, DecimalPipe],
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
  commissionRate: number = 0;
  commission: number = 0;
  private senderCardIdSub!:Subscription;
  private amountSub!:Subscription;

  transactionForm = new FormGroup({
    senderCardId: new FormControl<string | null>(null, [Validators.required]),
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
  updateComission(){
    const amount= this.transactionForm.get('amount')?.value;
    this.showCommission.set(amount != null);
    this.commission = amount ==null? 0: amount * (this.commissionRate /100);
    this.cdr.detectChanges();
  }

  ngOnDestroy(){
    if(this.senderCardIdSub){
      this.senderCardIdSub.unsubscribe();

    }
    if(this.amountSub){
      this.amountSub.unsubscribe();
    }
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
