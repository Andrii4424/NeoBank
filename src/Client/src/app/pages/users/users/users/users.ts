import { SharedService } from './../../../../data/services/shared-service';
import { Component, inject } from '@angular/core';
import { ISort } from '../../../../data/interfaces/filters/sort-interface';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ProfileService } from '../../../../data/services/auth/profile-service';
import { IPageResult } from '../../../../data/interfaces/page-inteface';
import { Observable, Subscription } from 'rxjs';
import { IProfile } from '../../../../data/interfaces/auth/profile-interface';
import { IFilter } from '../../../../data/interfaces/filters/filter-interface';
import { AsyncPipe } from '@angular/common';
import { Search } from "../../../../common-ui/search/search";
import { Loading } from "../../../../common-ui/loading/loading";
import { PageSwitcher } from "../../../../common-ui/page-switcher/page-switcher";

@Component({
  selector: 'app-users',
  imports: [AsyncPipe, Search, Loading, PageSwitcher, RouterLink],
  templateUrl: './users.html',
  styleUrl: './users.scss'
})
export class Users {
  profileService = inject(ProfileService);
  sharedService = inject(SharedService);
  route = inject(ActivatedRoute);
  router = inject(Router);
  usersPage$!: Observable<IPageResult<IProfile>>;
  querySub!: Subscription;
  baseUrl = 'https://localhost:7280/';

  //Filters values
  sortValues: ISort[]=[
      {name: "surname-ascending", description: "By Name (A-Z)"},
      {name: "surname-descending", description: "By Name (Z-A)"},
      {name: "date-ascending", description: "date-ascending"},
      {name: "date-descending", description: "date-descending"}
  ];
  
  searchPlaceholder: string ="Enter the card name";
  
  filterValues: IFilter[]=[
    {filterName:"VerifiedUsers", id: "VerifiedUsers", description: "Verified Users", value: true, chosen: false },
    {filterName:"WithAvatars", id: "WithAvatars", description: "With Avatars", value: true, chosen: false },
    {filterName:"HasFinancalNumber", id: "HasFinancalNumber", description: "Has Financal Number", value: true, chosen: false },
  ]

  ngOnInit(){
    this.querySub = this.route.queryParams.subscribe(params=>{
      this.usersPage$ = this.profileService.getUsers(params);
    })
  }

  ngOnDestroy(){
    if(this.querySub){
      this.querySub.unsubscribe();
    }
  }

  //Filters and pagination handlers
  onSortChange(sortMethod: string){
    this.router.navigate([],{
      relativeTo: this.route,
      queryParams: {SortValue : sortMethod, PageNumber: 1},
      queryParamsHandling: 'merge'
    });
  }

  onSearchChange(searchValue: string){
    this.router.navigate([],{
      relativeTo: this.route,
      queryParams: {SearchValue : searchValue, PageNumber: 1},
      queryParamsHandling: 'merge'
    });
  }

  onPageChange(pageNumber: number){
    this.router.navigate([],{
      relativeTo: this.route,
      queryParams: {PageNumber : pageNumber},
      queryParamsHandling: 'merge'
    });
  }

  onFiltersChange(filters: IFilter[] | null){
    if(filters===null){
      const savedParamsKeys =["PageNumber", "SearchValue", "SortValue"];
      const params= this.route.snapshot.queryParams;
      const newParams: any = { ...params, PageNumber: 1 };
      Object.keys(params).forEach(key => {
        if(!savedParamsKeys.includes(key)){
          newParams[key] = null;
        }
      });

      this.router.navigate([],{
        relativeTo: this.route,
        queryParams: newParams,
        queryParamsHandling: 'merge'
      });
    }
    else{
      const params = this.sharedService.getQueryList(filters);
      this.router.navigate([],{
        relativeTo: this.route,
        queryParams: params,
      });
    }
  }

}
