import { Component, Host, HostListener } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-layout',
  imports: [RouterOutlet],
  templateUrl: './layout.html',
  styleUrl: './layout.scss'
})


export class Layout {

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
}
