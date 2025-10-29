import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Params } from '@angular/router';
import { IPageResult } from '../../../interfaces/page-inteface';
import { ITransaction } from '../../../interfaces/bank/bank-products/cards/transaction-interface';
import { IAddFunds } from '../../../interfaces/bank/bank-products/cards/add-funds-interface';
import { environment } from '../../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class TransactionService {
  http = inject(HttpClient);
  baseUrl =`${environment.transactionsApiUrl}/Transaction/`;

  getTransactions(payload:{cardId: string, params: Params}){
    return this.http.get<IPageResult<ITransaction>>(`${this.baseUrl}GetTransactions/${payload.cardId}`, {params: payload.params})
  }

  getCardIdByNumber(cardNumber: string){
    return this.http.get<string>(`${this.baseUrl}GetCardId/${cardNumber}`);
  }

  getCommissionRate(payload: {cardId: string, amount: number}){
    return this.http.get<number>(`${this.baseUrl}GetComissionRate/${payload.cardId}/${payload.amount}`);
  }

  makeTransaction(transactionDetails : ITransaction){
    return this.http.post(`${this.baseUrl}MakeTransaction`, transactionDetails);
  }

  addFunds(transactionDetails: IAddFunds){
    return this.http.post(`${this.baseUrl}AddFunds`, transactionDetails)
  }
}
