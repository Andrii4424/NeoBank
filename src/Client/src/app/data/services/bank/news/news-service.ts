import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../../../environments/environment';
import { INews } from '../../../interfaces/news/news-interface';
import { IPageResult } from '../../../interfaces/page-inteface';
import { Params } from '@angular/router';
import { IAddNews } from '../../../interfaces/news/add-news-interface';

@Injectable({
  providedIn: 'root'
})
export class NewsService {
  http = inject(HttpClient);
  baseUrl =`${environment.apiUrl}/News`;

  getNews(params: Params) {
    return this.http.get<IPageResult<INews>>(`${this.baseUrl}`, params);
  }

  getNewsById(id: string) {
    return this.http.get<INews>(`${this.baseUrl}/${id}`);
  }

  addNews(news: IAddNews){
    return this.http.post(`${this.baseUrl}`, this.AddNewsToFormData(news));
  }

  updateNews(news: IAddNews){
    return this.http.put(`${this.baseUrl}`, this.AddNewsToFormData(news));
  }

  deleteNews(id: string){
    return this.http.delete(`${this.baseUrl}/id`,);
  }

  private AddNewsToFormData(news: IAddNews): FormData {
    const fd = new FormData();

    fd.append('Id', news.id ?? '');
    fd.append('Title', news.title ?? '');
    fd.append('Topic', news.topic ?? '');
    fd.append('Author', news.author ?? '');
    fd.append('Content', news.content ?? '');
    fd.append('CreatedAt', news.createdAt ?? '');
    fd.append('Image', news.image!);

    return fd;
  }
  
}
