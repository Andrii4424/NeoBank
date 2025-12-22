import { ProfileService } from './../../../data/services/auth/profile-service';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { NewsService } from './../../../data/services/bank/news/news-service';
import { Component, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { INews } from '../../../data/interfaces/news/news-interface';
import { AsyncPipe } from '@angular/common';
import { Loading } from "../../../common-ui/loading/loading";
import { TranslateModule } from '@ngx-translate/core';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-news-info',
  imports: [AsyncPipe, Loading, TranslateModule, RouterLink],
  templateUrl: './news-info.html',
  styleUrl: './news-info.scss'
})
export class NewsInfo {
  private newsService = inject(NewsService);
  private route = inject(ActivatedRoute);
  newsInfo$!: Observable<INews>;
  profileService = inject(ProfileService);
  baseUrl = `${environment.apiPhotoUrl}/`;

  ngOnInit() {
    this.newsInfo$= this.newsService.getNewsById(this.route.snapshot.paramMap.get('id')!);
  }
}
