import { ChangeDetectorRef, Component, ElementRef, inject, signal, ViewChild } from '@angular/core';
import { ICardTariffs } from '../../../../data/interfaces/bank/bank-products/card-tariffs.interface';
import { ActivatedRoute } from '@angular/router';
import { CardTariffsService } from '../../../../data/services/bank/bank-products/card-tariffs-service';
import { SharedService } from '../../../../data/services/shared-service';
import { PaymentSystem } from '../../../../data/enums/payment-system';
import { Currency } from '../../../../data/enums/currency';
import { FormArray, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CardType } from '../../../../data/enums/card-type';
import { CardLevel } from '../../../../data/enums/card-level';

@Component({
  selector: 'app-add-card-tariffs',
  imports: [ReactiveFormsModule],
  templateUrl: './add-card-tariffs.html',
  styleUrl: './add-card-tariffs.scss'
})
export class AddCardTariffs {
  cardTariffsService = inject(CardTariffsService);
  cards: ICardTariffs | null = null;
  route = inject(ActivatedRoute);
  sharedService = inject(SharedService);
  showValidationResult = signal<boolean>(false);
  successStatus = signal<boolean>(false);
  chosenPaymentSystems: PaymentSystem[] = [];
  chosenCurrencies: Currency[] = [];
  validationErrors : string[] = [];

  currentCreditLimit: number = 0;
  currentInterestRate: number = 0;

  @ViewChild('maxCreditLimit') maxCreditLimitInput!: ElementRef<HTMLInputElement>;
  @ViewChild('interestRate') interestRate!: ElementRef<HTMLInputElement>;
  @ViewChild('cardColor') cardColor!: ElementRef<HTMLInputElement>;

  constructor(private cdr: ChangeDetectorRef){}

  cardForm = new FormGroup({
    id: new FormControl<string | null>(null),
    bankId: new FormControl<string | null>(null),
    cardName: new FormControl<string | null>(null, [Validators.required]),
    type: new FormControl<CardType | null>(null, [Validators.required]),
    level: new FormControl<CardLevel | null>(null, [Validators.required]),
    validityPeriod: new FormControl<number | null>(null, [Validators.required]),
    maxCreditLimit: new FormControl<number | null>(null, [Validators.required]),
    enabledPaymentSystems: new FormArray<FormControl<PaymentSystem>>([]),
    interestRate: new FormControl<number | null>(null, [Validators.required]),
    enableCurrency: new FormArray<FormControl<Currency>>([]),
    annualMaintenanceCost: new FormControl<number | null>(null, [Validators.required]),
    p2PInternalCommission: new FormControl<number | null>(null, [Validators.required]),
    bin: new FormControl<string | null>(null, [Validators.required]),
    cardColor: new FormControl<string | null>(null),
  });


  //Enums
  CardType = CardType;
  CardLevel = CardLevel;
  PaymentSystem = PaymentSystem;
  Currency = Currency;

  closeValidationStatus(){
    this.showValidationResult.set(false);
    this.successStatus.set(false);
    this.cdr.detectChanges();
    this.validationErrors = [];
  }

  onSubmit(){
    if (this.cardForm.valid) {
      const formValue = this.cardForm.value as ICardTariffs;
      formValue.enabledPaymentSystems = this.chosenPaymentSystems;
      formValue.enableCurrency = this.chosenCurrencies;
      formValue.cardColor = this.cardColor.nativeElement.value;
      console.log(formValue);
      this.cardTariffsService.addCardTariffs(formValue).subscribe({
        next: (res) => {
          this.successStatus.set(true);
          this.showValidationResult.set(true);
          this.cdr.detectChanges();
        },
        error:(err)=>{
          this.successStatus.set(false);
          this.validationErrors = this.sharedService.serverResponseErrorToArray(err);
          this.showValidationResult.set(true);
          this.cdr.detectChanges();
        }
      });
    }
    else{
      const formValue = this.cardForm.value as ICardTariffs;
      formValue.enabledPaymentSystems = this.chosenPaymentSystems;
      formValue.enableCurrency = this.chosenCurrencies;
      formValue.cardColor = this.cardColor.nativeElement.value;
      console.log(formValue);
      this.validationErrors = [];
      this.validationErrors.push("All fields has to be provided!");
      this.successStatus.set(false);
      this.showValidationResult.set(true);
      this.cdr.detectChanges();
    }
  }

    onPaymentSystemChange(event: Event) {
    const checkbox = event.target as HTMLInputElement;
    const paymentSystem = Number(checkbox.value) as PaymentSystem;
    if(checkbox.checked) {
      if(!this.chosenPaymentSystems.includes(paymentSystem)) {
        this.chosenPaymentSystems.push(paymentSystem);
      }
    }
    else{
      if(this.chosenPaymentSystems.includes(paymentSystem)) {
        this.chosenPaymentSystems = this.chosenPaymentSystems.filter(ps => ps !== paymentSystem);
      }
    }
  }

  onValidityPeriodChange(event: Event) {
    const input = event.target as HTMLInputElement;
    let value = Number(input.value);
    if (value%1!==0.5 || value%1!==0) {
      value = Math.round(value * 2) / 2;
    }
    this.cardForm.patchValue({ validityPeriod: value });
  }

  onCurrencyChange(event: Event) {
    const checkbox = event.target as HTMLInputElement;
    const currency = Number(checkbox.value) as Currency;
    if(checkbox.checked) {
      if(!this.chosenCurrencies.includes(currency)) {
        this.chosenCurrencies.push(currency);
      }
    }
    else{
      if(this.chosenCurrencies.includes(currency)) {
        this.chosenCurrencies = this.chosenCurrencies.filter(c => c!== currency);
      }
    }
  }

  changeCardType(type: CardType){
    if(type === CardType.Credit){
      this.interestRate.nativeElement.disabled = false;
      this.maxCreditLimitInput.nativeElement.disabled = false;
      this.cardForm.patchValue({
        interestRate: this.currentInterestRate,
        maxCreditLimit: this.currentCreditLimit
      });
    }
    else{
      this.interestRate.nativeElement.disabled = true;
      this.maxCreditLimitInput.nativeElement.disabled = true;
      this.cardForm.patchValue({
        interestRate: 0,
        maxCreditLimit: 0
      });
    }
  }

  //Validatiors
  onCreditLimitChange(event: Event) {
    const input = event.target as HTMLInputElement;
    let value = Number(input.value);
    if (value < 0) {
      value = 0;
    }
    else if (value > 2084597250) {
      value = 2084597250;
    }
    this.cardForm.patchValue({ maxCreditLimit: value });
    this.currentCreditLimit = value;
  }

  onInterestRateChange(event: Event) {
    const input = event.target as HTMLInputElement;
    let value = Number(input.value);
    if (value < 0) {
      value = 0;
    }
    else if (value > 25) {
      value = 25;
    }
    this.cardForm.patchValue({ interestRate: value });
    this.currentInterestRate = value;
  }

  onP2PComissionChange(event : Event) {
    const input = event.target as HTMLInputElement;
    let value = Number(input.value);
    if (value < 0) {
      value = 0;
    }
    this.cardForm.patchValue({ p2PInternalCommission: value });
  }

  onMaintenanceChange(event: Event){
    const input = event.target as HTMLInputElement;
    let value = Number(input.value);
    if (value < 0) {
      value = 0;
    }
    this.cardForm.patchValue({ annualMaintenanceCost: value });
  }
}
