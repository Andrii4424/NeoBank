import { Component } from '@angular/core';
import { RouterModule } from "@angular/router";
import { TranslateModule } from '@ngx-translate/core';


@Component({
  selector: 'app-cards-layout',
  imports: [RouterModule, TranslateModule],
  templateUrl: './cards-layout.html',
  styleUrl: './cards-layout.scss'
})
export class CardsLayout {

}
