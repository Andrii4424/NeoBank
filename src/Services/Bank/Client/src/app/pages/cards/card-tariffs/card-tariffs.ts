import { SharedService } from './../../../data/services/shared-service';
import { CardType } from './../../../data/enums/card-type';
import { PaymentSystem } from '../../../data/enums/payment-system';
import { ICardTariffs } from '../../../data/interfaces/bank/bank-products/card-tariffs.interface';
import { IPageResult } from '../../../data/interfaces/page-inteface';
import { CardTariffsService } from './../../../data/services/bank/bank-products/card-tariffs-service';
import { ChangeDetectorRef, Component, ElementRef, inject, QueryList, ViewChildren } from '@angular/core';
import { PageSwitcher } from "../../../common-ui/page-switcher/page-switcher";
import { Search } from "../../../common-ui/search/search";
import { ISort } from '../../../data/interfaces/filters/sort-interface';
import { IFilter } from '../../../data/interfaces/filters/filter-interface';
import { CardLevel } from '../../../data/enums/card-level';
import { Currency } from '../../../data/enums/currency';
import { CardsLayout } from "../cards-layout/cards-layout";
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-card-tariffs',
  imports: [PageSwitcher, Search, CardsLayout],
  templateUrl: './card-tariffs.html',
  styleUrl: './card-tariffs.scss'
})
export class CardTariffs {
  cardTariffsService = inject(CardTariffsService);
  router = inject(Router);
  route = inject(ActivatedRoute);
  sharedService = inject(SharedService);
  cards?:IPageResult<ICardTariffs>;
  @ViewChildren('card') cardElements? : QueryList<ElementRef<HTMLDivElement>>

  //Filters values
    sortValues: ISort[]=[
      {name: "name-descending", description: "By Name Descending ▼"},
      {name: "name-ascending", description: "By Name Ascending ▲"},
      {name: "annual-maintenance-cost", description: "By Annual Maintenance Cost"},
      {name: "validity-period", description: "By Validity Period"}
  ];
  
  searchPlaceholder: string ="Enter the card name";
  
  filterValues: IFilter[]=[
    {filterName:"ChosenLevels", id: "premium", description: "Premium", value: CardLevel.Premium, chosen: false },
    {filterName:"ChosenLevels", id: "usual", description: "Usual", value: CardLevel.Normal, chosen: false },
    {filterName:"ChosenPaymentSystems", id: "mastercard", description: "Mastercard", value: PaymentSystem.Mastercard, chosen: false },
    {filterName:"ChosenPaymentSystems", id: "visa", description: "Visa",value: PaymentSystem.Visa, chosen: false},
    {filterName:"ChosenTypes", id: "credit", description: "Credit", value: CardType.Credit, chosen: false },
    {filterName:"ChosenTypes", id: "debit", description: "Debit", value: CardType.Debit, chosen: false },
    {filterName:"ChosenCurrency", id: "uah", description: "UAH", value: Currency.UAH, chosen: false },
    {filterName:"ChosenCurrency", id: "usd", description: "USD", value: Currency.USD, chosen: false },
    {filterName:"ChosenCurrency", id: "eur", description: "EUR", value: Currency.EUR, chosen: false },
  ]

  constructor(private cdr: ChangeDetectorRef) {}

  //Enums
  PaymentSystem = PaymentSystem;
  CardType = CardType;
  
  ngOnInit(){
    this.route.queryParams.subscribe(params =>{
      this.cardTariffsService.getCardTariffs(params).subscribe({
        next: (val) => {
          this.cards = val;
          this.cdr.detectChanges();
          this.updateCardTextColors();
        }
      });
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

  onSearchChange(serchValue: string){
    this.router.navigate([],{
      relativeTo: this.route,
      queryParams: {SearchValue : serchValue, PageNumber: 1},
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


  //Color methods
  private updateCardTextColors(): void {
    this.cardElements?.forEach(cardRef => {
      const el = cardRef.nativeElement;
      const style = window.getComputedStyle(el);
      const bgColor = style.backgroundColor;
      const rgb = this.extractRGB(bgColor);
      if (!rgb) {
        el.style.color = '#102A5A';
        return;
      }

      const [r, g, b] = rgb;

      const brightness = (r * 299 + g * 587 + b * 114) / 1000;

      if (brightness < 128) {
        el.style.color = 'white'; 
      } else {
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
