import { SharedService } from './../../../../data/services/shared-service';
import { Component, inject } from '@angular/core';
import { ISort } from '../../../../data/interfaces/filters/sort-interface';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ProfileService } from '../../../../data/services/auth/profile-service';
import { IPageResult } from '../../../../data/interfaces/page-inteface';
import { combineLatest, map, Observable, Subscription } from 'rxjs';
import { IProfile } from '../../../../data/interfaces/auth/profile-interface';
import { IFilter } from '../../../../data/interfaces/filters/filter-interface';
import { AsyncPipe } from '@angular/common';
import { Search } from "../../../../common-ui/search/search";
import { Loading } from "../../../../common-ui/loading/loading";
import { PageSwitcher } from "../../../../common-ui/page-switcher/page-switcher";
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { environment } from '../../../../../environments/environment';

@Component({
  selector: 'app-users',
  imports: [AsyncPipe, Search, Loading, PageSwitcher, RouterLink, TranslateModule],
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
  baseUrl = `${environment.apiUrl}/`;
   translate = inject(TranslateService);

  //Filters values
  sortValues$: Observable<ISort[]> = combineLatest([
    this.translate.stream('Sort.ByNameAsc'),
    this.translate.stream('Sort.ByNameDesc'),
    this.translate.stream('Sort.ByDateAsc'),
    this.translate.stream('Sort.ByDateDesc'),
  ]).pipe(
    map(([byNameAsc, byNameDesc, byDateAsc, byDateDesc])=>[
      {name: "surname-ascending", description: byNameAsc},
      {name: "surname-descending", description: byNameDesc},
      {name: "date-ascending", description: byDateAsc},
      {name: "date-descending", description: byDateDesc}
    ])
  )

  filterValues$: Observable<IFilter[]> = combineLatest([
    this.translate.stream('Filter.VerifiedUsers'),
    this.translate.stream('Filter.WithAvatars'),
    this.translate.stream('Filter.HasNumber'),
  ]).pipe(
    map(([verifiedUsers, withAvatars, hasNumber])=>[
      {filterName:"VerifiedUsers", id: "VerifiedUsers", description: verifiedUsers, value: true, chosen: false },
      {filterName:"WithAvatars", id: "WithAvatars", description: withAvatars, value: true, chosen: false },
      {filterName:"HasFinancalNumber", id: "HasFinancalNumber", description: hasNumber, value: true, chosen: false },
    ])
  )


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
