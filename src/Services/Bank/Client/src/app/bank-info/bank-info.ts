import { CommonModule } from '@angular/common';
import { BankService } from './../data/services/bank-service';
import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { bank } from '../data/interfaces/bank/bank.interface';

@Component({
  selector: 'bank-info',
  imports: [CommonModule],
  templateUrl: './bank-info.html',
  styleUrl: './bank-info.scss'
})
export class BankInfo {
  bankService = inject(BankService);
  bankInfo!: bank;
  private cdr = inject(ChangeDetectorRef);
  constructor(){
    this.bankService.getBankInfo().subscribe(val=>{
      this.bankInfo=val;
      this.cdr.detectChanges();
      console.log(this.bankInfo);
    })
  }
}
