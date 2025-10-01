import { CardType } from './../../../../data/enums/card-type';
import { Currency } from './../../../../data/enums/currency';
import { CardStatus } from './../../../../data/enums/card-status';
import { PaymentSystem } from './../../../../data/enums/payment-system';
import { Observable } from 'rxjs';
import { UserCardsService } from './../../../../data/services/bank/bank-products/user-cards-service';
import { Component, inject } from '@angular/core';
import { IUserCards } from '../../../../data/interfaces/bank/bank-products/cards/user-cards-interface';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { AsyncPipe } from '@angular/common';

@Component({
  selector: 'app-user-card-info',
  imports: [AsyncPipe, RouterLink],
  templateUrl: './user-card-info.html',
  styleUrl: './user-card-info.scss'
})
export class UserCardInfo {
  userCardsService = inject(UserCardsService);
  route = inject(ActivatedRoute);
  userCard$: Observable<IUserCards> = this.userCardsService.getCardInfo(this.route.snapshot.paramMap.get("id")!);
  
  //Enums
  PaymentSystem = PaymentSystem;
  CardStatus = CardStatus;
  Currency = Currency;
  CardType = CardType;


}
