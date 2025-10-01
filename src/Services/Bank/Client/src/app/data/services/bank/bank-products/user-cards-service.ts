import { ICreateCard } from './../../../interfaces/bank/bank-products/cards/create-card-interface';
import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { IUserCards } from '../../../interfaces/bank/bank-products/cards/user-cards-interface';
import { IPageResult } from '../../../interfaces/page-inteface';
import { Params } from '@angular/router';

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
}
