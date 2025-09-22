import { CardLevel } from './../../../data/enums/card-level';
import { IFilter } from '../../../data/filters/filter-interface';
import { Component } from '@angular/core';
import { RouterModule } from "@angular/router";
import { Search } from "../../../common-ui/search/search";
import { ISort } from '../../../data/filters/sort-interface';
import { Currency } from '../../../data/enums/currency';
import { PaymentSystem } from '../../../data/enums/payment-system';
import { CardType } from '../../../data/enums/card-type';

@Component({
  selector: 'app-cards-layout',
  imports: [RouterModule, Search],
  templateUrl: './cards-layout.html',
  styleUrl: './cards-layout.scss'
})
export class CardsLayout {
  sortValues: ISort[]=[
      {name: "name-descending", description: "By Name Descending ▼"},
      {name: "name-ascending", description: "By Name Ascending ▲"},
      {name: "annual-maintenance-cost", description: "By Annual Maintenance Cost"},
      {name: "validity-period", description: "By Validity Period"}
  ];
  
  searchPlaceholder: string ="Enter the card name";
  
  filterValues: IFilter[]=[
    {id: "premium", description: "Premium", value: CardLevel.Premium, chosen: false },
    {id: "usual", description: "Usual", value: CardLevel.Normal, chosen: false },
    {id: "mastercard", description: "Mastercard", value: PaymentSystem.Mastercard, chosen: false },
    {id: "visa", description: "Visa",value: PaymentSystem.Visa, chosen: false},
    {id: "credit", description: "Credit", value: CardType.Credit, chosen: false },
    {id: "debit", description: "Debit", value: CardType.Debit, chosen: false },
    {id: "uah", description: "UAH", value: Currency.UAH, chosen: false },
    {id: "usd", description: "USD", value: Currency.USD, chosen: false },
    {id: "eur", description: "EUR", value: Currency.EUR, chosen: false },
  ]
}
