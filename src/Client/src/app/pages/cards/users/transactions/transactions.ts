import { TransactionService } from './../../../../data/services/bank/bank-products/transaction-service';
import { Component, inject, HostListener } from '@angular/core';
import { TransactionsFilters } from "../transactions-filters/transactions-filters";

@Component({
  selector: 'app-transactions',
  imports: [TransactionsFilters],
  templateUrl: './transactions.html',
  styleUrl: './transactions.scss'
})
export class Transactions {
  transactionService = inject(TransactionService);

}
