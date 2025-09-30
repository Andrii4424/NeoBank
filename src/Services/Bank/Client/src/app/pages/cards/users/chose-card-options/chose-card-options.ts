import { SharedService } from './../../../../data/services/shared-service';
import { UserCardsService } from './../../../../data/services/bank/bank-products/user-cards-service';
import { CardTariffs } from './../../tariffs/card-tariffs/card-tariffs';
import { CardTariffsService } from './../../../../data/services/bank/bank-products/card-tariffs-service';
import { PaymentSystem } from './../../../../data/enums/payment-system';
import { Currency } from './../../../../data/enums/currency';
import { ChangeDetectorRef, Component, inject, signal } from '@angular/core';
import { ICreateCard } from '../../../../data/interfaces/bank/bank-products/cards/create-card-interface';
import { ICardTariffs } from '../../../../data/interfaces/bank/bank-products/cards/card-tariffs.interface';
import { FormControl, FormGroup, ReactiveFormsModule, Validators, ɵInternalFormsSharedModule } from '@angular/forms';
import { SuccessMessage } from "../../../../common-ui/success-message/success-message";
import { Router } from '@angular/router';


@Component({
  selector: 'app-chose-card-options',
  imports: [CardTariffs, ɵInternalFormsSharedModule, ReactiveFormsModule],
  templateUrl: './chose-card-options.html',
  styleUrl: './chose-card-options.scss'
})
export class ChoseCardOptions {
  isCardTariffsChosen = signal<boolean>(false);
  chosenTariffsName: string | null = null;
  cardTariffsService = inject(CardTariffsService);
  userCardsService = inject(UserCardsService);
  cardTariffs : ICardTariffs | null = null;
  sharedService = inject(SharedService);
  router = inject(Router);
  showValidationResult = signal<boolean>(false);
  validationErrors : string[] = [];

  createCardForm = new FormGroup({
    cardTariffId : new FormControl<string | null>(null),
    chosenCurrency : new FormControl<Currency | null>(null, Validators.required),
    chosenPaymentSystem : new FormControl<PaymentSystem | null>(null, Validators.required),
    pin : new FormControl<string | null>(null, Validators.required),

  });

  constructor(private cdr: ChangeDetectorRef){}

  //Enums
  Currency = Currency;
  PaymentSystem = PaymentSystem;

  choseTariffs(chosenCardTariffs: ICardTariffs){
    this.chosenTariffsName= chosenCardTariffs.cardName;
    this.createCardForm.patchValue({
      cardTariffId: chosenCardTariffs.id
    })
    this.cardTariffsService.getCardTariffsInfo(chosenCardTariffs.id!).subscribe({
      next:(val)=>{
        this.cardTariffs = val;
      },
      error:()=>{
        this.cardTariffs= null;
      },
      complete: ()=>{
        this.cdr.detectChanges();
      }
    });
    this.isCardTariffsChosen.set(true);
    this.cdr.detectChanges();
  } 
  
  onPinInput(event: Event) {
    const input = (event.target as HTMLInputElement).value;
    let pinInput ='';
    pinInput = input.replace(/\D/g, '').slice(0, 4);

    this.createCardForm.patchValue({
      pin: pinInput
    })
  }

  onSubmit(){
    if(this.createCardForm.valid){
      if(this.createCardForm.get('pin')?.value!.length !==4){
        this.validationErrors.push("PIN code must contain 4 characters");
        this.showValidationResult.set(true);
        this.cdr.detectChanges();
      }
      else{
        const formValue = this.createCardForm.value as ICreateCard;
        this.userCardsService.createCard(formValue).subscribe({
          next:()=>{
            this.router.navigate(['/cards/my-cards'], { queryParams: { success: true } })
          },
          error:(err)=>{
            this.validationErrors = this.sharedService.serverResponseErrorToArray(err);
            this.showValidationResult.set(true);
            this.cdr.detectChanges();
          }
        })
      }
    }
    else{
      this.validationErrors.push("All fields has to be provided");
      this.showValidationResult.set(true);
      this.cdr.detectChanges();
    }
  }

  closeValidationStatus(){
    this.showValidationResult.set(false);
    this.validationErrors =[];
  }
  rechoseTariffs(){
    this.isCardTariffsChosen.set(false);
  }
}
