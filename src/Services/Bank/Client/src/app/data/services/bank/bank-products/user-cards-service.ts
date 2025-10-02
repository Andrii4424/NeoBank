import { ICreateCard } from './../../../interfaces/bank/bank-products/cards/create-card-interface';
import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { IUserCards } from '../../../interfaces/bank/bank-products/cards/user-cards-interface';
import { IPageResult } from '../../../interfaces/page-inteface';
import { Params } from '@angular/router';
import { IAddFunds } from '../../../interfaces/bank/bank-products/cards/add-funds-interface';
import { IExchangeCurrency } from '../../../interfaces/bank/bank-products/cards/exchange-currency-interface';

@Injectable({
  providedIn: 'root'
})
export class UserCardsService {
  http = inject(HttpClient);
  baseUrl ="https://localhost:7280/api/UserCards/";

  getMyCards(params: Params){
    return this.http.get<IPageResult<IUserCards>>(`${this.baseUrl}GetMyCards`, {params});
  }

  createCard(cardDto: ICreateCard){
    return this.http.post(`${this.baseUrl}CreateCard`,cardDto);
  }

  getCardInfo(cardId: string){
    return this.http.get<IUserCards>(`${this.baseUrl}GetUsersCardInfo/${cardId}`)
  }

  addFunds(addFundsParams: IAddFunds){
    return this.http.post(`${this.baseUrl}AddFunds`, addFundsParams);
  }

  changePin(payload:{cardId: string, newPin: string}){
    return this.http.put(`${this.baseUrl}ChangePin`, payload);
  }

  exchangeCurrency(currencyParams: IExchangeCurrency){
    return this.http.get<number>(`${this.baseUrl}CreditLimitExchange`, {params: currencyParams as any});
  }
}
