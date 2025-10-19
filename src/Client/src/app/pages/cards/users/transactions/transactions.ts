import { TransactionStatus } from './../../../../data/enums/transaction-status';
import { TransactionType } from './../../../../data/enums/transaction-type';
import { ITransactionFilter } from '../../../../data/interfaces/filters/transaction-filters';
import { IPageResult } from './../../../../data/interfaces/page-inteface';
import { TransactionService } from './../../../../data/services/bank/bank-products/transaction-service';
import { Component, inject, HostListener, Input, ChangeDetectorRef, signal } from '@angular/core';
import { TransactionsFilters } from "../transactions-filters/transactions-filters";
import { ActivatedRoute, Params, Router } from '@angular/router';
import { ITransaction } from '../../../../data/interfaces/bank/bank-products/cards/transaction-interface';
import { DatePipe } from '@angular/common';
import { PageSwitcher } from "../../../../common-ui/page-switcher/page-switcher";
import { Subscription } from 'rxjs';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-transactions',
  imports: [TransactionsFilters, DatePipe, PageSwitcher, TranslateModule],
  templateUrl: './transactions.html',
  styleUrl: './transactions.scss'
})
export class Transactions {
  transactionService = inject(TransactionService);
  route = inject(ActivatedRoute);
  router = inject(Router);
  transactionsPage: IPageResult<ITransaction> | null = null;
  cardId = this.route.snapshot.paramMap.get("id");
  selectedSortValue: string | null = null;
  minSumValue: number | null = null;
  dateValue: string | null = null;
  pageNumber: number | null = null;
  @Input() cardCurrencyText : string | null= null;
  @Input() cardCurrencySymbol : string | null= null;
  private queryParamsSub?: Subscription;
  isLoaded = signal<boolean>(false);

  TransactionStatus = TransactionStatus;

  constructor(private cdr: ChangeDetectorRef){}

  ngOnInit(){
    this.queryParamsSub = this.route.queryParams.subscribe(params => {
      this.updatePage(params);
    });
  }

  submitFilters(filters: ITransactionFilter){
    const queryParams: Params  = {};
    queryParams['MinimalTransactionSum'] = filters.minimalSum == null? "": filters.minimalSum;
    queryParams['ChosenDate'] = filters.transactionsDate == null? "": filters.transactionsDate;
    queryParams['SortValue'] = filters.sortValue == null? "": filters.sortValue;
    queryParams['PageNumber'] = 1;

    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: queryParams,
      queryParamsHandling: 'merge'
    })
  }

  changePage(pageNumber: number){
    this.pageNumber = this.route.snapshot.queryParams["PageNumber"];
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: {PageNumber: pageNumber},
      queryParamsHandling: 'merge'
    })
  }

  refreshPage(){
    setTimeout(() => this.updatePage(this.route.snapshot.queryParams), 800);
  }

  updatePage(queryParams: Params){
    this.transactionService.getTransactions({cardId: this.cardId!, params: queryParams}).subscribe({
      next:(val)=>{
        this.transactionsPage = val;
        this.isLoaded.set(true);
        this.cdr.detectChanges();
      },
      error:(val)=>{
        this.isLoaded.set(true);
      }
    })
  }

  hasQuery(){
    return Object.keys(this.route.snapshot.queryParams).length > 0;
  }

  getTransactionTypeText(type: TransactionType){
    switch(type){
      case(TransactionType.P2P):{
        return "P2P";
      }
      case(TransactionType.Replenishment):{
        return "Replenishment";
      }
      case(TransactionType.CurrencyExchange):{
        return "Currency Exchange";
      }
      case(TransactionType.Credit):{
        return "Credit";
      }
      case(TransactionType.Deposit):{
        return "Deposit";
      }
      case(TransactionType.AmnualCardMatinance):{
        return "Amnual Card Matinance";
      }
      default:{
        return "";
      }
    }
  }

  getTransactionStatusText(status: TransactionStatus){
    switch(status){
      case(TransactionStatus.Completed):{
        return "Completed";
      }
      case(TransactionStatus.Failed):{
        return "Failed";
      }
      case(TransactionStatus.Pending):{
        return "Pending";
      }
      default:{
        return "Unknown";
      }
    }
  }
}
