import { Component, inject } from '@angular/core';
import { BankService } from '../../../../data/services/bank/bank-service';

@Component({
  selector: 'app-update-bank',
  imports: [],
  templateUrl: './update-bank.html',
  styleUrl: './update-bank.scss'
})
export class UpdateBank {
  bankservice= inject(BankService)

  testRequest(){
    this.bankservice.testRequest().subscribe(val=>{
      console.log("Test request has been send!")
    })
  }
}
