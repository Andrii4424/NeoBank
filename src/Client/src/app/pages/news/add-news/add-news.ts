import { SharedService } from './../../../data/services/shared-service';
import { ChangeDetectorRef, Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { IAddNews } from '../../../data/interfaces/news/add-news-interface';
import { NewsService } from '../../../data/services/bank/news/news-service';

@Component({
  selector: 'app-add-news',
  imports: [TranslateModule, ReactiveFormsModule],
  templateUrl: './add-news.html',
  styleUrl: './add-news.scss'
})
export class AddNews {
  selectedFile: File | null = null;
  updatedAvatarFile :File |null = null;
  updatedAvatarSrc : string | null = null;
  validationErrors: string[] =[];
  showValidationResult = signal<boolean>(false);
  successResult = signal<boolean>(false);
  private translate = inject(TranslateService);  
  private newsService = inject(NewsService);
  private sharedService = inject(SharedService);

  constructor(private cdr: ChangeDetectorRef) {
    
  }

  newsForm = new FormGroup({
    id: new FormControl<string | null>(null),
    title: new FormControl<string | null>(null, [Validators.required]),
    topic: new FormControl<string | null>(null, [Validators.required]),
    author: new FormControl<string | null>(null, [Validators.required]),
    content: new FormControl<string | null>(null, [Validators.required]),
    createdAt: new FormControl<string | null>(null),
    image: new FormControl<File | null>(null)
  });

  uploadPhoto(event :Event){
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;
    if (!file) return;
    if (!file.type.startsWith('image/')) return;

    this.updatedAvatarFile = file;
    this.updatedAvatarSrc = URL.createObjectURL(file);

    this.newsForm.patchValue({ image: file });

    this.newsForm.get('image')?.updateValueAndValidity();

    this.cdr.detectChanges();
    input.value = '';
  }

  onSubmit(){
    if(this.newsForm.valid){
      const formValue = this.newsForm.value as IAddNews;
      const file = this.newsForm.get('image')?.value;
      if(file === null){
        this.showValidationResult.set(true);
        this.successResult.set(false);
        this.validationErrors.push(this.translate.instant('Validation.ImageRequired'));
      }
      else{
        formValue.image = file!;
        this.newsService.addNews(formValue).subscribe({
          next: () => {
            this.validationErrors = [];
            this.showValidationResult.set(true);
            this.successResult.set(true);
            this.newsForm.reset();
          },
          error: (err) => {
            this.showValidationResult.set(true);
            this.successResult.set(false);
            this.validationErrors = this.sharedService.serverResponseErrorToArray(err);
          }
        });
      }
    }
    else{
      this.showValidationResult.set(true);
      this.validationErrors.push(this.translate.instant('Validation.AllFieldsRequired'));
    }
  }

  deleteStatus(){
    this.showValidationResult.set(false);
    this.successResult.set(false);
    this.validationErrors = [];
  }
  
}
