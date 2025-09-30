import { CardTariffs } from './../../tariffs/card-tariffs/card-tariffs';
import { CardTariffsService } from './../../../../data/services/bank/bank-products/card-tariffs-service';
import { PaymentSystem } from './../../../../data/enums/payment-system';
import { Currency } from './../../../../data/enums/currency';
import { ChangeDetectorRef, Component, inject, signal } from '@angular/core';
import { ICreateCard } from '../../../../data/interfaces/bank/bank-products/cards/create-card-interface';
import { ICardTariffs } from '../../../../data/interfaces/bank/bank-products/cards/card-tariffs.interface';
import { FormControl, FormGroup, ReactiveFormsModule, Validators, ɵInternalFormsSharedModule } from '@angular/forms';


@Component({
  selector: 'app-chose-card-options',
  imports: [CardTariffs, ɵInternalFormsSharedModule, ReactiveFormsModule],
  templateUrl: './chose-card-options.html',
  styleUrl: './chose-card-options.scss'
})
export class ChoseCardOptions {
  isCardTariffsChosen = signal<boolean>(false);
  chosenTariffsName: string | null = null;
  cardTariffsService = inject(CardTariffsService)
  cardTariffs : ICardTariffs | null = null

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

  rechoseTariffs(){
    this.isCardTariffsChosen.set(false);
  }
}
