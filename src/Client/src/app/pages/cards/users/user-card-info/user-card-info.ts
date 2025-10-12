import { SharedService } from './../../../../data/services/shared-service';
import { IAddFunds } from './../../../../data/interfaces/bank/bank-products/cards/add-funds-interface';
import { CardType } from './../../../../data/enums/card-type';
import { Currency } from './../../../../data/enums/currency';
import { CardStatus } from './../../../../data/enums/card-status';
import { PaymentSystem } from './../../../../data/enums/payment-system';
import { Observable, tap } from 'rxjs';
import { UserCardsService } from './../../../../data/services/bank/bank-products/user-cards-service';
import { ChangeDetectorRef, Component, inject, signal } from '@angular/core';
import { IUserCards } from '../../../../data/interfaces/bank/bank-products/cards/user-cards-interface';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AsyncPipe } from '@angular/common';
import { ModalInputWindow } from "../../../../common-ui/modal-input-window/modal-input-window";
import { SuccessMessage } from "../../../../common-ui/success-message/success-message";
import { ErrorMessage } from "../../../../common-ui/error-message/error-message";
import { IExchangeCurrency } from '../../../../data/interfaces/bank/bank-products/cards/exchange-currency-interface';
import { ConfirmWindow } from "../../../../common-ui/confirm-window/confirm-window";
import { Transactions } from "../transactions/transactions";

@Component({
  selector: 'app-user-card-info',
  imports: [AsyncPipe, RouterLink, ModalInputWindow, SuccessMessage, ErrorMessage, ConfirmWindow, Transactions],
  templateUrl: './user-card-info.html',
  styleUrl: './user-card-info.scss'
})
export class UserCardInfo {
  userCardsService = inject(UserCardsService);
  route = inject(ActivatedRoute);
  router= inject(Router);
  sharedService = inject(SharedService)
  cardId = this.route.snapshot.paramMap.get("id")!;
  exchangeParams? :IExchangeCurrency;
  cardCurrency? : Currency;
  startCreditLimitValue? : number;

  userCard$: Observable<IUserCards> = this.userCardsService.getCardInfo(this.cardId).pipe(
    tap(card=>{
      this.cardCurrency= card.chosenCurrency;
      this.startCreditLimitValue= card.creditLimit;
      if(card.cardTariffs.type===CardType.Credit){
        this.exchangeParams = {from: Currency.UAH , to: card.chosenCurrency, amount: card.cardTariffs.maxCreditLimit!}
      }
    })
  );

  openAddFoundsWindow = signal<boolean>(false);
  openAddChangePinWindow = signal<boolean>(false);
  openChangeCreditLimit = signal<boolean>(false);
  openConfirmWindow = signal<boolean>(false);
  openErrorMessage = signal<boolean>(false);
  openSuccessMessage = signal<boolean>(false);
  
  actionTitle: string ="";
  actionSubtitle: string ="";
  action: string ="";
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
    this.openChangeCreditLimit.set(false);
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

  openAddFounds(){
    this.openAddFoundsWindow.set(true);
    this.cdr.detectChanges();
  }

  openPin(){
    this.openAddChangePinWindow.set(true);
    this.cdr.detectChanges();
  }

  openCreditLimit(){
    this.openChangeCreditLimit.set(true);
    this.cdr.detectChanges();
  }

  reissueCard(){
    this.actionTitle="Confirm Card Reissue";
    this.actionSubtitle="Are you sure you want to reissue the card? The action will not be able to be undone.";
    this.action="reissue-card"
    this.openConfirmWindow.set(true);
    this.cdr.detectChanges();
  }

  blockCard(){
    this.actionTitle="Confirm Blocking";
    this.actionSubtitle="Are you sure you want to block the card? Withdrawal operations will be unavailable while the card is blocked.";
    this.action="block-card"
    this.openConfirmWindow.set(true);
    this.cdr.detectChanges();
  }

  unblockCard(){
    this.actionTitle="Confirm Unblocking";
    this.actionSubtitle="Are you sure you want to unblock the card? Withdrawal operations will be available after unblocking";
    this.action="unblock-card"
    this.openConfirmWindow.set(true);
    this.cdr.detectChanges();
  }

  closeCard(){
    this.actionTitle="Confirm Close";
    this.actionSubtitle="Are you sure you want to close the card? The action will not be able to be undone.";
    this.action="close-card"
    this.openConfirmWindow.set(true);
    this.cdr.detectChanges();
  }
  cancelAction(){
    this.openConfirmWindow.set(false);
    this.actionSubtitle="";
    this.actionTitle=""
    this.action="";
  }
  
  confirmAction(){
    switch (this.action) {
      case "reissue-card":
        this.userCardsService.reissueCard(this.cardId).subscribe({
          next: () => {
            this.userCard$ = this.userCardsService.getCardInfo(this.cardId);
            this.showSuccessMessage("The card has been reissued!");
          },
          error: (err) => {
            this.showErrorMessage(this.sharedService.serverResponseErrorToArray(err)[0]);
          }
        });
        break;

      case "block-card":
        this.userCardsService.changeStatus({ cardId: this.cardId, newStatus: CardStatus.Blocked }).subscribe({
          next: () => {
            this.userCard$ = this.userCardsService.getCardInfo(this.cardId);
            this.showSuccessMessage("The card status has been changed!");
          },
          error: (err) => {
            this.showErrorMessage(this.sharedService.serverResponseErrorToArray(err)[0]);
          }
        });
        break;

      case "unblock-card":
        this.userCardsService.changeStatus({ cardId: this.cardId, newStatus: CardStatus.Active }).subscribe({
          next: () => {
            this.userCard$ = this.userCardsService.getCardInfo(this.cardId);
            this.showSuccessMessage("The card status has been changed!");
          },
          error: (err) => {
            this.showErrorMessage(this.sharedService.serverResponseErrorToArray(err)[0]);
          }
        });
        break;

      case "close-card":
        this.userCardsService.closeCard(this.cardId).subscribe({
          next: () => {
            this.showSuccessMessage("Card has been deleted!");
            this.router.navigate(["cards/my-cards"]);
          },
          error: (err) => {
            this.showErrorMessage(this.sharedService.serverResponseErrorToArray(err)[0]);
          }
        });
        break;

      default:
        break;
    }

    this.cancelAction();
  }

  changeCreditLimit(newCreditLimit: number){
    if(newCreditLimit>=0){
      this.userCardsService.changeCreditLimit({cardId: this.cardId, newCreditLimit: newCreditLimit}).subscribe({
        next:()=>{
          this.userCard$ = this.userCardsService.getCardInfo(this.cardId);
          this.showSuccessMessage("Credit limit changed!");
        },
        error:(err)=>{
          this.showErrorMessage(this.sharedService.serverResponseErrorToArray(err)[0]);
        }
      });
    }
    this.closeModalWindow();
  }

  getCurrencySymbol(){
    switch (this.cardCurrency) {
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
