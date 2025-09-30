
import { Component, inject } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { Observable } from 'rxjs';
import { AsyncPipe } from '@angular/common';
import { CardsLayout } from '../../cards-layout/cards-layout';
import { Search } from '../../../../common-ui/search/search';
import { PaymentSystem } from '../../../../data/enums/payment-system';
import { Currency } from '../../../../data/enums/currency';
import { CardType } from '../../../../data/enums/card-type';
import { CardLevel } from '../../../../data/enums/card-level';
import { CardStatus } from '../../../../data/enums/card-status';
import { UserCardsService } from '../../../../data/services/bank/bank-products/user-cards-service';
import { IUserCards } from '../../../../data/interfaces/bank/bank-products/cards/user-cards-interface';


@Component({
  selector: 'app-user-cards',
  imports: [AsyncPipe, CardsLayout, Search, RouterLink],
  templateUrl: './user-cards.html',
  styleUrl: './user-cards.scss'
})
export class UserCards {
  userCardsService = inject(UserCardsService);
  userCards$: Observable<IUserCards[]> = this.userCardsService.getMyCards();

  PaymentSystem = PaymentSystem;
  Currency = Currency;
  CardType = CardType;
  CardStatus = CardStatus;
  CardLevel = CardLevel;
}
