import { Component, ElementRef, EventEmitter, HostListener, Input, Output, signal, ViewChild } from '@angular/core';
import { ITransactionFilter } from '../../../../data/interfaces/filters/transaction-filters';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-transactions-filters',
  imports: [FormsModule],
  templateUrl: './transactions-filters.html',
  styleUrl: './transactions-filters.scss'
})
export class TransactionsFilters {
  openFilters = signal<boolean>(false);
  @ViewChild('filtersBlock') filtersBlock?: ElementRef<HTMLDivElement>;
  @ViewChild('filtersImage') filtersImage?: ElementRef<HTMLImageElement>;
  @ViewChild('filtersButton') filtersButton?: ElementRef<HTMLImageElement>;
  @Input() selectedSortValue: string | null = null;
  @Input() minSumValue: string | null= null;
  @Input() dateValue: string | null= null;

  ngAfterViewInit(){
    console.log(this.selectedSortValue);
    if(this.selectedSortValue ===null){
      this.selectedSortValue ="date-descending";
    }
  }

  @Output() submitFiltersValues = new EventEmitter<ITransactionFilter>;

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
    this.submitFiltersValues.emit();
    this.openFilters.set(false);
  }
}
