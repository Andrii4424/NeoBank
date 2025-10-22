import { Observable } from 'rxjs';
import { ProfileService } from './../../../../data/services/auth/profile-service';
import { Component, inject } from '@angular/core';
import { IProfile } from '../../../../data/interfaces/auth/profile-interface';
import { ICroppedProfile } from '../../../../data/interfaces/auth/cropped-profile.interface';
import { ActivatedRoute } from '@angular/router';
import { AsyncPipe } from '@angular/common';
import { Loading } from "../../../../common-ui/loading/loading";

@Component({
  selector: 'app-user-profile',
  imports: [AsyncPipe, Loading],
  templateUrl: './user-profile.html',
  styleUrl: './user-profile.scss'
})
export class UserProfile {
  profileService = inject(ProfileService);
  isAdmin: boolean;
  route = inject(ActivatedRoute);
  userId: string;
  $userFullInfo:Observable<IProfile>| null = null;
  $userCroppedInfo:Observable<ICroppedProfile>| null = null;
  baseUrl ="https://localhost:7280/";
  
  constructor(){
    this.isAdmin = this.profileService.getRole() ==="Admin";
    this.userId = this.route.snapshot.paramMap.get('id')!;
  }

  ngOnInit(){
    if(this.isAdmin){
      this.$userFullInfo = this.profileService.getUserFullProfile(this.userId);
    }
    else{
      this.$userCroppedInfo = this.profileService.getUserCroppedProfile(this.userId);
    }
  }



}
