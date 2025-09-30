import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'cardFormat'   
})
export class CardFormatPipe implements PipeTransform {
    transform(value: string): string {
        if (!value) return '';
        return value.replace(/(\d{4})(?=\d)/g, '$1 ');
    }
}