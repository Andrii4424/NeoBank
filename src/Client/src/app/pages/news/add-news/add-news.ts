import { ChangeDetectorRef, Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';

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

  constructor(private cdr: ChangeDetectorRef) {
    
  }

  newsForm = new FormGroup({
    id: new FormControl<string | null>(null),
    title: new FormControl<string | null>(null, [Validators.required]),
    topic: new FormControl<string | null>(null, [Validators.required]),
    author: new FormControl<string | null>(null, [Validators.required]),
    content: new FormControl<string | null>(null, [Validators.required]),
    createdAt: new FormControl<string | null>(null),
    image: new FormControl<File | null>(null, [Validators.required])

  });

  uploadPhoto(event :Event){
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;
    if (!file) return;
    if (!file.type.startsWith('image/')) return;

    this.updatedAvatarFile = file;
    this.updatedAvatarSrc = URL.createObjectURL(file);
    this.cdr.detectChanges();
    input.value = '';
  }

  onSubmit(){
    
  }
}
