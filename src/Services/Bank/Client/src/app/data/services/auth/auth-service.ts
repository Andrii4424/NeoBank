import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { catchError, tap, throwError } from 'rxjs';
import { CookieService } from 'ngx-cookie-service';
import { IAccessToken } from '../../interfaces/auth/access-token.interface';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  http = inject(HttpClient);
  router = inject(Router)
  cookieService = inject(CookieService);
  
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
        this.cookieService.set("accessTokenExpires", new Date(val.expirationTime).toISOString());
        
        this.accessToken = val.accessToken;
        this.expiresOn = new Date(val.expirationTime);
      })
    )
  }

  refresh(){
    return this.http.post<IAccessToken>(`${this.baseUrl}Refresh`, null, {withCredentials: true,})
    .pipe(
      catchError(err =>{
        this.cookieService.delete("accessToken");
        this.cookieService.delete("accessTokenExpires");

        this.accessToken=null;
        this.expiresOn=null;
        
        this.router.navigate(['/login'], {queryParams: {error: "sessionExpired"}})
        return throwError(() => err);
      }),
      tap(val=>{
          this.accessToken = val.accessToken;
          this.expiresOn = new Date(val.expirationTime);

          this.cookieService.set("accessToken", val.accessToken);
          this.cookieService.set("accessTokenExpires", new Date(val.expirationTime).toISOString())
      })
    )
  }

  register(payload:{email: string |null, password:string |null, confirmPassword:string |null}){
    const fd = new FormData();
    fd.append("Email", payload.email!);
    fd.append("Password", payload.password!);
    fd.append("ConfirmPassword", payload.confirmPassword!);

    return this.http.post<IAccessToken>(`${this.baseUrl}Register`, fd).pipe(
      tap({
        next: (res=>{
          this.cookieService.set("accessToken", res.accessToken);
          this.cookieService.set("accessTokenExpires", new Date(res.expirationTime).toISOString());
          
          this.accessToken = res.accessToken;
          this.expiresOn = new Date(res.expirationTime);
        })
      })
    );
  }
}
