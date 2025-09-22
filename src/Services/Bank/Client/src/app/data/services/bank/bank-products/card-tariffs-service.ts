import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { ICardTariffs } from '../../../interfaces/bank/bank-products/card-tariffs.interface';
import { IPageResult } from '../../../interfaces/page-inteface';

@Injectable({
  providedIn: 'root'
})
export class CardTariffsService {
  http = inject(HttpClient);
  baseUrl="https://localhost:7280/api/CardTariffs/";
  
  getDefaultCardTarifs(){
    return this.http.get<IPageResult<ICardTariffs>>(`${this.baseUrl}GetDefaultPage`);
  }
}
