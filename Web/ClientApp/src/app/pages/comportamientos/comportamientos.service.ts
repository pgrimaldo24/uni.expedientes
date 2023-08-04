import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Criteria } from '@cal/criteria';
import { PagedListResponseMessage } from '@cal/response-message';
import { keys } from '@src/keys';
import { Observable } from 'rxjs';
import {
  ComportamientoExpedienteDto,
  ComportamientoExpedienteMasivoDto,
  RequisitoComportamientoExpedienteMasivoDto,
  EditComportamientoExpedienteDto,
  EditRequisitoComportamientoExpedienteDto,
  CreateRequisitoComportamientoExpedienteDto,
  CreateNivelUsoComportamientoExpedienteDto,
  GetNivelUsoComportamientoExpedienteDto
} from './comportamientos.models';

@Injectable({
  providedIn: 'root'
})
export class ComportamientosService {
  private readonly URL = `${keys.API_BASE_URL}/api/v1`;

  constructor(private http: HttpClient) {}

  getComportamientosExpedientes(
    criteria: Criteria
  ): Observable<PagedListResponseMessage<ComportamientoExpedienteDto>> {
    return this.http.post<
      PagedListResponseMessage<ComportamientoExpedienteDto>
    >(`${this.URL}/comportamientos-expedientes/query`, criteria);
  }

  deleteComportamiento(id: number): Observable<void> {
    return this.http.delete<void>(
      `${this.URL}/comportamientos-expedientes/${id}`
    );
  }

  deleteComportamientoMasivo(
    comportamientosExpedientes: ComportamientoExpedienteMasivoDto
  ): Observable<string[]> {
    return this.http.post<string[]>(
      `${this.URL}/comportamientos-expedientes/delete-massive`,
      comportamientosExpedientes
    );
  }

  getComportamiento(id: number): Observable<ComportamientoExpedienteDto> {
    return this.http.get<ComportamientoExpedienteDto>(
      `${this.URL}/comportamientos-expedientes/${id}`
    );
  }

  addComportamiento(payload: ComportamientoExpedienteDto): Observable<number> {
    return this.http.post<number>(
      `${this.URL}/comportamientos-expedientes`,
      payload
    );
  }

  updateComportamiento(
    payload: EditComportamientoExpedienteDto
  ): Observable<unknown> {
    return this.http.put(
      `${this.URL}/comportamientos-expedientes/${payload.id}`,
      payload
    );
  }

  deleteRequisitoComportamiento(id: number): Observable<void> {
    return this.http.delete<void>(
      `${this.URL}/comportamientos-expedientes/${id}/requisito-comportamiento`
    );
  }

  deleteRequisitoComportamientoMasivo(
    comportamientosExpedientes: RequisitoComportamientoExpedienteMasivoDto
  ): Observable<string[]> {
    return this.http.post<string[]>(
      `${this.URL}/comportamientos-expedientes/requisito-comportamiento-massive`,
      comportamientosExpedientes
    );
  }

  updateRequisitoComportamiento(
    payload: EditRequisitoComportamientoExpedienteDto
  ): Observable<unknown> {
    return this.http.put(
      `${this.URL}/comportamientos-expedientes/requisito-comportamiento/${payload.idRequisitoComportamiento}`,
      payload
    );
  }

  createRequisitoComportamiento(
    payload: CreateRequisitoComportamientoExpedienteDto
  ): Observable<unknown> {
    return this.http.post(
      `${this.URL}/comportamientos-expedientes/${payload.idComportamiento}/requisito-comportamiento`,
      payload
    );
  }

  createNivelUsoComportamiento(
    payload: CreateNivelUsoComportamientoExpedienteDto
  ): Observable<unknown> {
    return this.http.post(
      `${this.URL}/comportamientos-expedientes/${payload.idComportamiento}/nivel-uso-comportamiento`,
      payload
    );
  }

  deleteNivelUsoComportamiento(id: number): Observable<void> {
    return this.http.delete<void>(
      `${this.URL}/comportamientos-expedientes/${id}/nivel-uso-comportamiento`
    );
  }

  getNivelUsoDisplayNameComportamiento(
    payload: GetNivelUsoComportamientoExpedienteDto
  ): Observable<string> {
    return this.http.post(
      `${this.URL}/comportamientos-expedientes/nivel-uso-comportamiento/display-name`,
      payload,
      { responseType: 'text' }
    );
  }
}
