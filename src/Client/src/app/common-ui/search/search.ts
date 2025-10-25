import { ChangeDetectorRef, Component, ElementRef, EventEmitter, inject, input, Input, Output, QueryList, ViewChild, ViewChildren } from '@angular/core';
import { IFilter } from '../../data/interfaces/filters/filter-interface';
import { ISort } from '../../data/interfaces/filters/sort-interface';
import { ActivatedRoute } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-search',
  imports: [TranslateModule],
  templateUrl: './search.html',
  styleUrl: './search.scss'
})
export class Search {
  route = inject(ActivatedRoute);
  queryParams: Record<string, any> = {}

  @Input() sortValues: ISort[] = [];
  @Input() searchPlaceholder: string ="";
  @Input() filterValues: IFilter[] =[];
  @ViewChildren('filter') filtersInputs!: QueryList<ElementRef<HTMLInputElement>>;
  @ViewChild('allFilters') allFilters!: ElementRef<HTMLInputElement>;
  @ViewChild('searchInput') searchInput!: ElementRef<HTMLInputElement>;
  @ViewChild('sortSelector') sortSelector!: ElementRef<HTMLInputElement>;
  @Output() sortChange= new EventEmitter<string>();
  @Output() searchChange= new EventEmitter<string>();
  @Output() filterChange= new EventEmitter<IFilter[] | null>();
  
  ngAfterViewInit(){
    this.updateFilters();
  }

  constructor(private cdr: ChangeDetectorRef) {  }
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

  public updateFilters(){
    this.queryParams = { ...this.route.snapshot.queryParams };
    Object.keys(this.queryParams).forEach(query =>{
      this.filtersInputs.forEach(filter => {
        if(filter.nativeElement.dataset['filterName'] ===query){
          this.allFilters.nativeElement.checked=false;
          if(filter.nativeElement.value ===this.queryParams[query]){
            filter.nativeElement.checked = true;
          }
        }
      });
      if(query==="SearchValue"){
          this.searchInput.nativeElement.value = this.queryParams[query];
        }
      else if(query==="SortValue" && this.queryParams[query]!==""){
        this.sortSelector.nativeElement.value = this.queryParams[query];
      }
    });
  }

  setUpdatedFilters(){
    const chosenValue =(this.filterValues.find(val=>val.chosen === true))?.value;
    this.filtersInputs.forEach(element => {
      element.nativeElement.checked=element.nativeElement.value===chosenValue;
      if(element.nativeElement.checked){
        this.allFilters.nativeElement.checked=false;
      }
    });
    this.cdr.detectChanges();
  }
}
