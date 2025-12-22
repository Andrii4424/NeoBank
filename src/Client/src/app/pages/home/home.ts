import { Component } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { News } from '../news/news/news';
import { RouterLink } from "@angular/router";

@Component({
  selector: 'app-home',
  imports: [TranslateModule, News, RouterLink],
  templateUrl: './home.html',
  styleUrl: './home.scss'
})
export class Home {

}
