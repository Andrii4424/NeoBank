import { VacancyService } from './../../../../data/services/bank/users/vacancy-service';
import { Component, inject, signal } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { SharedService } from '../../../../data/services/shared-service';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  selector: 'app-add-vacancy',
  imports: [TranslateModule, ReactiveFormsModule],
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

  }

  onSubmit(){

  }

}