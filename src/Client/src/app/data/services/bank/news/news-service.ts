import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../../../environments/environment';
import { INews } from '../../../interfaces/news/news-interface';
import { IPageResult } from '../../../interfaces/page-inteface';
import { Params } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class NewsService {
  http = inject(HttpClient);
  baseUrl =`${environment.apiUrl}/News`;

  getNews(params: Params) {
    return this.http.get<IPageResult<INews>>(`${this.baseUrl}`, params);
  }

  addNews(news: INews){
    return this.http.post(`${this.baseUrl}`, news);
  }

  updateNews(news: INews){
    return this.http.put(`${this.baseUrl}`, news);
  }

  deleteNews(id: string){
    return this.http.delete(`${this.baseUrl}/id`,);
  }
  
}
