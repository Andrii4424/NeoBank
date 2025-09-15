import { IProfile } from './../../interfaces/auth/profile-interface';
import { HttpClient, HttpContext } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { SKIP_LOGIN_REDIRECT } from '../../../auth/skip-login-redirect.token';
import { F } from '@angular/cdk/keycodes';

@Injectable({
  providedIn: 'root'
})

export class ProfileService {
  http = inject(HttpClient);
  
  baseUrl ='https://localhost:7280/api/Profile/';

  getOwnProfile(skipRedirect: boolean){
    return this.http.get<IProfile>(`${this.baseUrl}Me`, {
      context: new HttpContext()
        .set(SKIP_LOGIN_REDIRECT, skipRedirect)
    });
  }


  getUserProfile(userId:string){
    return this.http.get<IProfile>(`${this.baseUrl}ProfileInfo/${userId}`);
  }
}
