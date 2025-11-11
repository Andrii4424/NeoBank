import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../data/services/auth/auth-service';
export const CanActivateAuth =() =>{
    const authService = inject(AuthService);
    const router = inject(Router);
    if(authService.IsLoggedIn()){
        return true;
    }
    
    return router.createUrlTree(['/auth/login'], {
        queryParams: { error: 'sessionExpired' },
    });
}