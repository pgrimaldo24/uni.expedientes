import { Pipe, PipeTransform } from '@angular/core';
import { NavegatorService } from '@src/app/services/navegator.service';

@Pipe({
  name: 'customDateCulture'
})
export class CustomDateCulturePipe implements PipeTransform {
  locale: string;
  constructor(navegatorService: NavegatorService) {
    this.locale = navegatorService.localeNavegator;
  }

  transform(value: Date | number): string {
    return new Date(value).toLocaleDateString(this.locale, {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit'
    });
  }
}
