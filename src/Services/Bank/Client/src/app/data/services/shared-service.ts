import { HttpErrorResponse } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { IFilter } from '../interfaces/filters/filter-interface';
import { IQueryArray } from '../interfaces/filters/query-array-interface';
import { ActivatedRoute } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class SharedService {
  route = inject(ActivatedRoute);

  copyText(text:string){
    return navigator.clipboard.writeText(text);
  }

  serverResponseErrorToArray(err : HttpErrorResponse) :string[]{
    const out: string[] = [];

    if (Array.isArray(err.error)) {
      for (const e of err.error as any[]) {
        if (typeof e === 'string') out.push(e);
        else if (e?.description) out.push(e.description);
        else if (e?.message) out.push(e.message);
        else out.push(JSON.stringify(e));
      }
      return out;
    }

    if (err.error?.errors && typeof err.error.errors === 'object') {
      for (const v of Object.values(err.error.errors) as unknown[]) {
        if (Array.isArray(v)) v.forEach(m => out.push(String(m)));
      }
      return out;
    }

    if (err.error?.message) {
      out.push(String(err.error.message));
      return out;
    }

    if (typeof err.error === 'string') {
      out.push(err.error);
      return out;
    }

    if(out.length===0){
      out.push(`HTTP ${err.status}: ${'Unknown error'}`);
    }

    return out;
  }


  toQueryMatrix(filters: IFilter[]){
    let keysArray: string[] = [];
    filters.forEach(filter => {
      if(!keysArray.includes(filter.filterName)) keysArray.push(filter.filterName);
    });

    let queryMatrix: IQueryArray[] =[];
    keysArray.forEach(arrayKey => {
      let keyValues: string[] =[];
      filters.forEach(filter => {
        if(filter.filterName===arrayKey){
          keyValues.push(filter.value);
        }
      });
      queryMatrix.push({key: arrayKey, values: keyValues});
    });
    return queryMatrix;
  }

  getQueryList(filters: IFilter[]){
    const queryParams: any = {};
    let queryList: IQueryArray[] = this.toQueryMatrix(filters);
    queryList.forEach(element => {
      queryParams[element.key] = element.values;
    });
    queryParams["page"] = this.route.snapshot.queryParams["page"];
    queryParams["search"] = this.route.snapshot.queryParams["search"];
    queryParams["sortBy"] = this.route.snapshot.queryParams["sortBy"];


    return queryParams;
  }
}
