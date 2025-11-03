import { TransactionService } from './../../../../data/services/bank/bank-products/transaction-service';
import { ITransaction } from './../../../../data/interfaces/bank/bank-products/cards/transaction-interface';
import { SharedService } from './../../../../data/services/shared-service';

import { ChangeDetectorRef, Component, ElementRef, Inject, inject, QueryList, signal, ViewChildren } from '@angular/core';
import { ActivatedRoute, RouterLink, RouterOutlet, Params, Router } from '@angular/router';
import { combineLatest, map, Observable, Subscription } from 'rxjs';
import { AsyncPipe, DecimalPipe } from '@angular/common';
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
import { TransactionWindow } from "../../../../common-ui/transaction-window/transaction-window";
import { Loading } from "../../../../common-ui/loading/loading";
import { ErrorMessage } from "../../../../common-ui/error-message/error-message";
import { TranslateModule, TranslateService } from '@ngx-translate/core';


@Component({
  selector: 'app-user-cards',
  imports: [CardsLayout, Search, RouterLink, SuccessMessage, CardFormatPipe, PageSwitcher, TransactionWindow, Loading, ErrorMessage, 
    TranslateModule, AsyncPipe],
  templateUrl: './user-cards.html',
  styleUrl: './user-cards.scss'
})
export class UserCards {
  userCardsService = inject(UserCardsService);
  sharedService = inject(SharedService);
  transactionService = inject(TransactionService);
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
  successTransaction = signal<boolean>(false);
  showError = signal<boolean>(false);
  errorMessage: string ="";
  openTransactionWindow = signal<boolean>(false);
  @ViewChildren('card') cardElements? : QueryList<ElementRef<HTMLDivElement>>
  isLoading = signal<boolean>(true);
  translate = inject(TranslateService);
  querySub!: Subscription;

  constructor(private cdr: ChangeDetectorRef){}

  //Filters values    
  sortValues$: Observable<ISort[]> = combineLatest([
    this.translate.stream('Sort.ByBalanceDesc'),
    this.translate.stream('Sort.ByBalanceAsc'),
    this.translate.stream('Sort.ByValidityPeriod'),
  ]).pipe(
    map(([byBalanceDesc, byBalanceAsc, byValidityPeriod])=>[
      {name: "balance-descending", description: byBalanceDesc},
      {name: "balance-ascending", description: byBalanceAsc},
      {name: "validity-period", description: byValidityPeriod},
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

  ngOnInit(){
    this.querySub=this.route.queryParams.subscribe((params: Params) => {
      const successParametr = params['success'];  
      if (successParametr) {
        this.success.set(true);

        this.router.navigate([], {
          queryParams: { success: null },
          queryParamsHandling: 'merge'      
        });
      }

      this.getCards(params);
    });
  }

  ngOnDestroy(){
    if(this.querySub){
      this.querySub.unsubscribe();
    }
  }

  getCards(params: Params){
    this.userCardsService.getMyCards(params).subscribe({
        next:(val)=>{
          this.userCards = val;
          this.isLoading.set(false);
          this.cdr.detectChanges();
        },
        error:()=>{
          this.isLoading.set(false);
        },
        complete:()=>{
          this.updateCardTextColors();
        }
    });
  }

  copyCardNumber(cardNumber : string, event: Event){
    event.stopPropagation();
    event.preventDefault();

    this.copied.set(false);
    this.sharedService.copyText(cardNumber);
    this.copied.set(true);
    setTimeout(() => this.copied.set(false), 3000);
  }

  //Transactions
  makeTransaction(transactionDetails: ITransaction){
    this.transactionService.makeTransaction(transactionDetails).subscribe({
      next:()=>{
        this.successTransaction.set(true);
        this.openTransactionWindow.set(false);
        setTimeout(() => this.successTransaction.set(false), 3000);
        this.getCards(this.route.snapshot.queryParams);
      },
      error:(err)=>{
        this.errorMessage=this.sharedService.serverResponseErrorToArray(err)[0];
        this.showError.set(true);
        this.openTransactionWindow.set(false);
        setTimeout(() => this.showError.set(false), 3000);
      }
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
