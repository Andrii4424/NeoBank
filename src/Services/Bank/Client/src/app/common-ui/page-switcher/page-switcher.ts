import { Component, EventEmitter, Input, Output } from '@angular/core';

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
  @Output() pageChange = new EventEmitter<number>();


  getElementsCountArray(){
    let array:number[] = [];
    for(let i=2; i<=this.pageCount!; i++){
      array.push(i);
    }
    return array;
  }

  changePage(pageNumber: number ){
    if(this.currentPage!==pageNumber && pageNumber>0 && pageNumber<=this.pageCount!){
      this.pageChange.emit(pageNumber);
    }
  }
}
