import { IProfile } from './../../interfaces/auth/profile-interface';
import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})

export class ProfileService {
  http = inject(HttpClient);
  
  baseUrl ='https://localhost:7280/api/Profile/';

  getOwnProfile(){
    return this.http.get<IProfile>(`${this.baseUrl}Me`);
  }
}
