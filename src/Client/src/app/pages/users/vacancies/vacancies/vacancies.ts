import { SharedService } from './../../../../data/services/shared-service';
import { ProfileService } from './../../../../data/services/auth/profile-service';
import { VacancyService } from '../../../../data/services/bank/users/vacancy-service';
import { ChangeDetectorRef, Component, inject, signal, ViewChild } from '@angular/core';
import { Search } from "../../../../common-ui/search/search";
import { Observable, Subscription, filter, tap } from 'rxjs';
import { IVacancy } from '../../../../data/interfaces/bank/users/vacancy-interface';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { IPageResult } from '../../../../data/interfaces/page-inteface';
import { AsyncPipe } from '@angular/common';
import { Loading } from "../../../../common-ui/loading/loading";
import { TranslateModule } from '@ngx-translate/core';
import { ISort } from '../../../../data/interfaces/filters/sort-interface';
import { IFilter } from '../../../../data/interfaces/filters/filter-interface';
import { PageSwitcher } from "../../../../common-ui/page-switcher/page-switcher";
import { ConfirmDelete } from "../../../../common-ui/confirm-delete/confirm-delete";
import { SuccessMessage } from "../../../../common-ui/success-message/success-message";
import { ErrorMessage } from "../../../../common-ui/error-message/error-message";

@Component({
  selector: 'app-vacancies',
  imports: [Search, AsyncPipe, Loading, RouterLink, TranslateModule, PageSwitcher, ConfirmDelete, SuccessMessage, ErrorMessage],
  templateUrl: './vacancies.html',
  styleUrl: './vacancies.scss'
})
export class Vacancies {
  vacancyService = inject(VacancyService);
  sharedService = inject(SharedService);
  route= inject(ActivatedRoute);
  router = inject(Router);
  deleteText: string="";
  deleteId: string| null =null;
  profileService = inject(ProfileService);
  $vacancy? : Observable<IPageResult<IVacancy>>;
  private queryParamsSubscription!: Subscription;

  showDeleteWindow = signal<boolean>(false);
  showSuccessWindow = signal<boolean>(false);
  showErrorWindow = signal<boolean>(false);
  errorMessage: string | null= null;
  successMessage: string | null= null;

  @ViewChild('searchComponent') searchComponent!: Search;
  
  constructor(private cdr: ChangeDetectorRef){}


  sortValues: ISort[]=[
    {name: "salary-descending", description: "By Salary Descending ▼"},
    {name: "salary-ascending", description: "By Salary Ascending ▲"},
    {name: "date-ascending", description: "By Publication Date ▼"},
    {name: "date-descending", description: "By Publication Date ▲"}
  ];

  filterValues: IFilter[]=[
    {filterName:"20000-40000", id: "20000-40000", description: "20000-40000₴", value: "20000-40000", chosen: false },
    {filterName:"40000-60000", id: "40000-60000", description: "40000-60000₴", value: "40000-60000", chosen: false },
    {filterName:"60000-80000", id: "60000-80000", description: "60000-80000₴", value: "60000-80000", chosen: false },
    {filterName:"80000", id: "80000", description: "80000₴ +", value: "80000", chosen: false },
  ]

  ngOnInit(){
    this.queryParamsSubscription = this.route.queryParams.subscribe(params => {
      this.$vacancy = this.vacancyService.getVacanciesPage(params);
    })
  }

  ngAfterViewInit(){
    if(this.route.snapshot.queryParams['filter'] != null){
      this.filterValues[this.filterValues.findIndex(val=>val.filterName ===this.route.snapshot.queryParams['filter'])].chosen=true;
      this.cdr.detectChanges();
      this.searchComponent.setUpdatedFilters();
    }
  }

  ngOnDestroy(){
    if(this.queryParamsSubscription){
      this.queryParamsSubscription.unsubscribe();
    }
  }

  openDeleteWindow(id: string, jobTitle: string){
    this.deleteText=`Vacancy ${jobTitle}`;
    this.deleteId=id;
    this.showDeleteWindow.set(true);
  }

  cancelDelete(){
    this.deleteText="";
    this.deleteId=null;
    this.showDeleteWindow.set(false);
  }
  confirmDelete(){
    this.vacancyService.deleteVacancy(this.deleteId!).subscribe({
      next:()=>{
        this.$vacancy = this.vacancyService.getVacanciesPage(this.route.snapshot.queryParams).pipe(
          tap(vacanciesPage=>{
            if(vacanciesPage.items.length ===0 && vacanciesPage.hasPreviousPage){
              this.router.navigate([],{
                relativeTo: this.route,
                queryParams: {PageNumber : vacanciesPage.pageNumber-1},
                queryParamsHandling: 'merge'
              });
            }
          })
        );

        this.showSuccessMessage("Vacancy has been deleted");
      },
      error:(err)=>{
        this.showErrorMessage(this.sharedService.serverResponseErrorToArray(err)[0]);
      }
    })
    this.cancelDelete();
  }
  
  showSuccessMessage(message: string){
    this.successMessage = message;
    this.showSuccessWindow.set(true);
    setTimeout(() => this.showSuccessWindow.set(false), 3000);
  }

  showErrorMessage(message: string){
    this.errorMessage = message;
    this.showErrorWindow.set(true);
    setTimeout(() => this.showErrorWindow.set(false), 3000);
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
      if(filters.length ===0){
        this.setFilters(null, null, null);
      }
      if(filters.length >1){
        const filter = filters.find(val=>val.value !== this.route.snapshot.queryParams["filter"])?.value;
        this.getAndSetFilters(filter);
      }
      else{
        this.getAndSetFilters(filters[0].value)
      }
    }
  }

  getAndSetFilters(filter : string | null){
    switch(filter){
      case(null):
        this.setFilters(null, null, null);
        break;
      case("20000-40000"):
        this.setFilters(20000, 40000, "20000-40000");
        break;
      case("40000-60000"):
        this.setFilters(40000, 60000, "40000-60000");
        break;
      case("60000-80000"):
        this.setFilters(60000, 80000, "60000-80000");
        break;
      case("80000"):
        this.setFilters(80000, null, "80000");
        break;
      default:
        this.setFilters(null, null, "80000");
        break;
    }
  }

  setFilters(minAmount : number | null, maxAmount : number | null, filterName: string | null){
    this.router.navigate([],{
      relativeTo: this.route,
      queryParams: {MinSalary : minAmount, MaxSalary: maxAmount, filter: filterName,  PageNumber: 1},
      queryParamsHandling: 'merge'
    });
    
    this.filterValues.forEach(filter => {
      filter.chosen=false;
    });
    this.filterValues[this.filterValues.findIndex(val=>val.filterName ===filterName)].chosen=true;
    this.cdr.detectChanges();
    this.searchComponent.setUpdatedFilters();
  }
}
