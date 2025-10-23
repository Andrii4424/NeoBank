import { CardType } from './../../../../data/enums/card-type';
import { PaymentSystem } from './../../../../data/enums/payment-system';
import { Currency } from './../../../../data/enums/currency';
import { CardStatus } from './../../../../data/enums/card-status';
import { ActivatedRoute } from '@angular/router';
import { UserCardsService } from './../../../../data/services/bank/bank-products/user-cards-service';
import { ChangeDetectorRef, Component, ElementRef, inject, QueryList, signal, ViewChildren } from '@angular/core';
import { ProfileService } from '../../../../data/services/auth/profile-service';
import { IUserCards } from '../../../../data/interfaces/bank/bank-products/cards/user-cards-interface';
import { TranslateModule } from '@ngx-translate/core';
import { SharedService } from '../../../../data/services/shared-service';
import { SuccessMessage } from "../../../../common-ui/success-message/success-message";
import { CardFormatPipe } from "../../../../data/pipes/card-format.pipe";
import { ICroppedUserCard } from '../../../../data/interfaces/bank/bank-products/cards/cropped-user-cards-interface';

@Component({
  selector: 'app-cards',
  imports: [TranslateModule, SuccessMessage, CardFormatPipe],
  templateUrl: './cards.html',
  styleUrl: './cards.scss'
})
export class Cards {
  userCardsService = inject(UserCardsService);
  profileService = inject(ProfileService);
  sharedService = inject(SharedService);
  isAdmin: boolean;
  copied = signal<boolean>(false);
  route = inject(ActivatedRoute);
  userId: string;
  fullCardsInfo: IUserCards[] | null = null;
  croppedCardsInfo: ICroppedUserCard[] | null = null;
  @ViewChildren('card') cardElements? : QueryList<ElementRef<HTMLDivElement>>;

  CardStatus = CardStatus;
  Currency = Currency;
  PaymentSystem = PaymentSystem;
  CardType= CardType;

  constructor(private cdr: ChangeDetectorRef){
    this.isAdmin = this.profileService.getRole() ==="Admin";
    this.userId = this.route.snapshot.paramMap.get('id')!;
  }

  ngOnInit(){
    if(!this.isAdmin){
      this.userCardsService.getUserFullCards(this.userId). subscribe({
        next:(val)=>{
          this.fullCardsInfo = val;
          this.cdr.detectChanges();

        },
        error:()=>{
        },
        complete:()=>{
          this.updateCardTextColors();
          this.cdr.detectChanges();
        }
      });
    }
    else{
      this.userCardsService.getUserCroppedCards(this.userId).subscribe({
        next:(val)=>{
          this.croppedCardsInfo= val;
          this.cdr.detectChanges();
        },
        error:()=>{
        },
        complete:()=>{
          this.updateCardTextColors();
          this.cdr.detectChanges();
        }
      })
    }
  }

  copyCardNumber(cardNumber : string, event: Event){
    event.stopPropagation();
    event.preventDefault();

    this.copied.set(false);
    this.sharedService.copyText(cardNumber);
    this.copied.set(true);
    setTimeout(() => this.copied.set(false), 3000);
  }


  //Color methods
  private updateCardTextColors(): void {
    this.cardElements?.forEach(cardRef => {
      const el = cardRef.nativeElement;
      const style = window.getComputedStyle(el);
      const bgColor = style.backgroundColor;
      const rgb = this.extractRGB(bgColor);
      if (!rgb) {
        const child = el.querySelector('.card-bank-logo');
        if(child){
          (child as HTMLImageElement).src="assets/images/bank-logo.png";
        }
        el.style.color = '#102A5A';
        return;
      }

      const [r, g, b] = rgb;

      const brightness = (r * 299 + g * 587 + b * 114) / 1000;

      if (brightness < 128) {
        const child = el.querySelector('.card-bank-logo');
        if(child){
          (child as HTMLImageElement).src="assets/images/white-bank-logo.png";
        }
        el.style.color = 'white';
      } else {
        const child = el.querySelector('.card-bank-logo');
        if(child){
          (child as HTMLImageElement).src="assets/images/bank-logo.png";
        }
        el.style.color = '#102A5A'; 
      }
    });
  }

  private extractRGB(color: string): [number, number, number] | null {
    const match = color.match(/rgba?\((\d+),\s*(\d+),\s*(\d+)/);
    if (!match) return null;
    return [parseInt(match[1]), parseInt(match[2]), parseInt(match[3])];
  }
}
