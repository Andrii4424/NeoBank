import { ICreateCard } from './../../../interfaces/bank/bank-products/cards/create-card-interface';
import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { IUserCards } from '../../../interfaces/bank/bank-products/cards/user-cards-interface';

@Injectable({
  providedIn: 'root'
})
export class UserCardsService {
  http = inject(HttpClient);
  baseUrl ="https://localhost:7280/api/UserCards/";

  getMyCards(){
    return this.http.get<IUserCards[]>(`${this.baseUrl}GetMyCards`);
  }

  createCard(cardDto: ICreateCard){
    return this.http.post(`${this.baseUrl}CreateCard`,cardDto);
  }
}
