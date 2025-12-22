import { Component, inject } from '@angular/core';
import { Observable, Subscription } from 'rxjs';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AsyncPipe } from '@angular/common';
import { ProfileService } from '../../../data/services/auth/profile-service';
import { NewsService } from '../../../data/services/bank/news/news-service';
import { IPageResult } from '../../../data/interfaces/page-inteface';
import { INews } from '../../../data/interfaces/news/news-interface';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-news',
  imports: [AsyncPipe, RouterLink],
  templateUrl: './news.html',
  styleUrl: './news.scss'
})
export class News {
  profileService = inject(ProfileService);
  newsService = inject(NewsService);
  route = inject(ActivatedRoute);
  router = inject(Router);
  private querySub!: Subscription;
  newsPage$!: Observable<IPageResult<INews>>;
  baseUrl = `${environment.apiPhotoUrl}/`;
  
  ngOnInit() {
    this.querySub = this.route.queryParams.subscribe(params => {
      this.newsPage$ = this.newsService.getNews(params);
    });
  }

}
