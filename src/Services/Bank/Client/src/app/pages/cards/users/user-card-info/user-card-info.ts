import { SharedService } from './../../../../data/services/shared-service';
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
import { ErrorMessage } from "../../../../common-ui/error-message/error-message";

@Component({
  selector: 'app-user-card-info',
  imports: [AsyncPipe, RouterLink, ModalInputWindow, SuccessMessage, ErrorMessage],
  templateUrl: './user-card-info.html',
  styleUrl: './user-card-info.scss'
})
export class UserCardInfo {
  userCardsService = inject(UserCardsService);
  route = inject(ActivatedRoute);
  sharedService = inject(SharedService)
  cardId = this.route.snapshot.paramMap.get("id")!;
  userCard$: Observable<IUserCards> = this.userCardsService.getCardInfo(this.cardId);
  openAddFoundsWindow = signal<boolean>(false);
  openAddChangePinWindow = signal<boolean>(false);
  openErrorMessage = signal<boolean>(false);
  openSuccessMessage = signal<boolean>(false);
  errorMessage: string = "";
  successMessage: string = ""
  successAddingFunds = signal<boolean>(false);
  
  //Enums
  PaymentSystem = PaymentSystem;
  CardStatus = CardStatus;
  Currency = Currency;
  CardType = CardType;

  constructor(private cdr: ChangeDetectorRef){}
  closeModalWindow(){
    this.openAddFoundsWindow.set(false);
    this.openAddChangePinWindow.set(false);
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
          this.showErrorMessage(this.sharedService.serverResponseErrorToArray(err)[0]);
        }
      })
    }
    this.closeModalWindow();
  }

  changePin(newPin: string){
    if(newPin.length !==4){
      this.showErrorMessage("PIN must be 4 digits. Please try again.");
    }
    else{
      this.userCardsService.changePin({cardId: this.cardId, newPin: newPin}).subscribe({
        next:()=>{
          this.userCard$ = this.userCardsService.getCardInfo(this.cardId);
          this.showSuccessMessage("PIN has been changed!");
        },
        error:(err)=>{
          this.showErrorMessage(this.sharedService.serverResponseErrorToArray(err)[0]);
        }
      });
    }
    this.closeModalWindow();
  }

  private showErrorMessage(message: string){
    this.openSuccessMessage.set(false)
    this.errorMessage=message;
    this.openErrorMessage.set(true);
    setTimeout(()=> this.openErrorMessage.set(false), 3000);
  }

  private showSuccessMessage(message: string){
    this.openErrorMessage.set(false)
    this.successMessage=message;
    this.openSuccessMessage.set(true);
    setTimeout(()=> this.openSuccessMessage.set(false), 3000);
  }
}
