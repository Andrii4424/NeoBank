import { HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class SharedService {
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

}
