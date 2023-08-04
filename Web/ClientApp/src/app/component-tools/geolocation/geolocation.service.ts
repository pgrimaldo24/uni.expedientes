import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { keys } from '@src/keys';
import { Country, Division, LevelDivision } from './models';

@Injectable({
  providedIn: 'root'
})
export class GeolocationService {
  private baseUrl: string;

  constructor(private http: HttpClient) {
    this.http = http;
    this.baseUrl = keys.API_BASE_URL;
  }

  getCountry(countryCode: string): Observable<Country> {
    return this.http.get<Country>(
      `${this.baseUrl}/api/v1/integracion/commons/countries/${countryCode}`
    );
  }

  getDivisionsCountry(isoCode: string): Observable<Division[]> {
    return this.http.get<Division[]>(
      `${this.baseUrl}/api/v1/integracion/commons/countries/${isoCode}/divisions`
    );
  }

  getPathDivision(code: string): Observable<LevelDivision[]> {
    return this.http.get<LevelDivision[]>(
      `${this.baseUrl}/api/v1/integracion/commons/countries/division/${code}/path`
    );
  }
}
