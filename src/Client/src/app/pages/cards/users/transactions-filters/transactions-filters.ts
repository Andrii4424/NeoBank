import { Component, ElementRef, HostListener, signal, ViewChild } from '@angular/core';

@Component({
  selector: 'app-transactions-filters',
  imports: [],
  templateUrl: './transactions-filters.html',
  styleUrl: './transactions-filters.scss'
})
export class TransactionsFilters {
  openFilters = signal<boolean>(false);
  @ViewChild('filtersBlock') filtersBlock?: ElementRef<HTMLDivElement>;
  @ViewChild('filtersImage') filtersImage?: ElementRef<HTMLImageElement>;
  @ViewChild('filtersButton') filtersButton?: ElementRef<HTMLImageElement>;

  @HostListener('window:click', ['$event'])
  onWindowClick(event: Event){
    if(!(event.target instanceof Node)) return;
    else if (this.filtersBlock !==undefined && this.filtersBlock.nativeElement.contains(event.target as Node)) {
      if(this.filtersButton!.nativeElement.contains(event.target as Node)){
        this.openFilters.set(false);
      }
      else{
        this.openFilters.set(true);
      }
    }
    else if(this.filtersImage!.nativeElement.contains(event.target as Node)){
      if(this.openFilters()){
        this.submitFilters()
      }  
      else {
        this.openFilters.set(true);
      }
    }
    else{
      this.submitFilters();
    }
  }

  submitFilters(){
      this.openFilters.set(false);
  }
}
