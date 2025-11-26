import { Component } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { CreditsLayout } from "../credits-layout/credits-layout";

@Component({
  selector: 'app-credit-tariffs',
  imports: [TranslateModule, CreditsLayout],
  templateUrl: './credit-tariffs.html',
  styleUrl: './credit-tariffs.scss'
})
export class CreditTariffs {

}
