import { CreditStatus } from './../../../../../data/enums/credit-status';
import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { UserCredit } from '../../../../../data/interfaces/bank/users/user-credits.models.';
import { DatePipe } from '@angular/common';
import { Currency } from '../../../../../data/enums/currency';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-credits-details-modal',
  imports: [DatePipe, TranslateModule],
  templateUrl: './credits-details-modal.html',
  styleUrl: './credits-details-modal.scss'
})
export class CreditsDetailsModal {
  translate = inject(TranslateService);
  @Input() credit!: UserCredit;
  @Output() closeModal = new EventEmitter<void>();

  CreditStatus = CreditStatus;

  close() {
    this.closeModal.emit();
  }

      getCurrency(currency: Currency): string {
          switch (currency) {
            case Currency.UAH:
              return 'UAH';
            case Currency.USD:
              return 'USD';
            case Currency.EUR:
              return 'EUR';
            default:
              return '';
          }
      }

  getCreditStatusText(status: CreditStatus){
    switch (status) {
      case CreditStatus.Active:
        return this.translate.instant('CreditStatus.Active');
      case CreditStatus.Closed:
        return this.translate.instant('CreditStatus.Closed');
      case CreditStatus.Rejected:
        return this.translate.instant('CreditStatus.Rejected');
      case CreditStatus.Overdue:
        return this.translate.instant('CreditStatus.Overdue');
      default:
        return '';
    }
  }
  
}
