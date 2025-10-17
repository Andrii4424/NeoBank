import { ChangeDetectorRef, Component, ElementRef, Host, HostListener, inject, signal, ViewChild, ViewChildren } from '@angular/core';
import { RouterLink, RouterModule, RouterOutlet } from '@angular/router';
import { Footer } from "../footer/footer";
import { BreakpointObserver } from '@angular/cdk/layout';
import { OwnProfile } from "../own-profile/own-profile";
import { FormsModule } from '@angular/forms';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-layout',
  imports: [RouterOutlet, Footer, RouterLink, OwnProfile, RouterModule, FormsModule, TranslateModule],
  templateUrl: './layout.html',
  styleUrl: './layout.scss'
})


export class Layout {
  //menu animation
  compact = false;
  chosenLanguage: string | null=null;
  adaptive = false; //need to ignore scroll compact animation if it's labtop or mobile
  showLanguageWindow = signal<boolean>(false);
  @ViewChild('languageForm') languageForm!: ElementRef<HTMLElement> ;
  @ViewChild('openLanguageWindow') openLanguageWindow!: ElementRef<HTMLElement> ;

  //adaptive
  private bo = inject(BreakpointObserver);
  labtop = signal(false);
  mobile = signal(false);
  @ViewChild('openMenu') openMenu!: ElementRef<HTMLImageElement> ;
  @ViewChild('closeMenu') closeMenu!: ElementRef<HTMLImageElement> ;
  @ViewChild('adaptiveMenu') adaptiveMenu!: ElementRef<HTMLElement> ;
  @ViewChild('main') main!: ElementRef<HTMLElement> ;


  constructor(private translate: TranslateService) {
    this.bo.observe(['(max-width: 1440px)']).subscribe(result => {
      this.labtop.set(result.matches);
      if(result.matches) this.adaptive = true;
      else this.adaptive = false;
    });

    this.bo.observe(['(max-width: 768px)']).subscribe(result => {
      this.mobile.set(result.matches);
    });
  }

  ngOnInit(){
    //Language settings
    this.chosenLanguage = localStorage.getItem("language");
    if(this.chosenLanguage===null){
      this.chosenLanguage = "en";
      localStorage.setItem("language", "en");
    }
    this.translate.use(this.chosenLanguage); 
  }
  
  onLanguageChange(){
    localStorage.setItem("language", this.chosenLanguage!);
    this.translate.use(this.chosenLanguage!); 
  }

  @HostListener('window:scroll', [])
  onWindowScroll() {
    if(pageYOffset > 440 && !this.adaptive) {
      this.compact = true;
    }
    else {
      this.compact = false;
    }
  }


  @HostListener('window:click', ['$event'])
  onWindowClick(event: Event) {
    if(!(event.target instanceof Node)) return;
    else{
      if (this.openMenu.nativeElement.contains(event.target as Node)) {
        this.openMenu.nativeElement.classList.remove('chosen');
        this.closeMenu.nativeElement.classList.add('chosen');
        this.adaptiveMenu.nativeElement.classList.add('opened');
      }
      else if (this.closeMenu.nativeElement.contains(event.target as Node) || this.main.nativeElement.contains(event.target as Node)) {
        this.openMenu.nativeElement.classList.add('chosen');
        this.closeMenu.nativeElement.classList.remove('chosen');
        this.adaptiveMenu.nativeElement.classList.remove('opened');
      }
      if(this.openLanguageWindow.nativeElement.contains(event.target as Node)){
        this.showLanguageWindow.set(!this.showLanguageWindow());
      }
      else if(!(this.languageForm.nativeElement.contains(event.target as Node))){
        this.showLanguageWindow.set(false);
      }
    }
  }

}
