import { SharedService } from './../../../../data/services/shared-service';

import { ChangeDetectorRef, Component, ElementRef, Inject, inject, QueryList, signal, ViewChildren } from '@angular/core';
import { ActivatedRoute, RouterLink, RouterOutlet, Params, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AsyncPipe } from '@angular/common';
import { CardsLayout } from '../../cards-layout/cards-layout';
import { Search } from '../../../../common-ui/search/search';
import { PaymentSystem } from '../../../../data/enums/payment-system';
import { Currency } from '../../../../data/enums/currency';
import { CardType } from '../../../../data/enums/card-type';
import { CardLevel } from '../../../../data/enums/card-level';
import { CardStatus } from '../../../../data/enums/card-status';
import { UserCardsService } from '../../../../data/services/bank/bank-products/user-cards-service';
import { IUserCards } from '../../../../data/interfaces/bank/bank-products/cards/user-cards-interface';
import { SuccessMessage } from "../../../../common-ui/success-message/success-message";
import { CardFormatPipe } from '../../../../data/pipes/card-format.pipe';
import { ISort } from '../../../../data/interfaces/filters/sort-interface';
import { IFilter } from '../../../../data/interfaces/filters/filter-interface';
import { IPageResult } from '../../../../data/interfaces/page-inteface';
import { PageSwitcher } from "../../../../common-ui/page-switcher/page-switcher";


@Component({
  selector: 'app-user-cards',
  imports: [CardsLayout, Search, RouterLink, SuccessMessage, CardFormatPipe, PageSwitcher],
  templateUrl: './user-cards.html',
  styleUrl: './user-cards.scss'
})
export class UserCards {
  userCardsService = inject(UserCardsService);
  sharedService = inject(SharedService);
  userCards: IPageResult<IUserCards> | null= null;
  copied = signal<boolean>(false);
  route = inject(ActivatedRoute);
  router = inject(Router);
  success = signal<boolean>(false);
  PaymentSystem = PaymentSystem;
  Currency = Currency;
  CardType = CardType;
  CardStatus = CardStatus;
  CardLevel = CardLevel;
  @ViewChildren('card') cardElements? : QueryList<ElementRef<HTMLDivElement>>

  constructor(private cdr: ChangeDetectorRef){}

    //Filters values
      sortValues: ISort[]=[
      {name: "balance-ascending", description: "By Balance ▲"},
      {name: "balance-descending", description: "By Balance ▼"},
      {name: "validity-period", description: "By Expiry Date"},
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
  

  ngOnInit(){
    this.route.queryParams.subscribe((params: Params) => {
      const successParametr = params['success'];  
      if (successParametr) {
        this.success.set(true);

        this.router.navigate([], {
          queryParams: { success: null },
          queryParamsHandling: 'merge'      
        });
      }

      this.userCardsService.getMyCards(params).subscribe({
        next:(val)=>{

          this.userCards = val;
          this.cdr.detectChanges();
        },
        complete:()=>{
          this.updateCardTextColors();
        }
      });
    });
  }

  copyCardNumber(cardNumber : string){
    this.copied.set(false);
    this.sharedService.copyText(cardNumber);
    this.copied.set(true);
    setTimeout(() => this.copied.set(false), 3000);
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
