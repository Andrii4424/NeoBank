import { Component, ElementRef, Host, HostListener, inject, signal, ViewChild } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Footer } from "../footer/footer";
import { BreakpointObserver } from '@angular/cdk/layout';

@Component({
  selector: 'app-layout',
  imports: [RouterOutlet, Footer],
  templateUrl: './layout.html',
  styleUrl: './layout.scss'
})


export class Layout {
  //menu animation
  compact = false;

  @HostListener('window:scroll', [])
  onWindowScroll() {
    if(pageYOffset > 440) {
      this.compact = true;
      console.log('scrolling', pageYOffset);
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

  @HostListener('window:click', ['$event'])
  onWindowClick(event: Event) {
    if(!(event.target instanceof Node)) return;
    else if (this.openMenu.nativeElement.contains(event.target as Node)) {
      this.openMenu.nativeElement.classList.remove('chosen');
      this.closeMenu.nativeElement.classList.add('chosen');
    }
    else if (this.closeMenu.nativeElement.contains(event.target as Node)) {
      this.openMenu.nativeElement.classList.add('chosen');
      this.closeMenu.nativeElement.classList.remove('chosen');
    }
  }


  constructor() {
    this.bo.observe(['(max-width: 1440px)']).subscribe(result => {
      this.labtop.set(result.matches);
    });

    this.bo.observe(['(max-width: 768px)']).subscribe(result => {
      this.mobile.set(result.matches);
    });
  }

}
