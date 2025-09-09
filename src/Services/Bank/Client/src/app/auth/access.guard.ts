import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../data/services/auth/auth-service';
export const CanActivateAuth =() =>{
    const authService = inject(AuthService);
    if(authService.IsLoggedIn()){
        return true;
    }

    return inject(Router).createUrlTree(['/login'])
}