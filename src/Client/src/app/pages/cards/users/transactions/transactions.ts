import { TransactionStatus } from './../../../../data/enums/transaction-status';
import { TransactionType } from './../../../../data/enums/transaction-type';
import { ITransactionFilter } from '../../../../data/interfaces/filters/transaction-filters';
import { IPageResult } from './../../../../data/interfaces/page-inteface';
import { TransactionService } from './../../../../data/services/bank/bank-products/transaction-service';
import { Component, inject, HostListener, Input, ChangeDetectorRef } from '@angular/core';
import { TransactionsFilters } from "../transactions-filters/transactions-filters";
import { ActivatedRoute, Router } from '@angular/router';
import { ITransaction } from '../../../../data/interfaces/bank/bank-products/cards/transaction-interface';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-transactions',
  imports: [TransactionsFilters, DatePipe],
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

  TransactionStatus = TransactionStatus;

  constructor(private cdr: ChangeDetectorRef){}

  ngOnInit(){
    this.selectedSortValue = this.route.snapshot.queryParams["SortValue"];
    this.minSumValue = this.route.snapshot.queryParams["MinimalTransactionSum"];
    this.dateValue = this.route.snapshot.queryParams["ChosenDate"];
    this.pageNumber = this.route.snapshot.queryParams["PageNumber"];
    this.transactionService.getTransactions({cardId: this.cardId!, params: this.route.snapshot.queryParams}).subscribe({
      next:(val)=>{
        this.transactionsPage = val;
        console.log(val);
        this.cdr.detectChanges();
      },
      error:(val)=>{

      }
    })
  }

  submitFilters(filters: ITransactionFilter){
    console.log(filters);
    /*if(filters.minimalSum != null){
      console.log("1");
      console.log(filters.minimalSum);
      this.router.navigate([],{
        relativeTo: this.route,
        queryParams: {MinimalTransactionSum : filters.minimalSum, PageNumber: 1},
        queryParamsHandling: 'merge'
      });
    }
    
    if(filters.transactionsDate != null){
      console.log("2");
      this.router.navigate([],{
        relativeTo: this.route,
        queryParams: {ChosenDate : filters.transactionsDate, PageNumber: 1},
        queryParamsHandling: 'merge'
      });
    }*/

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
