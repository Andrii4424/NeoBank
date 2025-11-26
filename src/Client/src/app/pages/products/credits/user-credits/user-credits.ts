import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { CreditsLayout } from "../credits-layout/credits-layout";

@Component({
  selector: 'app-user-credits',
  imports: [TranslateModule, RouterModule, CreditsLayout],
  templateUrl: './user-credits.html',
  styleUrl: './user-credits.scss'
})
export class UserCredits {

}
