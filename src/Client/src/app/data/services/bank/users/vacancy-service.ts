import { IVacancy } from './../../../interfaces/bank/users/vacancy-interface';
import { Params } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { IPageResult } from '../../../interfaces/page-inteface';

@Injectable({
  providedIn: 'root'
})
export class VacancyService {
  http = inject(HttpClient);
  baseUrl ="https://localhost:7280/api/Vacancies/";

  getVacanciesPage(params: Params){
    return this.http.get<IPageResult<IVacancy>>(`${this.baseUrl}GetPage`, {params})
  }

  addVacancy(vacancy: IVacancy){
    return this.http.post(`${this.baseUrl}AddVacancy`, vacancy);
  }

  updateVacancy(vacancy: IVacancy){
    return this.http.put(`${this.baseUrl}UpdateVacancy`, vacancy);
  }

    deleteVacancy(vacancyId: string){
    return this.http.delete(`${this.baseUrl}UpdateVacancy/${vacancyId}`);
  }
}
