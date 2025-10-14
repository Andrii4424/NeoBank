import { ProfileService } from './../../../../data/services/auth/profile-service';
import { AuthService } from './../../../../data/services/auth/auth-service';
import { Component, inject, signal } from '@angular/core';
import { BankService } from '../../../../data/services/bank/bank-service';
import { IBank } from '../../../../data/interfaces/bank/bank.interface';
import { AsyncPipe } from '@angular/common';
import { Observable } from 'rxjs';
import { RouterLink } from '@angular/router';
import { Loading } from "../../../../common-ui/loading/loading";

@Component({
  selector: 'app-bank-info',
  imports: [AsyncPipe, RouterLink, Loading],
  templateUrl: './bank-info.html',
  styleUrl: './bank-info.scss'
})
export class BankInfo {
  profileService = inject (ProfileService)
  bankService = inject(BankService);
  bank$ : Observable<IBank>;
  isLoading = signal<boolean>(true);

  constructor (){
    this.bank$ = this.bankService.getBank();
  }
}
