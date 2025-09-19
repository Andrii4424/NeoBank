import { IProfile } from './../../interfaces/auth/profile-interface';
import { HttpClient, HttpContext } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { SKIP_LOGIN_REDIRECT } from '../../../auth/skip-login-redirect.token';
import { F } from '@angular/cdk/keycodes';
import { IRole } from '../../interfaces/auth/role-interface';
import { tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})

export class ProfileService {
  http = inject(HttpClient);
  profileSignal = signal<IProfile | null  | undefined>(null);
  baseUrl ='https://localhost:7280/api/Profile/';

  getOwnProfile(skipRedirect: boolean){
    return this.http.get<IProfile>(`${this.baseUrl}Me`, {
      context: new HttpContext()
        .set(SKIP_LOGIN_REDIRECT, skipRedirect)
    });
  }

  constructor(){
    this.updateRole();
  }

  getUserProfile(userId:string){
    return this.http.get<IProfile>(`${this.baseUrl}ProfileInfo/${userId}`);
  }

  updateUser(payload:{id: string, email: string | null, firstName: string| null, surname: string | null, patronymic: string | null, dateOfBirth: string | null, 
    taxId: string | null, avatarPath: string | null, role: string | null, isVerified: boolean | null, phoneNumber: string | null, avatar: File | null}){
      const fd = new FormData();
      fd.append("Id", payload.id);
      Object.entries(payload).forEach(([key, value]) => {
      if (key === 'id' || key === 'avatar') return;

      if (value !== null && value !== undefined) {
        fd.append(this.capitalizeFirstLetter(key), value as string);
      }
    });
    if (payload.avatar) {
      fd.append("Avatar", payload.avatar);
    }
    return this.http.post<IProfile>(`${this.baseUrl}UpdateOwnProfile`, fd);    
  }

  updateProfileSignal(profile : IProfile |null |undefined){
    this.profileSignal.set(profile);
  }

  private capitalizeFirstLetter(str: string) {
    return str.charAt(0).toUpperCase() + str.slice(1);
  }

  updateRole(){
    this.http.get<IRole>(`https://localhost:7280/api/Account/GetRole`,{
      context: new HttpContext()
        .set(SKIP_LOGIN_REDIRECT, true)
    })
    .pipe(
      tap({
        next:(val)=>{
          localStorage.setItem( "role", val.role ?? "");
        },
        error:()=>{
          localStorage.setItem("role", "");
        }
      })
    ).subscribe();
  }

  getRole(){
    return localStorage.getItem("role");
  }
}

