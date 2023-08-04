import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Criteria } from '@cal/criteria';
import { PagedListResponseMessage } from '@cal/response-message';
import { keys } from '@src/keys';
import { BehaviorSubject, Observable } from 'rxjs';
import {
  DocumentoRequisitoDto,
  DocumentoRequisitoMasivoModel,
  DocumentoRequisitoModel,
  EditRequisitoModel,
  FileTypeAcademicoModel,
  RequisitoDto,
  RequisitoExpedienteDto,
  RequisitoExpedienteMasivoDto,
  RequisitoModel
} from './requesitos.models';

@Injectable({
  providedIn: 'root'
})
export class RequisitosService {
  private readonly URL = `${keys.API_BASE_URL}/api/v1`;
  public hasDocumentos: boolean;
  public documentacionProtegida = false;
  private documentacionProtegida$ = new BehaviorSubject<boolean>(
    this.documentacionProtegida
  );

  constructor(private http: HttpClient) {}

  get documentacionProtegidaChanched$(): Observable<boolean> {
    return this.documentacionProtegida$.asObservable();
  }

  setDocumentacionProtegida(documentacionProtegida: boolean): void {
    this.documentacionProtegida$.next(documentacionProtegida);
    this.documentacionProtegida = documentacionProtegida;
  }

  getRequisitosExpedientes(
    criteria: Criteria
  ): Observable<PagedListResponseMessage<RequisitoExpedienteDto>> {
    return this.http.post<PagedListResponseMessage<RequisitoExpedienteDto>>(
      `${this.URL}/requisitos-expedientes/query`,
      criteria
    );
  }

  deleteRequisito(id: number): Observable<void> {
    return this.http.delete<void>(`${this.URL}/requisitos-expedientes/${id}`);
  }

  deleteRequisitoMasivo(
    requisitosExpedientes: RequisitoExpedienteMasivoDto
  ): Observable<string[]> {
    return this.http.post<string[]>(
      `${this.URL}/requisitos-expedientes/delete-massive`,
      requisitosExpedientes
    );
  }

  addRequisito(payload: RequisitoModel): Observable<number> {
    return this.http.post<number>(
      `${this.URL}/requisitos-expedientes`,
      payload
    );
  }

  getRequisito(id: number): Observable<RequisitoDto> {
    return this.http.get<RequisitoDto>(
      `${this.URL}/requisitos-expedientes/${id}`
    );
  }

  updateRequisito(payload: EditRequisitoModel): Observable<void> {
    return this.http.put<void>(`${this.URL}/requisitos-expedientes`, payload);
  }

  deleteDocumentoRequisito(id: number): Observable<void> {
    return this.http.delete<void>(
      `${this.URL}/requisito-expediente-documento/${id}`
    );
  }

  getDocumentosRequisito(id: number): Observable<DocumentoRequisitoDto[]> {
    return this.http.get<DocumentoRequisitoDto[]>(
      `${this.URL}/requisito-expediente-documento/${id}`
    );
  }

  addDocumentosRequisito(payload: DocumentoRequisitoModel): Observable<void> {
    return this.http.post<void>(
      `${this.URL}/requisito-expediente-documento`,
      payload
    );
  }

  updateDocumentosRequisito(
    payload: DocumentoRequisitoModel
  ): Observable<void> {
    return this.http.put<void>(
      `${this.URL}/requisito-expediente-documento`,
      payload
    );
  }

  deleteDocumentosRequisito(
    documentosRequisitoExpediente: DocumentoRequisitoMasivoModel
  ): Observable<string[]> {
    return this.http.post<string[]>(
      `${this.URL}/requisito-expediente-documento/delete-massive`,
      documentosRequisitoExpediente
    );
  }

  getFilesTypes(): Observable<FileTypeAcademicoModel[]> {
    return this.http.get<FileTypeAcademicoModel[]>(
      `${this.URL}/integracion/erp-academico/files-types`
    );
  }
}
