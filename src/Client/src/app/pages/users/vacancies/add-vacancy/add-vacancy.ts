import { VacancyService } from './../../../../data/services/bank/users/vacancy-service';
import { Component, inject, signal } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { SharedService } from '../../../../data/services/shared-service';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { IVacancy } from '../../../../data/interfaces/bank/users/vacancy-interface';
import { OnlyNumbers } from "../../../../data/directives/only-numbers";

@Component({
  selector: 'app-add-vacancy',
  imports: [TranslateModule, ReactiveFormsModule, OnlyNumbers],
  templateUrl: './add-vacancy.html',
  styleUrl: './add-vacancy.scss'
})
export class AddVacancy {
  vacancyService = inject(VacancyService);
  sharedService = inject(SharedService);
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

  closeValidationStatus(){
    this.validationErrors=[];
    this.successStatus.set(false);
    this.showValidationResult.set(false);
  }

  onSubmit(){
    if(this.vacancyForm.valid){
      this.vacancyService.addVacancy(this.vacancyForm.value as IVacancy).subscribe({
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