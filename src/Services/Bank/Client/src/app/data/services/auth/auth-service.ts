import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { tap } from 'rxjs';
import { CookieService } from 'ngx-cookie-service';
import { IAccessToken } from '../../interfaces/auth/access-token.interface';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  http = inject(HttpClient)
  cookieService = inject(CookieService)
  
  baseUrl = "https://localhost:7280/api/Account/";

  accessToken :string | null = null;
  expiresOn :Date | null = null;

  IsLoggedIn(){
    this.accessToken = this.cookieService.get("accessToken");
    if(this.accessToken!==null && this.accessToken!==""){// && this.expiresOn!==null && this.expiresOn.getTime()>Date.now()){
      console.log(this.accessToken);
      return true;
    }
    return false;
  }

  login(payload: {email:string, password:string }){
    const fd = new FormData();

    fd.append("Email", payload.email);
    fd.append("Password", payload.password)

    return this.http.post<IAccessToken>(
      `${this.baseUrl}Login`,
      fd
    ).pipe(
      tap(val=>{
        this.cookieService.set("accessToken", val.accessToken);
        this.cookieService.set("accessTokenExpires", new Date(val.expirationTime).toISOString())
      })
    )
  }

}
