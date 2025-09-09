import { Login } from './../../pages/auth/login/login';
import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  http = inject(HttpClient)
  
  baseUrl = "https://localhost:7280/api/Account/";

  login(payload: {email:string, password:string }){
    const fd = new FormData();

    fd.append("Email", payload.email);
    fd.append("Password", payload.password)

    return this.http.post(
      `${this.baseUrl}Login`,
      fd
    )

  }

}
