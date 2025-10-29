import { IVacancy } from './../../../interfaces/bank/users/vacancy-interface';
import { Params } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { IPageResult } from '../../../interfaces/page-inteface';
import { environment } from '../../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class VacancyService {
  http = inject(HttpClient);
  baseUrl =`${environment.apiUrl}/Vacancies/`;

  getVacanciesPage(params: Params){
    return this.http.get<IPageResult<IVacancy>>(`${this.baseUrl}GetPage`, {params})
  }

  getVacancy(vacancyId: string){
    return this.http.get<IVacancy>(`${this.baseUrl}GetVacancy/${vacancyId}`);
  }

  addVacancy(vacancy: IVacancy){
    return this.http.post(`${this.baseUrl}AddVacancy`, vacancy);
  }

  updateVacancy(vacancy: IVacancy){
    return this.http.put(`${this.baseUrl}UpdateVacancy`, vacancy);
  }

  deleteVacancy(vacancyId: string){
    return this.http.delete(`${this.baseUrl}DeleteVacancy/${vacancyId}`);
  }
}
