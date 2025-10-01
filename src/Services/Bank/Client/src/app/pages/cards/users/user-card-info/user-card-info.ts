import { IAddFunds } from './../../../../data/interfaces/bank/bank-products/cards/add-funds-interface';
import { CardType } from './../../../../data/enums/card-type';
import { Currency } from './../../../../data/enums/currency';
import { CardStatus } from './../../../../data/enums/card-status';
import { PaymentSystem } from './../../../../data/enums/payment-system';
import { Observable } from 'rxjs';
import { UserCardsService } from './../../../../data/services/bank/bank-products/user-cards-service';
import { ChangeDetectorRef, Component, inject, signal } from '@angular/core';
import { IUserCards } from '../../../../data/interfaces/bank/bank-products/cards/user-cards-interface';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { AsyncPipe } from '@angular/common';
import { ModalInputWindow } from "../../../../common-ui/modal-input-window/modal-input-window";
import { SuccessMessage } from "../../../../common-ui/success-message/success-message";

@Component({
  selector: 'app-user-card-info',
  imports: [AsyncPipe, RouterLink, ModalInputWindow, SuccessMessage],
  templateUrl: './user-card-info.html',
  styleUrl: './user-card-info.scss'
})
export class UserCardInfo {
  userCardsService = inject(UserCardsService);
  route = inject(ActivatedRoute);
  cardId = this.route.snapshot.paramMap.get("id")!;
  userCard$: Observable<IUserCards> = this.userCardsService.getCardInfo(this.cardId);
  openAddFoundsWindow = signal<boolean>(false);
  successAddingFunds = signal<boolean>(false);
  
  //Enums
  PaymentSystem = PaymentSystem;
  CardStatus = CardStatus;
  Currency = Currency;
  CardType = CardType;

  constructor(private cdr: ChangeDetectorRef){}
  closeModalWindow(){
    this.openAddFoundsWindow.set(false);
  }

  addFunds(fundsCount: number){
    if(fundsCount>0){
      const addFundsParams : IAddFunds={cardId: this.cardId, amount: fundsCount};
      this.userCardsService.addFunds(addFundsParams).subscribe({
        next:()=>{
          this.userCard$ = this.userCardsService.getCardInfo(this.cardId);
          this.successAddingFunds.set(true);
          setTimeout(() => this.successAddingFunds.set(false), 3000); 
        },
        error:(err)=>{
          console.log(err);
        }
      })
    }
    this.openAddFoundsWindow.set(false);
  }
}
