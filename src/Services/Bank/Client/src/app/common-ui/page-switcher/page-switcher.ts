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

  getPagesArray(){
    let numbersArray: number[]=[];
    if(this.currentPage !==1 && this.currentPage !==this.pageCount ) {
      if(this.currentPage!-1 > 1){
        numbersArray.push(this.currentPage!-1);
      }
      numbersArray.push(this.currentPage!); 
      if(this.currentPage!+1 <this.pageCount!){
        numbersArray.push(this.currentPage!+1);
      }
    }
    else if(this.currentPage ===1){
      numbersArray.push(2);
      numbersArray.push(3);
    }
    else if(this.currentPage === this.pageCount){
      numbersArray.push(this.pageCount!-2);
      numbersArray.push(this.pageCount!-1);
    }
    return numbersArray;
  }

  changePage(pageNumber: number ){
    if(this.currentPage!==pageNumber && pageNumber>0 && pageNumber<=this.pageCount!){
      this.pageChange.emit(pageNumber);
    }
  }
}
