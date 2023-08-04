import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class NavegatorService {
  private _localeNavegator: string;
  set localeNavegator(value: string) {
    this._localeNavegator = value;
  }
  get localeNavegator(): string {
    return this._localeNavegator || 'es';
  }

  public static getNavigatorLanguage(): string {
    let browserLang = NavegatorService.getNavegatorCulture();
    if (!browserLang) return;

    if (browserLang.indexOf('-') !== -1) {
      browserLang = browserLang.split('-')[0];
    }

    if (browserLang.indexOf('_') !== -1) {
      browserLang = browserLang.split('_')[0];
    }
    return browserLang;
  }

  public static getNavegatorCulture(): string | undefined {
    if (
      typeof window === 'undefined' ||
      typeof window.navigator === 'undefined'
    ) {
      return undefined;
    }

    const languages = window.navigator['languages'];

    let browserLang: string = languages
      ? languages[languages.length - 1]
      : null;
    browserLang =
      browserLang ||
      window.navigator.language ||
      window.navigator['browserLanguage'] ||
      window.navigator['userLanguage'];
    return browserLang;
  }
}
