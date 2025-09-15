import { SKIP_LOGIN_REDIRECT } from './../../../auth/skip-login-redirect.token';
import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { catchError, finalize, map, Observable, shareReplay, tap, throwError } from 'rxjs';
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
  private refresh$?: Observable<string>;

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

  refresh(skipLoginRedirect: boolean): Observable<string> {
    if (!this.refresh$) {
      this.refresh$ = this.http
        .post<IAccessToken>(`${this.baseUrl}Refresh`, null, { withCredentials: true })
        .pipe(
          map(val => {
            this.accessToken = val.accessToken;
            this.expiresOn = new Date(val.expirationTime);

            this.cookieService.set('accessToken', val.accessToken);
            this.cookieService.set('accessTokenExpires', new Date(val.expirationTime).toISOString());

            return val.accessToken;
          }),
          shareReplay(1),
          catchError(err => {
            this.cookieService.delete('accessToken');
            this.cookieService.delete('accessTokenExpires');
            this.accessToken = null;
            this.expiresOn = null;

            if (!skipLoginRedirect) {
              this.router.navigate(['/login'], { queryParams: { error: 'sessionExpired' } });
            }
            this.refresh$ = undefined;
            return throwError(() => err);
          }),
          finalize(() => {
            this.refresh$ = undefined;
          })
        );
    }
    return this.refresh$;
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

  getAccessToken():string |null{
    return this.cookieService.get("accessToken");
  }
}
