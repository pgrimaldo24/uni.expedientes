import { formatCurrency, getLocaleCurrencySymbol } from '@angular/common';
import { InjectionToken, Pipe, PipeTransform } from '@angular/core';
import { CultureService } from '@src/app/services/culture.service';
import { NavegatorService } from '@src/app/services/navegator.service';

export const DEFAULT_CURRENCY_CODE = new InjectionToken<string>(
  'DEFAULT_CURRENCY_CODE'
);

@Pipe({
  name: 'customCurrency'
})
export class CustomCurrencyPipe implements PipeTransform {
  localeCulture: string;
  localeNavegator: string;
  constructor(
    cultureService: CultureService,
    navegatorService: NavegatorService
  ) {
    this.localeCulture = cultureService.locale;
    this.localeNavegator = navegatorService.localeNavegator;
  }

  transform(
    value: number,
    currencyCode?: string,
    locale?: string,
    digistInfo = '.2-2'
  ): string | null {
    if (!locale && !this.localeCulture && !this.localeNavegator)
      return String(value);

    const localeCurrencySymbol = getLocaleCurrencySymbol(
      locale || this.localeCulture || this.localeNavegator
    );
    return formatCurrency(
      value,
      locale || this.localeCulture || this.localeNavegator,
      localeCurrencySymbol,
      currencyCode,
      digistInfo
    );
  }
}
