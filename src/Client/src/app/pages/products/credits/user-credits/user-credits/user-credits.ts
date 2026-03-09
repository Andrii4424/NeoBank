import { ProfileService } from './../../../../../data/services/auth/profile-service';
import { TransactionService } from './../../../../../data/services/bank/bank-products/transaction-service';
import { OpenCreditModal } from './../open-credit-modal/open-credit-modal';
import { UserCreditService } from '../../../../../data/services/bank/users/user-credit-service';
import { ChangeDetectorRef, Component, ElementRef, inject, QueryList, signal, ViewChildren } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { CreditsLayout } from '../../credits-layout/credits-layout';
import { combineLatest, map, Observable } from 'rxjs';
import { ISort } from '../../../../../data/interfaces/filters/sort-interface';
import { IFilter } from '../../../../../data/interfaces/filters/filter-interface';
import { Currency } from '../../../../../data/enums/currency';
import { CardType } from '../../../../../data/enums/card-type';
import { PaymentSystem } from '../../../../../data/enums/payment-system';
import { SharedService } from '../../../../../data/services/shared-service';
import { IPageResult } from '../../../../../data/interfaces/page-inteface';
import { UserCredit } from '../../../../../data/interfaces/bank/users/user-credits.models.';
import { Search } from "../../../../../common-ui/search/search";
import { AsyncPipe, DatePipe, DecimalPipe } from '@angular/common';
import { Loading } from "../../../../../common-ui/loading/loading";
import { ITransaction } from '../../../../../data/interfaces/bank/bank-products/cards/transaction-interface';
import { SuccessMessage } from "../../../../../common-ui/success-message/success-message";
import { ErrorMessage } from "../../../../../common-ui/error-message/error-message";
import { PageSwitcher } from "../../../../../common-ui/page-switcher/page-switcher";
import { CreditStatus } from '../../../../../data/enums/credit-status';
import { PayForCreditModal } from "../pay-for-credit-modal/pay-for-credit-modal";
import { CreditsDetailsModal } from "../credits-details-modal/credits-details-modal";


@Component({
  selector: 'app-user-credits',
  imports: [TranslateModule, RouterModule, CreditsLayout, Search, AsyncPipe, Loading, DecimalPipe, DatePipe, OpenCreditModal, SuccessMessage, ErrorMessage, PageSwitcher, PayForCreditModal, CreditsDetailsModal],
  templateUrl: './user-credits.html',
  styleUrl: './user-credits.scss'
})
export class UserCredits {
  router = inject(Router);
  route = inject(ActivatedRoute);
  profileService = inject(ProfileService);
  sharedService = inject(SharedService);
  translate = inject(TranslateService);
  @ViewChildren('card') cardElements? : QueryList<ElementRef<HTMLDivElement>>
  userCreditService = inject(UserCreditService);
  credits?:IPageResult<UserCredit>;
  isLoading = signal<boolean>(true);
  openCreditModal = signal<boolean>(false);
  transactionService = inject(TransactionService);
  showSuccessWindow = signal<boolean>(false);
  successMessage : string | null = null;
  openErrorMessage = signal<boolean>(false);
  errorMessage: string | null = null;
  openPayForCreditModal = signal<boolean>(false);
  openCreditDetailsModal = signal<UserCredit | null>(null);
  selectedCredit: UserCredit | null = null;

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
    this.translate.stream('ShowClosedCredits')
  ]).pipe(
    map(([uah, usd, eur, showClosedCredits])=>[
      {filterName:"ChosenCurrency", id: "uah", description: uah, value: Currency.UAH, chosen: false },
      {filterName:"ChosenCurrency", id: "usd", description: usd, value: Currency.USD, chosen: false },
      {filterName:"ChosenCurrency", id: "eur", description: eur, value: Currency.EUR, chosen: false },
      {filterName:"ShowClosedCredits", id: "show-closed", description: showClosedCredits, value: true, chosen: false },

    ])
  )

  constructor(private cdr: ChangeDetectorRef) {}

  //Enums
  PaymentSystem = PaymentSystem;
  CardType = CardType;
  CreditStatus = CreditStatus;
  
  ngOnInit(){
    this.route.queryParams.subscribe(params =>{
      this.loadCredits(params);
    });
  }

  loadCredits(params: any) {
      this.userCreditService.getCreditsPage(params).subscribe({
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

  onOpenCredit(transaction: ITransaction){
    transaction.getterId = localStorage.getItem("userId")!;
    this.transactionService.makeTransaction(transaction).subscribe({
      next:()=>{
        this.openCreditModal.set(false);
        this.loadCredits(this.route.snapshot.queryParams);
        this.showSuccessMessage(this.translate.instant('OpenCreditModal.OpenCreditSuccessMessage'));
      },
      error:(err)=>{
        this.openCreditModal.set(false);
        this.showErrorMessage(this.sharedService.serverResponseErrorToArray(err)[0]);
      }
    });
  }

  onMakeCreditPayment(transaction: ITransaction){
    this.transactionService.makeTransaction(transaction).subscribe({
      next:()=>{
        this.openPayForCreditModal.set(false);
        this.loadCredits(this.route.snapshot.queryParams);
        this.showSuccessMessage(this.translate.instant('CreditPaymentSuccessMessage'));
      },
      error:(err)=>{
        this.openPayForCreditModal.set(false);
        this.showErrorMessage(this.sharedService.serverResponseErrorToArray(err)[0]);
      }
    });
  }
  
  onOpenPayForCreditModal(credit: UserCredit){
    this.selectedCredit = credit;
    this.openPayForCreditModal.set(true);
  }

  onCLosePayForCreditModal(){
    this.selectedCredit = null;
    this.openPayForCreditModal.set(false);
  }
      
  showSuccessMessage(message: string){
    this.successMessage = message;
    this.showSuccessWindow.set(true);
    setTimeout(() => this.showSuccessWindow.set(false), 3000);
  }

  private showErrorMessage(message: string){
    this.showSuccessWindow.set(false)
    this.errorMessage=message;
    this.openErrorMessage.set(true);
    setTimeout(()=> this.openErrorMessage.set(false), 3000);
  }


    getCurrency(currency: Currency): string {
        switch (currency) {
          case Currency.UAH:
            return 'UAH';
          case Currency.USD:
            return 'USD';
          case Currency.EUR:
            return 'EUR';
          default:
            return '';
        }
    }

  getCreditStatusText(status: CreditStatus){
    switch (status) {
      case CreditStatus.Active:
        return this.translate.instant('CreditStatus.Active');
      case CreditStatus.Closed:
        return this.translate.instant('CreditStatus.Closed');
      case CreditStatus.Rejected:
        return this.translate.instant('CreditStatus.Rejected');
      case CreditStatus.Overdue:
        return this.translate.instant('CreditStatus.Overdue');
      default:
        return '';
    }
  }

}
