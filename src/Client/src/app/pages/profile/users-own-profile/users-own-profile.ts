import { ChangeDetectorRef, Component, ElementRef, inject, signal, ViewChild } from '@angular/core';
import { ProfileService } from '../../../data/services/auth/profile-service';
import { ActivatedRoute, Router } from '@angular/router';
import { IProfile } from '../../../data/interfaces/auth/profile-interface';
import { FormControl, FormGroup, ɵInternalFormsSharedModule, ReactiveFormsModule } from '@angular/forms';
import { SharedService } from '../../../data/services/shared-service';
import { AuthService } from '../../../data/services/auth/auth-service';
import { TranslateModule } from '@ngx-translate/core';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-users-own-profile',
  imports: [ɵInternalFormsSharedModule, ReactiveFormsModule, TranslateModule],
  templateUrl: './users-own-profile.html',
  styleUrl: './users-own-profile.scss'
})
export class UsersOwnProfile {
  //Services
  profileService = inject(ProfileService);
  sharedService = inject(SharedService);
  authService = inject(AuthService);
  baseUrl = `${environment.apiPhotoUrl}/`;

  //Local variables
  router = inject(Router);
  profile! :IProfile |null;
  updateMode = signal<boolean>(false);
  updatedAvatarFile :File |null = null;
  updatedAvatarSrc : string | null = null;
  validationErrors: string[] =[];
  validationSuccess = signal<boolean>(false);
  showValidationResult = signal<boolean>(false);

  profileForm = new FormGroup({
    email: new FormControl<string | null>(null),
    firstName: new FormControl<string | null>(null),
    surname: new FormControl<string | null>(null),
    patronymic: new FormControl<string | null>(null),
    dateOfBirth: new FormControl<string | null>(null),
    taxId: new FormControl<string | null>(null),
    phoneNumber: new FormControl<string | null>(null),
    jobTitle: new FormControl<string | null>(null),
    jobCategory: new FormControl<string | null>(null),
    salary: new FormControl<number | null>(null),
  });


  constructor(private cd: ChangeDetectorRef, private routeId: ActivatedRoute) {
    
  }

  ngOnInit(){
    this.profileService.getOwnProfile(false).subscribe({
      next:(val)=>{
        this.profile=val;
        if(this.profile.role===null){
          this.profile.role ="User";
        }
        this.patchFormValues();
        this.profileService.updateProfileSignal(val);
      },
      error: (err)=>{
        this.profile=null;
      }, 
      complete:()=>{
        this.cd.detectChanges();
      }
    })
  }

  toUpdateMode(){
    this.updateMode.set(true);
  }

  cancelChanges(){
    this.updatedAvatarFile = null;
    this.updateMode.set(false);
    this.patchFormValues();
  }

  uploadPhoto(event :Event){
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;
    if (!file) return;
    if (!file.type.startsWith('image/')) return;

    this.updatedAvatarFile = file;
    this.updatedAvatarSrc = URL.createObjectURL(file);
    this.cd.detectChanges();
    input.value = '';
  }

  submitForm(){
    const formValues = this.profileForm.getRawValue();
    const payload: IProfile = {          
      ...formValues,
      id: this.profile!.id,
      avatarPath: this.profile!.avatarPath,
      role: this.profile!.role,
      isVerified: this.profile!.isVerified,
      avatar: this.updatedAvatarFile ?? null,

    };

    this.profileService.updateUser(payload).subscribe({
      next: (val) =>{
        this.profileService.updateProfileSignal(val);
        this.profile=val;
        this.validationSuccess.set(true);
        this.updateMode.set(false);
      },
      error: (err)=>{
        this.validationErrors = this.sharedService.serverResponseErrorToArray(err);
        this.validationSuccess.set(false);
        this.showValidationResult.set(true);

      },
      complete:() =>{
        this.showValidationResult.set(true);
      }
    });
  }

  hideValidationResult(){
    this.showValidationResult.set(false);
    this.validationSuccess.set(false);
    this.validationErrors=[];
  }

  patchFormValues(){
    this.profileForm.patchValue({
      email: this.profile!.email,
      firstName: this.profile!.firstName,
      surname: this.profile!.surname,
      patronymic: this.profile!.patronymic,
      dateOfBirth: this.profile!.dateOfBirth,
      taxId: this.profile!.taxId,
      phoneNumber: this.profile!.phoneNumber,
      jobTitle: this.profile!.jobTitle,
      jobCategory: this.profile!.jobCategory,
      salary: this.profile!.salary
    });
  }

  logout(){
    localStorage.removeItem("role");
    this.authService.logout().subscribe({
    });
  }
}