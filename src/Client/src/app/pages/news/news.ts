import { NewsService } from './../../data/services/bank/news/news-service';
import { ProfileService } from './../../data/services/auth/profile-service';
import { Component, inject } from '@angular/core';
import { Observable, Subscription } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { IPageResult } from '../../data/interfaces/page-inteface';
import { INews } from '../../data/interfaces/news/news-interface';
import { AsyncPipe } from '@angular/common';

@Component({
  selector: 'app-news',
  imports: [AsyncPipe],
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


  ngOnInit() {
    this.querySub = this.route.queryParams.subscribe(params => {
      this.newsPage$ = this.newsService.getNews(params);
    });
  }

}
