import { inject } from '@angular/core';
import { AuthService } from './../services/auth/auth-service';
import { Router } from '@angular/router';
export const CanActivateAuth =() =>{
    const authService = inject(AuthService);
    if(authService.IsLoggedIn()){
        return true;
    }

    return inject(Router).createUrlTree(['/login'])
}