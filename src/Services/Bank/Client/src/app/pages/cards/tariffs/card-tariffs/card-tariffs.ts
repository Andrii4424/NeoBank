import { ProfileService } from './../../../../data/services/auth/profile-service';
import { ChangeDetectorRef, Component, ElementRef, EventEmitter, inject, Input, Output, QueryList, ViewChild, ViewChildren } from '@angular/core';
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

@Component({
  selector: 'app-card-tariffs',
  imports: [PageSwitcher, Search, CardsLayout, RouterLink],
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
