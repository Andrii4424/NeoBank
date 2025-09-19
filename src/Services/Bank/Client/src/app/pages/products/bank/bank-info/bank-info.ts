import { Component, inject } from '@angular/core';
import { BankService } from '../../../../data/services/bank/bank-service';
import { IBank } from '../../../../data/interfaces/bank/bank.interface';
import { AsyncPipe } from '@angular/common';
import { Observable } from 'rxjs';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-bank-info',
  imports: [AsyncPipe, RouterLink],
  templateUrl: './bank-info.html',
  styleUrl: './bank-info.scss'
})
export class BankInfo {
  bankService = inject(BankService);
  bank$ : Observable<IBank>;

  constructor (){
    this.bank$ = this.bankService.getBank();
  }
}
