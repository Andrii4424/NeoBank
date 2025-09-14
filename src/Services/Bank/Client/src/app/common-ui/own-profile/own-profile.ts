import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { ProfileService } from '../../data/services/auth/profile-service';
import { IProfile } from '../../data/interfaces/auth/profile-interface';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-own-profile',
  imports: [RouterLink],
  templateUrl: './own-profile.html',
  styleUrl: './own-profile.scss'
})
export class OwnProfile {
  profileService = inject(ProfileService);
  profile?: IProfile;
  
  constructor(private cd: ChangeDetectorRef) {}


  ngOnInit(){
    this.profileService.getOwnProfile().subscribe({
      next:(val)=>{
        this.profile=val;
      },
      error: (err)=>{
        this.profile=undefined;
      }, 
      complete:()=>{
        this.cd.detectChanges();
      }
    })
  }
}
