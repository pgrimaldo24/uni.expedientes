import { HttpClient, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ConsolidacionRequisitoExpedienteDto } from '@pages/expediente-alumno/expediente-models';
import { keys } from '@src/keys';
import { Observable, Subject } from 'rxjs';
import {
  ConsolidacionRequisitoExpedienteDocumentoDto,
  ConsolidacionRequisitoExpedienteModel
} from '../consolidar-requisito.models';

@Injectable({
  providedIn: 'root'
})
export class ConsolidarRequisitoService {
  private urlExpedientesAlumnos: string;
  private urlConsolidacionesRquisitosExpedientes: string;
  private urlConsolidacionesRquisitosExpedientesDocumentos: string;
  public consolidacionSubject: Subject<ConsolidacionRequisitoExpedienteDto>;
  constructor(private http: HttpClient) {
    this.http = http;
    this.urlExpedientesAlumnos = `${keys.API_BASE_URL}/api/v1/expedientes-alumnos`;
    this.urlConsolidacionesRquisitosExpedientes = `${keys.API_BASE_URL}/api/v1/consolidaciones-requisitos-expedientes`;
    this.urlConsolidacionesRquisitosExpedientesDocumentos = `${keys.API_BASE_URL}/api/v1/consolidaciones-requisitos-expedientes-documentos`;
    this.consolidacionSubject = new Subject<ConsolidacionRequisitoExpedienteDto>();
  }

  loadConsolidacion(consolidacion: ConsolidacionRequisitoExpedienteDto): void {
    this.consolidacionSubject.next(consolidacion);
  }

  uploadFileConsolidacionRequisitoDocumento(
    id: number,
    formData: FormData
  ): Observable<number> {
    return this.http.post<number>(
      `${this.urlConsolidacionesRquisitosExpedientesDocumentos}/upload-file/consolidacion-requisito-expediente/${id}`,
      formData
    );
  }

  getConsolidacionRequisitoDocumentosByIdConsolidacion(
    id: number
  ): Observable<ConsolidacionRequisitoExpedienteDocumentoDto[]> {
    return this.http.get<ConsolidacionRequisitoExpedienteDocumentoDto[]>(
      `${this.urlConsolidacionesRquisitosExpedientesDocumentos}/consolidacion-requisito-expediente/${id}`
    );
  }

  deleteConsolidacionRequisitoDocumentosById(id: number): Observable<void> {
    return this.http.delete<void>(
      `${this.urlConsolidacionesRquisitosExpedientesDocumentos}/${id}`
    );
  }

  updateConsolidacionRequisitoDocumentosById(
    payload: ConsolidacionRequisitoExpedienteDocumentoDto
  ): Observable<void> {
    return this.http.put<void>(
      `${this.urlConsolidacionesRquisitosExpedientesDocumentos}/${payload.id}`,
      payload
    );
  }

  downloadFicheroConsolidacionRequisitoDocumento(
    id: number
  ): Observable<HttpResponse<Blob>> {
    return this.http.get(
      `${this.urlConsolidacionesRquisitosExpedientesDocumentos}/${id}/download`,
      {
        observe: 'response',
        responseType: 'blob'
      }
    );
  }

  updateConsolidacionRequisitoExpediente(
    payload: ConsolidacionRequisitoExpedienteModel
  ): Observable<void> {
    return this.http.put<void>(
      `${this.urlConsolidacionesRquisitosExpedientes}/${payload.id}`,
      payload
    );
  }

  createConsolidacionRequisitoExpediente(
    payload: ConsolidacionRequisitoExpedienteModel
  ): Observable<number> {
    return this.http.post<number>(
      this.urlConsolidacionesRquisitosExpedientes,
      payload
    );
  }

  deleteConsolidacionRequisito(id: number): Observable<void> {
    return this.http.delete<void>(
      `${this.urlConsolidacionesRquisitosExpedientes}/${id}`
    );
  }

  rechazarConsolidacionRequisitoExpediente(
    payload: ConsolidacionRequisitoExpedienteModel
  ): Observable<void> {
    return this.http.put<void>(
      `${this.urlConsolidacionesRquisitosExpedientes}/${payload.id}/rechazar`,
      payload
    );
  }

  validarConsolidacionRequisitoExpediente(
    payload: ConsolidacionRequisitoExpedienteModel
  ): Observable<void> {
    return this.http.put<void>(
      `${this.urlConsolidacionesRquisitosExpedientes}/${payload.id}/validar`,
      payload
    );
  }

  getConsolidacionRequisitoExpediente(
    id: number
  ): Observable<ConsolidacionRequisitoExpedienteDto> {
    return this.http.get<ConsolidacionRequisitoExpedienteDto>(
      `${this.urlConsolidacionesRquisitosExpedientes}/${id}`
    );
  }

  getConsolidacionesRequisitosByIdExpediente(
    idExpediente: number
  ): Observable<ConsolidacionRequisitoExpedienteDto[]> {
    return this.http.get<ConsolidacionRequisitoExpedienteDto[]>(
      `${this.urlConsolidacionesRquisitosExpedientes}/expediente/${idExpediente}`
    );
  }

  createConsolidacionesRequisitosExpediente(
    idExpediente: number
  ): Observable<void> {
    return this.http.post<void>(
      `${this.urlExpedientesAlumnos}/${idExpediente}/consolidar-requisitos`,
      {}
    );
  }
}
