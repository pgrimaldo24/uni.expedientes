import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class CultureService {
  private _locale: string;
  private culture$ = new Subject<string>();

  set locale(value: string) {
    this._locale = value;
  }
  get locale(): string {
    return this._locale || 'es';
  }

  set setCulture$(value: string) {
    this.culture$.next(value);
  }

  get getCulture$(): Observable<string> {
    return this.culture$.asObservable();
  }

  static replaceCultureDefault(value: string): string {
    if (!value) {
      return 'es';
    }
    return value.replace('es-ES', 'es');
  }

  static toLowerCase(value: string): string {
    if (value.split('-').length == 1) return value.toLocaleLowerCase();
    return value;
  }
}
