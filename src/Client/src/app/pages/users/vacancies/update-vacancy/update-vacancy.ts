import { Component, inject, signal } from '@angular/core';
import { SharedService } from '../../../../data/services/shared-service';
import { VacancyService } from '../../../../data/services/bank/users/vacancy-service';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { IVacancy } from '../../../../data/interfaces/bank/users/vacancy-interface';
import { TranslateModule } from '@ngx-translate/core';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-update-vacancy',
  imports: [TranslateModule, ReactiveFormsModule],
  templateUrl: './update-vacancy.html',
  styleUrl: './update-vacancy.scss'
})
export class UpdateVacancy {
  vacancyService = inject(VacancyService);
  sharedService = inject(SharedService);
  route = inject(ActivatedRoute);
  router= inject(Router);
  showValidationResult = signal<boolean>(false);
  successStatus = signal<boolean>(false);
  validationErrors : string[] = [];

  vacancyForm = new FormGroup({
    id: new FormControl<string | null>(null),
    jobTitle: new FormControl<string | null>(null,  [Validators.required]),
    category: new FormControl<string | null>(null, [Validators.required]),
    salary: new FormControl<number | null>(null, [Validators.required]),
    publicationDate: new FormControl<string | null>(null)
  });

  ngOnInit(){
    this.vacancyService.getVacancy(this.route.snapshot.paramMap.get("id")!).subscribe({
      next:(val)=>{
        this.vacancyForm.patchValue({
          id: val.id,
          jobTitle: val.jobTitle,
          category: val.category,
          salary: val.salary,
          publicationDate: val.publicationDate
        })
      },
      error:()=>{
        this.router.navigate(['/vacancies']);
      }
    });
  }


  closeValidationStatus(){
    this.validationErrors=[];
    this.successStatus.set(false);
    this.showValidationResult.set(false);
  }

  onSubmit(){
    if(this.vacancyForm.valid){
      this.vacancyService.updateVacancy(this.vacancyForm.value as IVacancy).subscribe({
        next:()=>{
          this.showSuccessMessage();
        },
        error:(err)=>{
          this.showValidationError(this.sharedService.serverResponseErrorToArray(err));
        }
      })
    }
    else{
      this.showValidationError("All fields has to be provided!");
    }
  }

  showValidationError(errorMessage: string | string[]){
    this.validationErrors=[];
    this.successStatus.set(false);
    if(typeof errorMessage ==='string'){
      this.validationErrors.push(errorMessage);
    }
    else{
      this.validationErrors=errorMessage;
    }
    this.showValidationResult.set(true);
  }

  showSuccessMessage(){
    this.validationErrors=[];
    this.successStatus.set(true);
    this.showValidationResult.set(true);
  }
}
