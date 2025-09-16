import { ChangeDetectorRef, Component, ElementRef, inject, signal, ViewChild } from '@angular/core';
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
  updateMode = signal<boolean>(false);
  updatedAvatarFile :File |null = null;
  updatedAvatarSrc : string | null = null;
  validationErrors: string[] =[];


  constructor(private cd: ChangeDetectorRef, private routeId: ActivatedRoute) {
    
  }

  ngOnInit(){
    this.profileService.getOwnProfile(false).subscribe({
      next:(val)=>{
        this.profile=val;
        if(this.profile.role===null){
          this.profile.role ="User";
        }
      },
      error: (err)=>{
        this.profile=null;
      }, 
      complete:()=>{
        this.cd.detectChanges();
      }
    })
  }

  toUpdateMode(){
    this.updateMode.set(true);
  }

  cancelChanges(){
    this.updateMode.set(false);
  }

  uploadPhoto(event :Event){
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;
    if (!file) return;
    if (!file.type.startsWith('image/')) return;

    this.updatedAvatarFile = file;
    this.updatedAvatarSrc = URL.createObjectURL(file);
    this.cd.detectChanges();
    input.value = '';
  }
}
