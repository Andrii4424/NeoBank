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
  @Input() minSumValue: number | null= null;
  @Input() dateValue?: string | null= null;
  selectedSortStartValue: string | null = null;
  minSumStartValue: number | null = null;
  dateStartValue?: string | null = null;


  ngAfterViewInit(){  
    console.log(this.minSumValue);
    console.log(this.selectedSortValue);
    console.log(this.dateValue);
    if(this.selectedSortValue == null){
      this.selectedSortValue ="date-descending";
    }
    this.selectedSortStartValue = this.selectedSortValue;
    this.minSumStartValue = this.minSumValue;
    this.dateStartValue = this.dateValue;
  }

  @Output() submitFiltersValues = new EventEmitter<ITransactionFilter>;

  submitFilters(){
    if(this.selectedSortStartValue !== this.selectedSortValue || this.dateStartValue !== this.dateValue || this.minSumStartValue !== this.minSumValue){
      this.dateStartValue = this.dateValue;
      this.minSumStartValue = this.minSumValue;
      this.submitFiltersValues.emit({sortValue: this.selectedSortValue, minimalSum: this.minSumValue, transactionsDate: this.dateValue });
    }
    this.openFilters.set(false);
  }

  filterHasValue() {
    return this.dateValue == null && this.minSumValue == null;
  }

  closeFilters(){
    this.dateValue = this.dateStartValue;
    this.minSumValue = this.minSumStartValue;

    this.openFilters.set(false);
  }
}
