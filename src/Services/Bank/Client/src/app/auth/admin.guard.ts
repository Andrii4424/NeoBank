import { ProfileService } from './../data/services/auth/profile-service';
import { inject } from "@angular/core";
import { CanActivateFn, Router } from "@angular/router";
import { AuthService } from "../data/services/auth/auth-service";
import { catchError, map, of, tap } from "rxjs";

export const adminGuard: CanActivateFn =()=>{
    const profileService = inject(ProfileService);
    const router = inject(Router);
    
    if(profileService.role()?.toLowerCase()==='admin'){
        return true;
    }
    router.navigate(['/']);
    return false;
}