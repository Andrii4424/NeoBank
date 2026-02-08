import { ChangeDetectorRef, Component, ElementRef, inject, signal, ViewChild } from '@angular/core';
import { CreditTariffsService } from '../../../../../data/services/bank/bank-products/credit-tariffs-service';
import { ICreditTariffs } from '../../../../../data/interfaces/bank/bank-products/credits/credit-tariffs/credit-tariffs';
import { ActivatedRoute } from '@angular/router';
import { SharedService } from '../../../../../data/services/shared-service';
import { PaymentSystem } from '../../../../../data/enums/payment-system';
import { Currency } from '../../../../../data/enums/currency';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  selector: 'app-update-credit-tariffs',
  imports: [TranslateModule, ReactiveFormsModule],
  templateUrl: './update-credit-tariffs.html',
  styleUrl: './update-credit-tariffs.scss'
})
export class UpdateCreditTariffs {
  creditTariffsService = inject(CreditTariffsService);
  cards: ICreditTariffs | null = null;
  route = inject(ActivatedRoute);
  sharedService = inject(SharedService);
  showValidationResult = signal<boolean>(false);
  successStatus = signal<boolean>(false);
  chosenPaymentSystems: PaymentSystem[] = [];
  chosenCurrencies: Currency[] = [];
  validationErrors : string[] = [];
  translate = inject(TranslateService);  

  currentCreditLimit: number = 0;
  currentInterestRate: number = 0;

  @ViewChild('maxCreditLimit') maxCreditLimitInput!: ElementRef<HTMLInputElement>;
  @ViewChild('interestRate') interestRate!: ElementRef<HTMLInputElement>;
  @ViewChild('cardColor') cardColor!: ElementRef<HTMLInputElement>;

  constructor(private cdr: ChangeDetectorRef){}

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');

    if (!id) return;

    this.creditTariffsService
      .getCreditTariffsInfo(id)
      .subscribe({
        next: (val) => {
          this.creditForm.patchValue({
            id: val.id,
            name: val.name,
            interestRate: val.interestRate,
            minAmount: val.minAmount,
            maxAmount: val.maxAmount,
            minTermMonths: val.minTermMonths,
            maxTermMonths: val.maxTermMonths,
            enableCurrency: val.enableCurrency
          });
          this.chosenCurrencies = val.enableCurrency ?? [];
        }
      });
  }


  creditForm = new FormGroup({
    id: new FormControl<string | null>(null),

    name: new FormControl<string | null>(null, [
      Validators.required,
      Validators.maxLength(100)
    ]),

    interestRate: new FormControl<number | null>(null, [
      Validators.required,
      Validators.min(0),
      Validators.max(100)
    ]),

    minAmount: new FormControl<number | null>(null, [
      Validators.required,
      Validators.min(0)
    ]),

    maxAmount: new FormControl<number | null>(null, [
      Validators.required,
      Validators.min(0)
    ]),

    minTermMonths: new FormControl<number | null>(null, [
      Validators.required,
      Validators.min(1)
    ]),

    maxTermMonths: new FormControl<number | null>(null, [
      Validators.required,
      Validators.min(1)
    ]),

    enableCurrency: new FormControl<Currency[] | null>(null, [])
  });


  //Enums
  PaymentSystem = PaymentSystem;
  Currency = Currency;

  closeValidationStatus(){
    this.showValidationResult.set(false);
    this.successStatus.set(false);
    this.cdr.detectChanges();
    this.validationErrors = [];
  }

  onSubmit(){
    if (this.creditForm.valid) {
      const formValue = this.creditForm.value as ICreditTariffs;
      formValue.enableCurrency = this.chosenCurrencies;
      this.creditTariffsService.updateCreditTariffs(formValue).subscribe({
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
      const formValue = this.creditForm.value as ICreditTariffs;
      formValue.enableCurrency = this.chosenCurrencies;
      console.log(formValue);
      this.validationErrors = [];
      this.validationErrors.push(this.translate.instant('Validation.AllFieldsRequired'));
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


  onInterestRateChange(event: Event) {
    const input = event.target as HTMLInputElement;
    let value = Number(input.value);
    if (value < 0) {
      value = 0;
    }
    else if (value > 25) {
      value = 25;
    }
    this.creditForm.patchValue({ interestRate: value });
    this.currentInterestRate = value;
  }

}
