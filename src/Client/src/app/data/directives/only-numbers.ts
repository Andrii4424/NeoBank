import { Directive, ElementRef, HostListener } from '@angular/core';

@Directive({
  selector: '[appOnlyNumbers]'
})
export class OnlyNumbers {

  constructor(private input: ElementRef<HTMLInputElement>) { }

  @HostListener('input', ['$event'])
  onInputChange(event: Event): void {
    const initialValue = this.input.nativeElement.value;

    this.input.nativeElement.value = initialValue.replace(/\D/g, '');
    
    if (initialValue !== this.input.nativeElement.value) {
      event.stopPropagation();
    }
  }
}
