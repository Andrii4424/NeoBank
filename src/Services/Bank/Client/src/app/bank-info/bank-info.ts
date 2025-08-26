import { CommonModule } from '@angular/common';
import { BankService } from './../data/services/bank-service';
import { Component, inject } from '@angular/core';

@Component({
  selector: 'bank-info',
  imports: [CommonModule],
  templateUrl: './bank-info.html',
  styleUrl: './bank-info.scss'
})
export class BankInfo {
  bankService = inject(BankService);
  bankInfo : any= null;
  constructor(){
    this.bankService.getBankInfo().subscribe(val=>{
      this.bankInfo=val;
    })
  }
}
