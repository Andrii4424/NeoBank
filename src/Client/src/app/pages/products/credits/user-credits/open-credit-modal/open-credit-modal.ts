import { CreditTariffsService } from './../../../../../data/services/bank/bank-products/credit-tariffs-service';
import { UserCardsService } from './../../../../../data/services/bank/bank-products/user-cards-service';
import { ChangeDetectorRef, Component, EventEmitter, inject, Input, Output, signal } from '@angular/core';
import { Currency } from '../../../../../data/enums/currency';
import { TransactionType } from '../../../../../data/enums/transaction-type';
import { CardStatus } from '../../../../../data/enums/card-status';
import { TransactionService } from '../../../../../data/services/bank/bank-products/transaction-service';
import { IUserCards } from '../../../../../data/interfaces/bank/bank-products/cards/user-cards-interface';
import { Subscription } from 'rxjs';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { ITransaction } from '../../../../../data/interfaces/bank/bank-products/cards/transaction-interface';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DecimalPipe } from '@angular/common';
import { ICreditTariffs } from '../../../../../data/interfaces/bank/bank-products/credits/credit-tariffs/credit-tariffs';

@Component({
  selector: 'app-open-credit-modal',
  imports: [TranslateModule, ReactiveFormsModule],
  templateUrl: './open-credit-modal.html',
  styleUrl: './open-credit-modal.scss'
})
export class OpenCreditModal {
  transactionService = inject(TransactionService);
  userCardsService = inject(UserCardsService);
  creditTariffsService = inject(CreditTariffsService);

  cards?: IUserCards[];
  CreditTariffs? : ICreditTariffs[];
  @Output() cancelTransaction = new EventEmitter<void>();
  chosenCardRate: number | null = null;
  chosenCurrencySymbol: string | null= null;
  showValidationError = signal<boolean>(false);
  commissionRate: number = 0;
  commission: number = 0;
  errorText: string | null = null;
  private senderCardIdSub!:Subscription;
  private amountSub!:Subscription;
  @Output() openCredit = new EventEmitter<ITransaction>;
  translate = inject(TranslateService);
  selectedTariff = signal<ICreditTariffs | null>(null);
  

  transactionForm = new FormGroup({
    getterCardId: new FormControl<string | null>(null, [Validators.required]),
    creditTariffsId: new FormControl<string | null>(null, [Validators.required]),
    amount: new FormControl<number | null>(null, [Validators.required]),
    creditPeriodMonths: new FormControl<number>(1, Validators.required),
    creditCurrency: new FormControl<Currency | null>(null, Validators.required)
  })

  constructor(private cdr: ChangeDetectorRef){};

  ngOnInit(){
    this.userCardsService.getMyCards({PageNumber: 1, PageSize: 100}).subscribe({
      next: (response) => {
        this.cards = response.items;
      }
    });

    this.transactionForm.get('creditTariffsId')?.valueChanges.subscribe(id => {
      const tariff = this.CreditTariffs?.find(t => t.id === id);
      this.selectedTariff.set(tariff ?? null);

      const currencyControl = this.transactionForm.get('creditCurrency');

      if (tariff) {
        this.transactionForm.get('creditPeriodMonths')?.setValue(tariff.minTermMonths);

        if (tariff.enableCurrency?.length) {
          currencyControl?.setValue(tariff.enableCurrency[0]);
        }
      } else {
        currencyControl?.setValue(null);
      }
    });

    this.creditTariffsService.getCreditTariffs({PageNumber: 1, PageSize: 100}).subscribe({
      next: (response) => {
        this.CreditTariffs = response.items;
      }
    });

    this.transactionForm.get('creditTariffsId')?.valueChanges.subscribe(id => {
      const tariff = this.CreditTariffs?.find(t => t.id === id);

      this.selectedTariff.set(tariff ?? null);

      if (tariff) {
        this.transactionForm.patchValue({
          creditPeriodMonths: tariff.minTermMonths
        });
      }
    });    

    this.senderCardIdSub=this.transactionForm.get('senderCardId')!.valueChanges.subscribe(selectedCardId=>{
      const chosenCard = this.cards?.find(c=> c.id===selectedCardId);
      if(chosenCard !=null){
        this.commissionRate = chosenCard.cardTariffs.p2PInternalCommission ==null? 0:chosenCard.cardTariffs.p2PInternalCommission;
        this.chosenCurrencySymbol= this.getCurrencySymbol(chosenCard.chosenCurrency);
      }
    });
    this.amountSub= this.transactionForm.get('amount')!.valueChanges.subscribe(amount=>{
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
      if(this.transactionForm.get('getterCardId')?.value === ""){
        this.showError(this.translate.instant('OpenCreditModal.NoGetterCard'));
        return;
      }
      const chosenCard = this.cards?.find(c=>c.id == this.transactionForm.get('getterCardId')?.value);
      if(chosenCard?.status !== CardStatus.Active){
        this.showError(this.translate.instant('TransferModal.Errors.InactiveCard'));
        return ;
      }
      if(chosenCard?.chosenCurrency !== this.transactionForm.get('creditCurrency')?.value){
        this.showError(this.translate.instant('OpenCreditModal.InvalidCardCurrency'));
        return ;
      }

      console.log(this.transactionForm.get('creditCurrency')!.value!);
      this.openCredit.emit({getterCardId: this.transactionForm.get('getterCardId')!.value!, amount: this.transactionForm.get('amount')!.value!,
          type: TransactionType.Credit, creditTariffsId: this.transactionForm.get('creditTariffsId')!.value!, 
          termMonths: this.transactionForm.get('creditPeriodMonths')!.value!, isCreditPayment: true, transactionCurrency: this.transactionForm.get('creditCurrency')!.value!})
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

  getCurrencyLabel(currency: Currency): string {
    switch (currency) {
      case Currency.UAH: return 'UAH';
      case Currency.USD: return 'USD';
      case Currency.EUR: return 'EUR';
      default: return '';
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
