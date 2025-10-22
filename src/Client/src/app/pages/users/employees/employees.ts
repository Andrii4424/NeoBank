import { ProfileService } from './../../../data/services/auth/profile-service';
import { Component, inject } from '@angular/core';
import { Search } from "../../../common-ui/search/search";
import { Observable, Subscription } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { IProfile } from '../../../data/interfaces/auth/profile-interface';
import { IPageResult } from '../../../data/interfaces/page-inteface';
import { AsyncPipe } from '@angular/common';
import { Loading } from "../../../common-ui/loading/loading";

@Component({
  selector: 'app-employees',
  imports: [Search, AsyncPipe, Loading],
  templateUrl: './employees.html',
  styleUrl: './employees.scss'
})
export class Employees {
  profileService = inject(ProfileService);
  route = inject(ActivatedRoute);
  usersPage$!: Observable<IPageResult<IProfile>>;
  querySub!: Subscription;
  baseUrl = 'https://localhost:7280/';


  ngOnInit(){
    this.querySub = this.route.queryParams.subscribe(params=>{
      this.usersPage$ = this.profileService.getEmployees(params);
    })
  }

  ngOnDestroy(){
    if(this.querySub){
      this.querySub.unsubscribe();
    }
  }



}
