import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../../../environments/environment';
import { Params } from '@angular/router';
import { IPageResult } from '../../../interfaces/page-inteface';
import { UserCredit } from '../../../interfaces/bank/users/user-credits.models.';

@Injectable({
  providedIn: 'root'
})
export class UserCreditService {
  http = inject(HttpClient);
  baseBankApiUrl =`${environment.apiUrl}/credits/users/`;
  baseTransactionsApiUrl =`${environment.apiUrl}/Transactions/`;

  getCreditsPage(params: Params){
    return this.http.get<IPageResult<UserCredit>>(`${this.baseBankApiUrl}`, {params})
  }

}
