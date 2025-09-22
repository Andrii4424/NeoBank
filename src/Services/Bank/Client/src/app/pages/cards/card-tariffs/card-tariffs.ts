import { CardType } from './../../../data/enums/card-type';
import { PaymentSystem } from '../../../data/enums/payment-system';
import { ICardTariffs } from '../../../data/interfaces/bank/bank-products/card-tariffs.interface';
import { IPageResult } from '../../../data/interfaces/page-inteface';
import { CardTariffsService } from './../../../data/services/bank/bank-products/card-tariffs-service';
import { ChangeDetectorRef, Component, ElementRef, inject, QueryList, ViewChildren } from '@angular/core';

@Component({
  selector: 'app-card-tariffs',
  imports: [],
  templateUrl: './card-tariffs.html',
  styleUrl: './card-tariffs.scss'
})
export class CardTariffs {
  cardTariffsService = inject(CardTariffsService);
  cards?:IPageResult<ICardTariffs>;
  @ViewChildren('card') cardElements? : QueryList<ElementRef<HTMLDivElement>>

  constructor(private cdr: ChangeDetectorRef) {}

  //Enums
  PaymentSystem = PaymentSystem;
  CardType = CardType;
  
  ngOnInit(){
    this.cardTariffsService.getDefaultCardTarifs().subscribe({
      next:(val)=>{
        this.cards=val;
        this.cdr.detectChanges();
        this.updateCardTextColors();
      }
    });
  }



  private updateCardTextColors(): void {
    this.cardElements?.forEach(cardRef => {
      const el = cardRef.nativeElement;
      const style = window.getComputedStyle(el);
      const bgColor = style.backgroundColor;
      const rgb = this.extractRGB(bgColor);
      if (!rgb) {
        el.style.color = '#102A5A';
        return;
      }

      const [r, g, b] = rgb;

      const brightness = (r * 299 + g * 587 + b * 114) / 1000;

      if (brightness < 128) {
        el.style.color = 'white'; 
      } else {
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
