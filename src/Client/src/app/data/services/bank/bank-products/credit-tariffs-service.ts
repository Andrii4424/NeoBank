import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../../../environments/environment';
import { Params } from '@angular/router';
import { IPageResult } from '../../../interfaces/page-inteface';
import { ICreditTariffs } from '../../../interfaces/bank/bank-products/credits/credit-tariffs/credit-tariffs';

@Injectable({
  providedIn: 'root'
})
export class CreditTariffsService {
  http = inject(HttpClient);
  baseUrl =`${environment.apiUrl}/CreditTariffs/`;

  getCreditTariffs(params: Params){
    return this.http.get<IPageResult<ICreditTariffs>>(`${this.baseUrl}`, {params})
  }

  getCreditTariffsInfo(id: string){
    return this.http.get<ICreditTariffs>(`${this.baseUrl}${id}`);
  }

  updateCreditTariffs(credit: ICreditTariffs){
    return this.http.put(`${this.baseUrl}`, credit);
  }

  addCreditTariffs(credit: ICreditTariffs){
    return this.http.post(`${this.baseUrl}`, credit);
  }

  deleteCreditTariffs(id: string){
    return this.http.delete(`${this.baseUrl}${id}`);
  }
}
