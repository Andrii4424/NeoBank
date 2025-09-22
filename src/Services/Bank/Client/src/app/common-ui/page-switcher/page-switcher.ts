import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-page-switcher',
  imports: [],
  templateUrl: './page-switcher.html',
  styleUrl: './page-switcher.scss'
})
export class PageSwitcher {
  @Input() pageCount?: number;
  @Input() currentPage?: number;
  @Input() hasPreviousPage?: boolean;
  @Input() hasNextPage?: boolean;


  getElementsCountArray(){
    let array:number[] = [];
    for(let i=2; i<=this.pageCount!; i++){
      array.push(i);
    }
    return array;
  }
}
