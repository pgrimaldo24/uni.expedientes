import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, Subject } from 'rxjs';
import { keys } from '@src/keys';
import {
  AsignaturaExpedienteDto,
  ReconocimientoClasificacionDto,
  RequerimientoPlanDto,
  RequestRequerimientoPlan,
  TrayectoPlanDto,
  TrayectoSeleccionado
} from './calificaciones-model';

@Injectable({
  providedIn: 'root'
})
export class CalificacionesService {
  private urlIntegracion: string;
  private urlAsignaturaExpediente: string;
  public changeTrayectoCalificaciones$ = new Subject<TrayectoSeleccionado>();
  public changeTrayectoResumen$ = new Subject<TrayectoSeleccionado>();

  constructor(private http: HttpClient) {
    this.http = http;
    this.urlIntegracion = `${keys.API_BASE_URL}/api/v1/integracion`;
    this.urlAsignaturaExpediente = `${keys.API_BASE_URL}/api/v1/asignaturas-expedientes`;
  }

  changeTrayectoCalificaciones(trayecto: TrayectoSeleccionado): void {
    this.changeTrayectoCalificaciones$.next(trayecto);
  }

  changeTrayectoResumen(trayecto: TrayectoSeleccionado): void {
    this.changeTrayectoResumen$.next(trayecto);
  }

  getRequerimientosPlan(
    request: RequestRequerimientoPlan
  ): Observable<RequerimientoPlanDto[]> {
    return this.http.post<RequerimientoPlanDto[]>(
      `${this.urlIntegracion}/erp-academico/plan-requerimientos`,
      request
    );
  }

  getTrayectosRequerimientosPlan(
    request: RequestRequerimientoPlan
  ): Observable<TrayectoPlanDto[]> {
    return this.http.post<TrayectoPlanDto[]>(
      `${this.urlIntegracion}/erp-academico/trayectos-requerimientos-plan`,
      request
    );
  }

  getAsignaturasByIdExpediente(
    idExpedienteAlumno: number
  ): Observable<AsignaturaExpedienteDto[]> {
    return this.http.get<AsignaturaExpedienteDto[]>(
      `${this.urlAsignaturaExpediente}/expediente/${idExpedienteAlumno}`
    );
  }

  getAsignaturasReconocimientos(
    idIntegracionAlumno: string,
    idRefPlan: string,
    idRefVersionPlan: string
  ): Observable<ReconocimientoClasificacionDto> {
    return this.http.get<ReconocimientoClasificacionDto>(
      `${this.urlAsignaturaExpediente}/reconocimientos?idIntegracionAlumno=${idIntegracionAlumno}&idRefPlan=${idRefPlan}&idRefVersionPlan=${idRefVersionPlan}`
    );
  }
}
