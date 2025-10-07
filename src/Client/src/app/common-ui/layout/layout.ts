import { Component, ElementRef, Host, HostListener, inject, signal, ViewChild, ViewChildren } from '@angular/core';
import { RouterLink, RouterModule, RouterOutlet } from '@angular/router';
import { Footer } from "../footer/footer";
import { BreakpointObserver } from '@angular/cdk/layout';
import { SharedService } from '../../data/services/shared-service';
import { OwnProfile } from "../own-profile/own-profile";

@Component({
  selector: 'app-layout',
  imports: [RouterOutlet, Footer, RouterLink, OwnProfile, RouterModule],
  templateUrl: './layout.html',
  styleUrl: './layout.scss'
})


export class Layout {
  //menu animation
  compact = false;
  adaptive = false; //need to ignore scroll compact animation if it's labtop or mobile

  @HostListener('window:scroll', [])
  onWindowScroll() {
    if(pageYOffset > 440 && !this.adaptive) {
      this.compact = true;
    }
    else {
      this.compact = false;
    }
  }

  //adaptive
  private bo = inject(BreakpointObserver);
  labtop = signal(false);
  mobile = signal(false);
  @ViewChild('openMenu') openMenu!: ElementRef<HTMLImageElement> ;
  @ViewChild('closeMenu') closeMenu!: ElementRef<HTMLImageElement> ;
  @ViewChild('adaptiveMenu') adaptiveMenu!: ElementRef<HTMLElement> ;
  @ViewChild('main') main!: ElementRef<HTMLElement> ;


  @HostListener('window:click', ['$event'])
  onWindowClick(event: Event) {
    if(!(event.target instanceof Node)) return;
    else if (this.openMenu.nativeElement.contains(event.target as Node)) {
      this.openMenu.nativeElement.classList.remove('chosen');
      this.closeMenu.nativeElement.classList.add('chosen');
      this.adaptiveMenu.nativeElement.classList.add('opened');
    }
    else if (this.closeMenu.nativeElement.contains(event.target as Node) || this.main.nativeElement.contains(event.target as Node)) {

      this.openMenu.nativeElement.classList.add('chosen');
      this.closeMenu.nativeElement.classList.remove('chosen');
      this.adaptiveMenu.nativeElement.classList.remove('opened');
    }
  }

  constructor() {
    this.bo.observe(['(max-width: 1440px)']).subscribe(result => {
      this.labtop.set(result.matches);
      if(result.matches) this.adaptive = true;
      else this.adaptive = false;
    });

    this.bo.observe(['(max-width: 768px)']).subscribe(result => {
      this.mobile.set(result.matches);
    });
  }
}
