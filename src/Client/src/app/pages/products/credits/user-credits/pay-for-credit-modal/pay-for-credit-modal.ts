import { UserCreditService } from './../../../../../data/services/bank/users/user-credit-service';
import { ChangeDetectorRef, Component, EventEmitter, inject, Input, Output, signal } from '@angular/core';
import { Currency } from '../../../../../data/enums/currency';
import { CardStatus } from '../../../../../data/enums/card-status';
import { TransactionService } from '../../../../../data/services/bank/bank-products/transaction-service';
import { IUserCards } from '../../../../../data/interfaces/bank/bank-products/cards/user-cards-interface';
import { Subscription } from 'rxjs';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TransactionType } from '../../../../../data/enums/transaction-type';
import { ITransaction } from '../../../../../data/interfaces/bank/bank-products/cards/transaction-interface';
import { IPageResult } from '../../../../../data/interfaces/page-inteface';
import { UserCredit } from '../../../../../data/interfaces/bank/users/user-credits.models.';
import { UserCardsService } from '../../../../../data/services/bank/bank-products/user-cards-service';

@Component({
  selector: 'app-pay-for-credit-modal',
  imports: [TranslateModule, ReactiveFormsModule],
  templateUrl: './pay-for-credit-modal.html',
  styleUrl: './pay-for-credit-modal.scss'
})
export class PayForCreditModal {
  transactionService = inject(TransactionService);
  userCardsService = inject(UserCardsService);

  
  @Input() cards?: IUserCards[];
  @Output() cancelTransaction = new EventEmitter<void>();
  chosenCardRate: number | null = null;
  chosenCurrencySymbol: string | null= null;
  showValidationError = signal<boolean>(false);
  errorText: string | null = null;
  @Output() makeTransaction = new EventEmitter<ITransaction>;
  translate = inject(TranslateService);
  @Input() credit? :UserCredit;
  

  transactionForm = new FormGroup({
    senderCardId: new FormControl<string | null>("", [Validators.required]),
    amount: new FormControl<number | null>(null, [Validators.required]),
  })

  constructor(private cdr: ChangeDetectorRef){};

  ngOnInit(){
    this.userCardsService.getMyCards({PageNumber: 1, PageSize: 100}).subscribe({
      next: (response) => {
        this.cards = response.items;
      }
    });
  }

  onSubmit(){
    if(this.transactionForm.valid){
      if(this.transactionForm.get('senderCardId')?.value === ""){
        this.showError(this.translate.instant('TransferModal.Errors.NoSenderCard'));
        return;
      }
      const chosenCard = this.cards?.find(c=>c.id == this.transactionForm.get('senderCardId')?.value);
      const availableBalance = chosenCard!.balance + chosenCard!.creditLimit;
      if(availableBalance < this.transactionForm.get('amount')!.value!){
        this.showError(this.translate.instant('TransferModal.Errors.InsufficientBalance'));
        return ;
      }
      if(chosenCard?.status !== CardStatus.Active){
        this.showError(this.translate.instant('TransferModal.Errors.InactiveCard'));
        return ;
      }
      if(chosenCard.cardNumber === this.transactionForm.get('getterCardId')?.value!){
        this.showError(this.translate.instant('TransferModal.Errors.SameCard'));
        return ;
      }
      this.makeTransaction.emit({senderCardId: this.transactionForm.get('senderCardId')!.value!, 
          amount: this.transactionForm.get('amount')!.value!, type: TransactionType.Credit, isCreditPayment: true, 
          userCreditId: this.credit?.id, senderCurrency: chosenCard.chosenCurrency})
    }
    else{
      this.showError(this.translate.instant('TransferModal.Errors.AllFieldsRequired'));
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
