import { ProfileService } from './../../../../data/services/auth/profile-service';
import { VacancyService } from '../../../../data/services/bank/users/vacancy-service';
import { Component, inject } from '@angular/core';
import { Search } from "../../../../common-ui/search/search";
import { Observable, Subscription } from 'rxjs';
import { IVacancy } from '../../../../data/interfaces/bank/users/vacancy-interface';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { IPageResult } from '../../../../data/interfaces/page-inteface';
import { AsyncPipe } from '@angular/common';
import { Loading } from "../../../../common-ui/loading/loading";
import { TranslateModule } from '@ngx-translate/core';
import { ISort } from '../../../../data/interfaces/filters/sort-interface';
import { IFilter } from '../../../../data/interfaces/filters/filter-interface';

@Component({
  selector: 'app-vacancies',
  imports: [Search, AsyncPipe, Loading, RouterLink, TranslateModule],
  templateUrl: './vacancies.html',
  styleUrl: './vacancies.scss'
})
export class Vacancies {
  vacancyService = inject(VacancyService);
  route= inject(ActivatedRoute);
  router = inject(Router);
  profileService = inject(ProfileService);
  $vacancy? : Observable<IPageResult<IVacancy>>;
  private queryParamsSubscription!: Subscription;

  sortValues: ISort[]=[
    {name: "salary-descending", description: "By Salary Descending ▼"},
    {name: "salary-ascending", description: "By Salary Ascending ▲"},
    {name: "date-ascending", description: "By Date Descending ▼"},
    {name: "date-descending", description: "By Validity Ascending ▲"}
  ];

  filterValues: IFilter[]=[
    {filterName:"20000-40000", id: "20000-40000", description: "20000-40000₴", value: "20000-40000", chosen: false },
    {filterName:"40000-60000", id: "40000-60000", description: "40000-60000₴", value: "40000-60000", chosen: false },
    {filterName:"60000-80000", id: "60000-80000", description: "60000-80000₴", value: "60000-80000", chosen: false },
    {filterName:"80000", id: "80000", description: "80000₴+", value: "80000", chosen: false },

  ]

  ngOnInit(){
    this.queryParamsSubscription = this.route.queryParams.subscribe(params => {
      this.$vacancy = this.vacancyService.getVacanciesPage(params);
    })
  }
  


  //Filters and pagination handlers
  onSortChange(sortMethod: string){
    this.router.navigate([],{
      relativeTo: this.route,
      queryParams: {SortValue : sortMethod, PageNumber: 1},
      queryParamsHandling: 'merge'
    });
  }

  onSearchChange(searchValue: string){
    console.log("LOX")
    this.router.navigate([],{
      relativeTo: this.route,
      queryParams: {SearchValue : searchValue, PageNumber: 1},
      queryParamsHandling: 'merge'
    });
  }

  onPageChange(pageNumber: number){
    this.router.navigate([],{
      relativeTo: this.route,
      queryParams: {PageNumber : pageNumber},
      queryParamsHandling: 'merge'
    });
  }

  onFiltersChange(filters: IFilter[] | null){
    if(filters===null){
      const savedParamsKeys =["PageNumber", "SearchValue", "SortValue"];
      const params= this.route.snapshot.queryParams;
      const newParams: any = { ...params, PageNumber: 1 };
      Object.keys(params).forEach(key => {
        if(!savedParamsKeys.includes(key)){
          newParams[key] = null;
        }
      });

      this.router.navigate([],{
        relativeTo: this.route,
        queryParams: newParams,
        queryParamsHandling: 'merge'
      });
    }
    else{

    }
  }

}
