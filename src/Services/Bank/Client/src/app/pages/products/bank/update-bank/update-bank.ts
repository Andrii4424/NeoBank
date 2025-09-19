import { IBank } from './../../../../data/interfaces/bank/bank.interface';
import { Component, inject, signal } from '@angular/core';
import { BankService } from '../../../../data/services/bank/bank-service';
import { Observable } from 'rxjs';
import { FormControl, FormGroup, Validators, ɵInternalFormsSharedModule, ReactiveFormsModule } from '@angular/forms';
import { SharedService } from '../../../../data/services/shared-service';

@Component({
  selector: 'app-update-bank',
  imports: [ɵInternalFormsSharedModule, ReactiveFormsModule],
  templateUrl: './update-bank.html',
  styleUrl: './update-bank.scss'
})
export class UpdateBank {
  bankService= inject(BankService);
  sharedService = inject(SharedService);
  bank : IBank |null = null;
  showValidationResult = signal<boolean>(false);
  successStatus = signal<boolean>(false);
  validationErrors: string[] =[];

  bankForm = new FormGroup({
    rating: new FormControl<number | null>(null, [Validators.required]),
    hasLicense: new FormControl<boolean | null>(null, [Validators.required]),
    bankFounderFullName: new FormControl<string | null>(null, [Validators.required]),
    bankDirectorFullName: new FormControl<string | null>(null, [Validators.required]),
    capitalization: new FormControl<number | null>(null, [Validators.required]),
    contactPhone: new FormControl<string | null>(null, [Validators.required]),
    legalAddress: new FormControl<string | null>(null, [Validators.required]),
    percentageCommissionForBuyingCurrency: new FormControl<number | null>(null),
    percentageCommissionForSellingCurrency: new FormControl<number | null>(null),
    swiftCode: new FormControl<string | null>(null, [Validators.required]),
    mfoCode: new FormControl<string | null>(null, [Validators.required]),
    taxId: new FormControl<string | null>(null, [Validators.required]),
    contactEmail: new FormControl<string | null>(null, [Validators.required]),
  });

  ngOnInit(){
    this.bankService.getBank().subscribe({
      next:(val)=>{
        this.bank= val;
        this.patchFormValues();
      }
    });
  }

  patchFormValues(){
    this.bankForm.patchValue({
      rating: this.bank?.rating,
      hasLicense: this.bank?.hasLicense,
      bankFounderFullName: this.bank?.bankFounderFullName,
      bankDirectorFullName: this.bank?.bankDirectorFullName,
      capitalization: this.bank?.capitalization,
      contactPhone: this.bank?.contactPhone,
      legalAddress: this.bank?.legalAddress,
      percentageCommissionForBuyingCurrency: this.bank?.percentageCommissionForBuyingCurrency,
      percentageCommissionForSellingCurrency: this.bank?.percentageCommissionForSellingCurrency,
      swiftCode: this.bank?.swiftCode,
      mfoCode: this.bank?.mfoCode,
      taxId: this.bank?.taxId,
      contactEmail: this.bank?.contactEmail
    });
  }

  closeValidationStatus(){
    this.showValidationResult.set(false);
    this.successStatus.set(false);
    this.validationErrors =[];
  }

  onSubmit(){
    if(this.bankForm.valid){
      const updatedBank : IBank ={
        ...this.bank!,
        ...this.bankForm.value
      }
      this.bankService.updateBank(updatedBank).subscribe({
        next:(val)=>{
          this.bank= val;
          this.successStatus.set(true);
          this.showValidationResult.set(true);
        },
        error:(err)=>{
          this.showValidationResult.set(true);
          this.validationErrors= this.sharedService.serverResponseErrorToArray(err);
        }
      });
    }
    else{
      this.validationErrors.push("All fields has to be provided");
      this.showValidationResult.set(true);
    }
  }


  onCapitalizationChange(event : Event){
    const input = event.target as HTMLInputElement;
    let value = Number(input.value);
    if(value<0) value=0;
    else if(value>2084597250) value = 2084597250;

    this.bankForm.get('capitalization')?.setValue(value, { emitEvent: false });
  }

  onRatingChange(event : Event){
    const input = event.target as HTMLInputElement;
    let value = Number(input.value);
    if(value<1) value=1;
    else if(value>5.0) value = 5.0;

    this.bankForm.get('rating')?.setValue(value, { emitEvent: false });
  }

  onPercentForBuyingChange(event : Event){
    const input = event.target as HTMLInputElement;
    let value = Number(input.value);
    if(value<0) value=0;

    this.bankForm.get('percentageCommissionForBuyingCurrency')?.setValue(value, { emitEvent: false });
  }

  onPercentForSellingChange(event : Event){
    const input = event.target as HTMLInputElement;
    let value = Number(input.value);
    if(value<0) value=0;
    this.bankForm.get('percentageCommissionForSellingCurrency')?.setValue(value, { emitEvent: false });
  }
  
}
