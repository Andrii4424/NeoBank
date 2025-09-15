import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { ProfileService } from '../../../data/services/auth/profile-service';
import { ActivatedRoute, Router } from '@angular/router';
import { IProfile } from '../../../data/interfaces/auth/profile-interface';

@Component({
  selector: 'app-users-own-profile',
  imports: [],
  templateUrl: './users-own-profile.html',
  styleUrl: './users-own-profile.scss'
})
export class UsersOwnProfile {
  profileService = inject(ProfileService);
  router = inject(Router);
  profile! :IProfile |null;

  constructor(private cd: ChangeDetectorRef, private routeId: ActivatedRoute) {
    
  }

  ngOnInit(){
    this.profileService.getOwnProfile(false).subscribe({
      next:(val)=>{
        this.profile=val;
      },
      error: (err)=>{
        this.profile=null;
      }, 
      complete:()=>{
        this.cd.detectChanges();
      }
    })
  }
}
