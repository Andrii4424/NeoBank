import { Component, ElementRef, EventEmitter, Input, Output, QueryList, ViewChild, ViewChildren } from '@angular/core';
import { IFilter } from '../../data/interfaces/filters/filter-interface';
import { ISort } from '../../data/interfaces/filters/sort-interface';

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
  @ViewChild('searchInput') searchInput!: ElementRef<HTMLInputElement>;
  @Output() sortChange= new EventEmitter<string>();
  @Output() searchChange= new EventEmitter<string>();
  @Output() filterChange= new EventEmitter<IFilter[] | null>();
  

  submitFilters(){
    this.allFilters.nativeElement.checked=true;
    let filters: IFilter[] = [];
    this.filtersInputs.forEach(element => {
      if(element.nativeElement.checked){
        this.allFilters.nativeElement.checked=false;
        filters.push({filterName: element.nativeElement.dataset['filterName']!,
          value: element.nativeElement.value} as IFilter);
      }
    });
    if(filters.length===0){
      this.filterChange.emit(null);
    }
    else{
      this.filterChange.emit(filters);
    }
  }
  deleteFilters(){
    this.allFilters.nativeElement.checked=true;
    this.filtersInputs.forEach(element => {
      element.nativeElement.checked=false;
    });
    this.filterChange.emit(null);
  }

  onSortChange(event: Event){
    const select = event.target as HTMLSelectElement;
    this.sortChange.emit(select.value);
  }
  
  onSearchChange(event: Event){
    const input = event.target as HTMLInputElement;
    this.searchChange.emit(input.value);
  }

  onSearchSubmit(){
    this.searchChange.emit(this.searchInput.nativeElement.value);
  }
}
