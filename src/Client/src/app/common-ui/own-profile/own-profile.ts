import { ChangeDetectorRef, Component, inject, signal } from '@angular/core';
import { ProfileService } from '../../data/services/auth/profile-service';
import { IProfile } from '../../data/interfaces/auth/profile-interface';
import { RouterLink } from '@angular/router';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-own-profile',
  imports: [RouterLink],
  templateUrl: './own-profile.html',
  styleUrl: './own-profile.scss'
})
export class OwnProfile {
  profileService = inject(ProfileService);
  baseUrl = `${environment.apiPhotoUrl}/`;
  showLogButtons = signal<boolean>(false);
  
  constructor(private cd: ChangeDetectorRef) {}

  ngOnInit(){
    this.profileService.getOwnProfile(true).subscribe({
      next:(val)=>{
        this.profileService.updateProfileSignal(val);
        this.showLogButtons.set(true);
      },
      error: (err)=>{
        this.profileService.updateProfileSignal(undefined);
        this.showLogButtons.set(true);
        this.cd.detectChanges();
      }, 
      complete:()=>{
        this.cd.detectChanges();
      }
    })
  }
}
