import { ProfileService } from './../../../../data/services/auth/profile-service';
import { ChangeDetectorRef, Component, ElementRef, EventEmitter, inject, Input, Output, QueryList, signal, ViewChild, ViewChildren } from '@angular/core';
import { CardsLayout } from "../../cards-layout/cards-layout";
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { SharedService } from '../../../../data/services/shared-service';
import { PageSwitcher } from '../../../../common-ui/page-switcher/page-switcher';
import { Search } from '../../../../common-ui/search/search';
import { CardTariffsService } from '../../../../data/services/bank/bank-products/card-tariffs-service';
import { IPageResult } from '../../../../data/interfaces/page-inteface';
import { ISort } from '../../../../data/interfaces/filters/sort-interface';
import { IFilter } from '../../../../data/interfaces/filters/filter-interface';
import { CardLevel } from '../../../../data/enums/card-level';
import { PaymentSystem } from '../../../../data/enums/payment-system';
import { CardType } from '../../../../data/enums/card-type';
import { Currency } from '../../../../data/enums/currency';
import { ICardTariffs } from '../../../../data/interfaces/bank/bank-products/cards/card-tariffs.interface';
import { Loading } from "../../../../common-ui/loading/loading";
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { combineLatest, map, Observable } from 'rxjs';
import { AsyncPipe } from '@angular/common';

@Component({
  selector: 'app-card-tariffs',
  imports: [PageSwitcher, Search, CardsLayout, RouterLink, Loading, TranslateModule, AsyncPipe],
  templateUrl: './card-tariffs.html',
  styleUrl: './card-tariffs.scss'
})
export class CardTariffs {
  profileService = inject(ProfileService);
  cardTariffsService = inject(CardTariffsService);
  router = inject(Router);
  route = inject(ActivatedRoute);
  sharedService = inject(SharedService);
  cards?:IPageResult<ICardTariffs>;
  @Input() isHelper? : boolean;
  @Output() chosenTariffs = new EventEmitter<ICardTariffs>
  @ViewChildren('card') cardElements? : QueryList<ElementRef<HTMLDivElement>>
  isLoading = signal<boolean>(true);
  translate = inject(TranslateService);

  //Filters values
  sortValues$: Observable<ISort[]> = combineLatest([
    this.translate.stream('Sort.ByNameAsc'),
    this.translate.stream('Sort.ByNameDesc'),
    this.translate.stream('Sort.ByAnnualMaintenanceCost'),
    this.translate.stream('Sort.ByValidityPeriod'),
  ]).pipe(
    map(([byNameAsc, byNameDesc, byMaintenanceCost, byValidityPeriod])=>[
      {name: "name-descending", description: byNameAsc},
      {name: "name-ascending", description: byNameDesc},
      {name: "annual-maintenance-cost", description: byMaintenanceCost},
      {name: "validity-period", description: byValidityPeriod}
    ])
  )

  filterValues$: Observable<IFilter[]> = combineLatest([
    this.translate.stream('Filter.Premium'),
    this.translate.stream('Filter.Usual'),
    this.translate.stream('Filter.Mastercard'),
    this.translate.stream('Filter.Visa'),
    this.translate.stream('Filter.Credit'),
    this.translate.stream('Filter.Debit'),
    this.translate.stream('Filter.UAH'),
    this.translate.stream('Filter.USD'),
    this.translate.stream('Filter.EUR'),
  ]).pipe(
    map(([premium, usual, mastercard, visa, credit, debit, uah, usd, eur])=>[
      {filterName:"ChosenLevels", id: "premium", description: premium, value: CardLevel.Premium, chosen: false },
      {filterName:"ChosenLevels", id: "usual", description: usual, value: CardLevel.Normal, chosen: false },
      {filterName:"ChosenPaymentSystems", id: "mastercard", description: mastercard, value: PaymentSystem.Mastercard, chosen: false },
      {filterName:"ChosenPaymentSystems", id: "visa", description: visa,value: PaymentSystem.Visa, chosen: false},
      {filterName:"ChosenTypes", id: "credit", description: credit, value: CardType.Credit, chosen: false },
      {filterName:"ChosenTypes", id: "debit", description: debit, value: CardType.Debit, chosen: false },
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
      this.cardTariffsService.getCardTariffs(params).subscribe({
        next: (val) => {
          this.cards = val;
          this.isLoading.set(false);

          this.cdr.detectChanges();
          this.updateCardTextColors();
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
          this.updateCardTextColors();
        }
      });
      this.updateCardTextColors();
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

  acceptTariffs(cardId: string, cardName: string){
    if(this.isHelper){
      let cardOptions : ICardTariffs ={} as ICardTariffs;
      cardOptions.id = cardId;
      cardOptions.cardName = cardName
      this.chosenTariffs.emit(cardOptions);
    }
  }

  //Color methods
  private updateCardTextColors(): void {
    this.cardElements?.forEach(cardRef => {
      const el = cardRef.nativeElement;
      const style = window.getComputedStyle(el);
      const bgColor = style.backgroundColor;
      const rgb = this.extractRGB(bgColor);
      if (!rgb) {
        const child = el.querySelector('.card-bank-logo');
        if(child){
          (child as HTMLImageElement).src="assets/images/bank-logo.png";
        }
        el.style.color = '#102A5A';
        return;
      }

      const [r, g, b] = rgb;

      const brightness = (r * 299 + g * 587 + b * 114) / 1000;

      if (brightness < 128) {
        const child = el.querySelector('.card-bank-logo');
        if(child){
          (child as HTMLImageElement).src="assets/images/white-bank-logo.png";
        }
        el.style.color = 'white';
      } else {
        const child = el.querySelector('.card-bank-logo');
        if(child){
          (child as HTMLImageElement).src="assets/images/bank-logo.png";
        }
        el.style.color = '#102A5A'; 
      }
    });
  }

  private extractRGB(color: string): [number, number, number] | null {
    const match = color.match(/rgba?\((\d+),\s*(\d+),\s*(\d+)/);
    if (!match) return null;
    return [parseInt(match[1]), parseInt(match[2]), parseInt(match[3])];
  }
}
