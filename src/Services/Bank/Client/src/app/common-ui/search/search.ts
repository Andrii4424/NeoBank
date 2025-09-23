import { Component, ElementRef, EventEmitter, Input, Output, QueryList, ViewChild, ViewChildren } from '@angular/core';
import { IFilter } from '../../data/filters/filter-interface';
import { ISort } from '../../data/filters/sort-interface';

@Component({
  selector: 'app-search',
  imports: [],
  templateUrl: './search.html',
  styleUrl: './search.scss'
})
export class Search {
  @Input() sortValues: ISort[] = [];
  @Input() searchPlaceholder: string ="";
  @Input() filterValues: IFilter[] =[];
  @ViewChildren('filter') filtersInputs!: QueryList<ElementRef<HTMLInputElement>>;
  @ViewChild('allFilters') allFilters!: ElementRef<HTMLInputElement>;
  @Output() sortEmmiter= new EventEmitter<string>();
  
  submitFilters(){
    this.allFilters.nativeElement.checked=true;
    this.filtersInputs.forEach(element => {
      if(element.nativeElement.checked){
        this.allFilters.nativeElement.checked=false;
      }
    });
  }
  deleteFilters(){
    this.allFilters.nativeElement.checked=true;
    this.filtersInputs.forEach(element => {
      element.nativeElement.checked=false;
    });
  }

  onSortChange(event: Event){
    const select = event.target as HTMLSelectElement;
    this.sortEmmiter.emit(select.value);
  }
}
