import { Component } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { News } from '../news/news/news';

@Component({
  selector: 'app-home',
  imports: [TranslateModule, News],
  templateUrl: './home.html',
  styleUrl: './home.scss'
})
export class Home {

}
