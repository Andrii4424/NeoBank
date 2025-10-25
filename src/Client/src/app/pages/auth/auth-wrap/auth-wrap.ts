import { Component } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { RouterModule } from "@angular/router";

@Component({
  selector: 'app-auth-wrap',
  imports: [RouterModule],
  templateUrl: './auth-wrap.html',
  styleUrl: './auth-wrap.scss'
})
export class AuthWrap {

  constructor(private translate: TranslateService){
    
  }

  ngOnInit(){
    //Language settings
    let chosenLanguage = localStorage.getItem("language");
    if(chosenLanguage===null){
      chosenLanguage = "en";
      localStorage.setItem("language", "en");
    }
    this.translate.use(chosenLanguage); 
  }
}
