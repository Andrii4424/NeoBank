import { ProfileService } from './../../../../data/services/auth/profile-service';
import { VacancyService } from '../../../../data/services/bank/users/vacancy-service';
import { Component, inject } from '@angular/core';
import { Search } from "../../../../common-ui/search/search";
import { Observable, Subscription } from 'rxjs';
import { IVacancy } from '../../../../data/interfaces/bank/users/vacancy-interface';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { IPageResult } from '../../../../data/interfaces/page-inteface';
import { AsyncPipe } from '@angular/common';
import { Loading } from "../../../../common-ui/loading/loading";
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-vacancies',
  imports: [Search, AsyncPipe, Loading, RouterLink, TranslateModule],
  templateUrl: './vacancies.html',
  styleUrl: './vacancies.scss'
})
export class Vacancies {
  vacancyService = inject(VacancyService);
  route= inject(ActivatedRoute);
  router = inject(Router);
  profileService = inject(ProfileService);
  $vacancy? : Observable<IPageResult<IVacancy>>;
  private queryParamsSubscription!: Subscription;

  ngOnInit(){
    this.queryParamsSubscription = this.route.queryParams.subscribe(params => {
      this.$vacancy = this.vacancyService.getVacanciesPage(params);
    })
  }
  

}
