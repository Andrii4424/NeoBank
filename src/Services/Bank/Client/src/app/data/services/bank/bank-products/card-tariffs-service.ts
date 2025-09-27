import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { ICardTariffs } from '../../../interfaces/bank/bank-products/card-tariffs.interface';
import { IPageResult } from '../../../interfaces/page-inteface';
import { Params } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class CardTariffsService {
  http = inject(HttpClient);
  baseUrl="https://localhost:7280/api/CardTariffs/";

  getCardTariffs(params: Params){
    return this.http.get<IPageResult<ICardTariffs>>(`${this.baseUrl}GetPage`, {params})
  }

  getCardTariffsInfo(id: string){
    return this.http.get<ICardTariffs>(`${this.baseUrl}GetCardTariffs/${id}`);
  }

  updateCardTariffs(card: ICardTariffs){
    return this.http.put(`${this.baseUrl}UpdateCard`, card);
  }
}
