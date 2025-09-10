import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class BankService {
  http = inject(HttpClient);


  testRequest(){
    return this.http.post('https://localhost:7280/api/Bank', "");
  }
}
