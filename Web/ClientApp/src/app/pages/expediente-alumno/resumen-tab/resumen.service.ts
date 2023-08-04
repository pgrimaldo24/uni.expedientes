import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { keys } from '@src/keys';
import {
  HitoConseguidoDto,
  TipoSituacionEstadoExpedienteDto
} from './resumen.models';

@Injectable({
  providedIn: 'root'
})
export class ResumenService {
  private urlTiposSituacionEstado: string;
  private urlHitosConseguidos: string;

  constructor(private http: HttpClient) {
    this.http = http;
    this.urlTiposSituacionEstado = `${keys.API_BASE_URL}/api/v1/tipos-situacion-estado-expedientes`;
    this.urlHitosConseguidos = `${keys.API_BASE_URL}/api/v1/hitos-conseguidos`;
  }

  getUltimoTipoSituacionEstadoByIdExpediente(
    idExpedienteAlumno: number
  ): Observable<TipoSituacionEstadoExpedienteDto> {
    return this.http.get<TipoSituacionEstadoExpedienteDto>(
      `${this.urlTiposSituacionEstado}/expediente/${idExpedienteAlumno}`
    );
  }

  getHitosConseguidosByIdExpediente(
    idExpedienteAlumno: number
  ): Observable<HitoConseguidoDto[]> {
    return this.http.get<HitoConseguidoDto[]>(
      `${this.urlHitosConseguidos}/expediente/${idExpedienteAlumno}`
    );
  }
}
