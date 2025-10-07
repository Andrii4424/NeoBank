
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ChangeDetectorRef, Component, inject, signal } from '@angular/core';
import { Observable } from 'rxjs';
import { AsyncPipe } from '@angular/common';
import { CardTariffsService } from '../../../../data/services/bank/bank-products/card-tariffs-service';
import { ProfileService } from '../../../../data/services/auth/profile-service';
import { CardType } from '../../../../data/enums/card-type';
import { CardLevel } from '../../../../data/enums/card-level';
import { PaymentSystem } from '../../../../data/enums/payment-system';
import { Currency } from '../../../../data/enums/currency';
import { ConfirmDelete } from "../../../../common-ui/confirm-delete/confirm-delete";
import { ICardTariffs } from '../../../../data/interfaces/bank/bank-products/cards/card-tariffs.interface';

@Component({
  selector: 'app-card-tariffs-info',
  imports: [AsyncPipe, RouterLink, ConfirmDelete],
  templateUrl: './card-tariffs-info.html',
  styleUrl: './card-tariffs-info.scss'
})
export class CardTariffsInfo {
  cardTariffsService = inject(CardTariffsService);
  route = inject(ActivatedRoute);
  card$:Observable<ICardTariffs>;
  showDeleteDialog = signal<boolean>(false);
  profileService = inject(ProfileService);
  id : string = this.route.snapshot.paramMap.get('id')!;

  //enums
  CardType = CardType;
  CardLevel = CardLevel;
  PaymentSystem = PaymentSystem;
  Currency = Currency;

  constructor() {
    this.card$ = this.cardTariffsService.getCardTariffsInfo(this.id);
  }

  onCancel(){
    this.showDeleteDialog.set(false);
  }

  onDelete(){
    this.cardTariffsService.deleteCardTariffs(this.id).subscribe({
      next:()=>{
        this.showDeleteDialog.set(false);
        window.history.back();
      },
      error:(err)=>{
        this.showDeleteDialog.set(false);
      }
    });
  }
}
