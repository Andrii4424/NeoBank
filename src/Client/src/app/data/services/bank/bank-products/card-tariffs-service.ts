import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { IPageResult } from '../../../interfaces/page-inteface';
import { Params } from '@angular/router';
import { ICardTariffs } from '../../../interfaces/bank/bank-products/cards/card-tariffs.interface';
import { environment } from '../../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CardTariffsService {
  http = inject(HttpClient);
  baseUrl =`${environment.apiUrl}/CardTariffs/`;

  getCardTariffs(params: Params){
    return this.http.get<IPageResult<ICardTariffs>>(`${this.baseUrl}GetPage`, {params})
  }

  getCardTariffsInfo(id: string){
    return this.http.get<ICardTariffs>(`${this.baseUrl}GetCardTariffs/${id}`);
  }

  updateCardTariffs(card: ICardTariffs){
    return this.http.put(`${this.baseUrl}UpdateCard`, card);
  }

  addCardTariffs(card: ICardTariffs){
    return this.http.post(`${this.baseUrl}AddCard`, card);
  }

  deleteCardTariffs(id: string){
    return this.http.delete(`${this.baseUrl}DeleteCard/${id}`);
  }
}
