import { ChangeDetectorRef, Component, ElementRef, EventEmitter, inject, Input, Output, QueryList, signal, ViewChildren } from '@angular/core';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { CreditsLayout } from "../../credits-layout/credits-layout";
import { ProfileService } from '../../../../../data/services/auth/profile-service';
import { CreditTariffsService } from '../../../../../data/services/bank/bank-products/credit-tariffs-service';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { SharedService } from '../../../../../data/services/shared-service';
import { IPageResult } from '../../../../../data/interfaces/page-inteface';
import { combineLatest, map, Observable } from 'rxjs';
import { ICreditTariffs } from '../../../../../data/interfaces/bank/bank-products/credits/credit-tariffs/credit-tariffs';
import { ISort } from '../../../../../data/interfaces/filters/sort-interface';
import { IFilter } from '../../../../../data/interfaces/filters/filter-interface';
import { PaymentSystem } from '../../../../../data/enums/payment-system';
import { CardType } from '../../../../../data/enums/card-type';
import { ICardTariffs } from '../../../../../data/interfaces/bank/bank-products/cards/card-tariffs.interface';
import { Currency } from '../../../../../data/enums/currency';
import { Search } from "../../../../../common-ui/search/search";
import { Loading } from "../../../../../common-ui/loading/loading";
import { AsyncPipe, DecimalPipe } from '@angular/common';
import { PageSwitcher } from "../../../../../common-ui/page-switcher/page-switcher";
import { ConfirmDelete } from "../../../../../common-ui/confirm-delete/confirm-delete";
import { SuccessMessage } from "../../../../../common-ui/success-message/success-message";

@Component({
  selector: 'app-credit-tariffs',
  imports: [TranslateModule, CreditsLayout, Search, Loading, AsyncPipe, DecimalPipe, RouterModule, PageSwitcher, ConfirmDelete, SuccessMessage],
  templateUrl: './credit-tariffs.html',
  styleUrl: './credit-tariffs.scss'
})
export class CreditTariffs {
  profileService = inject(ProfileService);
  creditTariffsService = inject(CreditTariffsService);
  router = inject(Router);
  route = inject(ActivatedRoute);
  sharedService = inject(SharedService);
  credits?:IPageResult<ICreditTariffs>;
  @Output() chosenTariffs = new EventEmitter<ICreditTariffs>
  @ViewChildren('card') cardElements? : QueryList<ElementRef<HTMLDivElement>>
  isLoading = signal<boolean>(true);
  translate = inject(TranslateService);
  showDeleteWindow = signal<boolean>(false);
  deleteText = '';
  deletedId: string | null = null;
  successMessage: string | null = null;
  showSuccessWindow = signal<boolean>(false);

  //Filters values
  sortValues$: Observable<ISort[]> = combineLatest([
    this.translate.stream('Sort.ByNameAsc'),
    this.translate.stream('Sort.ByNameDesc'),
    this.translate.stream('Sort.ByInterestRateAsc'),
    this.translate.stream('Sort.ByInterestRateDesc'),
  ]).pipe(
    map(([byNameAsc, byNameDesc, byInterestRateAsc, byInterestRateDesc])=>[
      {name: "name-descending", description: byNameAsc},
      {name: "name-ascending", description: byNameDesc},
      {name: "interest-rate-ascending", description: byInterestRateDesc},
      {name: "interest-rate-descending", description: byInterestRateAsc}
    ])
  )

  filterValues$: Observable<IFilter[]> = combineLatest([
    this.translate.stream('Filter.UAH'),
    this.translate.stream('Filter.USD'),
    this.translate.stream('Filter.EUR'),
  ]).pipe(
    map(([uah, usd, eur])=>[
      {filterName:"ChosenCurrency", id: "uah", description: uah, value: Currency.UAH, chosen: false },
      {filterName:"ChosenCurrency", id: "usd", description: usd, value: Currency.USD, chosen: false },
      {filterName:"ChosenCurrency", id: "eur", description: eur, value: Currency.EUR, chosen: false },
    ])
  )

  constructor(private cdr: ChangeDetectorRef) {}

  //Enums
  PaymentSystem = PaymentSystem;
  CardType = CardType;
  
  ngOnInit(){
    this.route.queryParams.subscribe(params =>{
      this.loadCredits(params);
    });
  }

  loadCredits(params: any) {
      this.creditTariffsService.getCreditTariffs(params).subscribe({
        next: (val) => {
          this.credits = val;
          this.isLoading.set(false);

          this.cdr.detectChanges();
        },
        error:()=>{
          this.isLoading.set(false);
        },
        complete:()=>{
          const pageNumber = this.route.snapshot.queryParams['PageNumber'];
          if(this.cardElements?.length===0 && pageNumber>1){
            this.router.navigate([],{
              relativeTo: this.route,
              queryParams: {PageNumber : pageNumber-1},
              queryParamsHandling: 'merge'
            });
          }
        }
      });
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
      const params = this.sharedService.getQueryList(filters);
      this.router.navigate([],{
        relativeTo: this.route,
        queryParams: params,
      });
    }
  }

  openDeleteWindow(id: string, name: string){
    this.deletedId = id;
    this.deleteText = name;
    this.showDeleteWindow.set(true);
  }

  cancelDelete(){
    this.deletedId = null;
    this.showDeleteWindow.set(false);
  }

  confirmDelete(){
    if(this.deletedId){
      this.creditTariffsService.deleteCreditTariffs(this.deletedId).subscribe({
        next:()=>{
          this.showDeleteWindow.set(false);
          this.deletedId = null;
          this.loadCredits(this.route.snapshot.queryParams);
          this.showSuccessMessage(this.translate.instant('CreditTariffDeleted'));
        }
      });
    }
  }

    
  showSuccessMessage(message: string){
    this.successMessage = message;
    this.showSuccessWindow.set(true);
    setTimeout(() => this.showSuccessWindow.set(false), 3000);
  }


  getCurrencies(currencies: Currency[] | null | undefined): string[] {
    if (!currencies) return [];

    return currencies.map(c => {
      switch (c) {
        case Currency.UAH:
          return 'UAH';
        case Currency.USD:
          return 'USD';
        case Currency.EUR:
          return 'EUR';
        default:
          return '';
      }
    });
  }
}
