import { ICreateCard } from './../../../interfaces/bank/bank-products/cards/create-card-interface';
import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { IUserCards } from '../../../interfaces/bank/bank-products/cards/user-cards-interface';
import { IPageResult } from '../../../interfaces/page-inteface';
import { Params } from '@angular/router';
import { IAddFunds } from '../../../interfaces/bank/bank-products/cards/add-funds-interface';
import { IExchangeCurrency } from '../../../interfaces/bank/bank-products/cards/exchange-currency-interface';
import { CardStatus } from '../../../enums/card-status';
import { ICroppedUserCard } from '../../../interfaces/bank/bank-products/cards/cropped-user-cards-interface';

@Injectable({
  providedIn: 'root'
})
export class UserCardsService {
  http = inject(HttpClient);
  baseUrl ="https://localhost:7280/api/UserCards/";

  getMyCards(params: Params){
    return this.http.get<IPageResult<IUserCards>>(`${this.baseUrl}GetMyCards`, {params});
  }
  getUserFullCards(userId: string){
    return this.http.get<IUserCards[]>(`${this.baseUrl}GetUserCards/${userId}`);
  }
  getUserCroppedCards(userId: string){
    return this.http.get<ICroppedUserCard[]>(`${this.baseUrl}GetCroppedUserCards/${userId}`);
  }

  createCard(cardDto: ICreateCard){
    return this.http.post(`${this.baseUrl}CreateCard`,cardDto);
  }

  getCardInfo(cardId: string){
    return this.http.get<IUserCards>(`${this.baseUrl}GetUsersCardInfo/${cardId}`)
  }

  changePin(payload:{cardId: string, newPin: string}){
    return this.http.put(`${this.baseUrl}ChangePin`, payload);
  }

  exchangeCurrency(currencyParams: IExchangeCurrency){
    return this.http.get<number>(`${this.baseUrl}CreditLimitExchange`, {params: currencyParams as any});
  }

  changeCreditLimit(payload: {cardId: string, newCreditLimit: number}){
    return this.http.put(`${this.baseUrl}ChangeCreditLimit`, payload)
  }

  reissueCard(cardId: string){
    return this.http.put(`${this.baseUrl}ReissueCard/${cardId}`, {})
  }

  changeStatus(payload:{cardId: string, newStatus: CardStatus}){
    return this.http.put(`${this.baseUrl}ChangeCardStatus`, payload);
  }

  closeCard(cardId: string){
    return this.http.delete(`${this.baseUrl}CloseCard/${cardId}`);
  }
}
