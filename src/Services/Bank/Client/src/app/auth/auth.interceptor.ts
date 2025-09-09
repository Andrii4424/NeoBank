import { inject } from '@angular/core';
import { HttpInterceptor, HttpInterceptorFn } from "@angular/common/http";
import { AuthService } from '../data/services/auth/auth-service';

export const authTokenInterceptor : HttpInterceptorFn =(req, next) =>{
    const authService = inject(AuthService);

    req = req.clone({
        setHeaders:{
            Authotization: `Bearer ${authService.accessToken}`
        }
    })


    return next(req);
}