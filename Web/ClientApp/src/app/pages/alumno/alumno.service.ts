import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { keys } from '@src/keys';
import { Criteria } from '@cal/criteria';
import {
  DeudaClienteModel,
  DeudaClienteParameters,
  DocumentoAlumnoModel,
  QueryResponse
} from './alumno-models';
import { AlumnoInfoDto } from '@pages/expediente-alumno/expediente-models';

@Injectable({
  providedIn: 'root'
})
export class AlumnoService {
  private baseUrl: string;
  public alumnoInfoDto: AlumnoInfoDto;
  public idsExpedientes: number[] = [];
  public idsMatriculas: number[] = [];
  public flat = false;
  private flat$ = new BehaviorSubject<boolean>(this.flat);
  public flatExpediente$ = new Subject();

  constructor(private http: HttpClient) {
    this.http = http;
    this.baseUrl = keys.API_BASE_URL;
  }

  get flatChanched$(): Observable<boolean> {
    return this.flat$.asObservable();
  }

  setFlat(flat: boolean): void {
    this.flat$.next(flat);
    this.flat = flat;
  }

  setRowExpediente(): void {
    this.flatExpediente$.next();
  }

  advancedSearch(criteria: Criteria): Observable<QueryResponse> {
    return this.http.post<QueryResponse>(
      `${this.baseUrl}/api/v1/seguimientos-expedientes/query`,
      criteria
    );
  }

  getDocumentosAlumno(idAlumno: number): Observable<DocumentoAlumnoModel> {
    return this.http.get<DocumentoAlumnoModel>(
      `${this.baseUrl}/api/v1/integracion/erp-academico/alumno/${idAlumno}/documentos`
    );
  }

  getSaldosAlumno(
    payload: DeudaClienteParameters
  ): Observable<DeudaClienteModel> {
    return this.http.post<DeudaClienteModel>(
      `${this.baseUrl}/api/v1/integracion/financiero/deuda-cliente`,
      payload
    );
  }
}
