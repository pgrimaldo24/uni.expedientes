import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { keys } from '@src/keys';
import { Criteria } from '@cal/criteria';
import { QueryResponse } from './seguimientos-models';

@Injectable({
  providedIn: 'root'
})
export class SeguimientosService {
  private baseUrl: string;

  constructor(private http: HttpClient) {
    this.http = http;
    this.baseUrl = keys.API_BASE_URL;
  }

  advancedSearch(criteria: Criteria): Observable<QueryResponse> {
    return this.http.post<QueryResponse>(
      `${this.baseUrl}/api/v1/seguimientos-expedientes/query`,
      criteria
    );
  }

  getSeguimientosByIdExpedienteAlumno(
    criteria: Criteria,
    idExpediente: number
  ): Observable<QueryResponse> {
    return this.http.post<QueryResponse>(
      `${this.baseUrl}/api/v1/seguimientos-expedientes/${idExpediente}/query`,
      criteria
    );
  }
}
