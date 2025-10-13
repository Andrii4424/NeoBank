import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Params } from '@angular/router';
import { IPageResult } from '../../../interfaces/page-inteface';
import { ITransaction } from '../../../interfaces/bank/bank-products/cards/transaction-interface';

@Injectable({
  providedIn: 'root'
})
export class TransactionService {
  http = inject(HttpClient);
  baseUrl = "https://localhost:7281/api/Transaction/";

  getTransactions(payload:{cardId: string, params: Params}){
    return this.http.get<IPageResult<ITransaction>>(`${this.baseUrl}GetTransactions/${payload.cardId}`, {params: payload.params})
  }

}
