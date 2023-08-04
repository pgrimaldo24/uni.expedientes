import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, Subject } from 'rxjs';
import { keys } from '@src/keys';
import { Criteria } from '@cal/criteria';
import {
  AlumnoInfoDto,
  AlumnoMatriculasParameters,
  AnotacionDto,
  DatosErpResponse,
  EditExpedienteAlumno,
  ExpedienteAlumnoDto,
  ExpedienteEspecializacionDto,
  MatriculaErpAcademicoModel,
  QueryResponse,
  SolicitudDto,
  Universities,
  UniversityBindingType
} from '@pages/expediente-alumno/expediente-models';
import { Observacion } from './observacion-form/observacion-form.models';
import { PagedListResponseMessage } from '@cal/response-message';
import { ConfiguracionExpedienteUniversidadDto } from './requisitos-tab/consolidar-requisito.models';
import { GrafoDto, RequestNodoPlan } from './resumen-tab/resumen.models';
import { GenerateSolicitudTitulo } from './show-generate-solicitud-titulo/show-generate-solicitud-titulo.models';

@Injectable({
  providedIn: 'root'
})
export class ExpedienteService {
  private baseUrlExpedientesAlumnos: string;
  private baseUrlIntegracion: string;
  private baseUrlAnotaciones: string;
  public hasInfoGrafo$ = new Subject<GrafoDto>();
  constructor(private http: HttpClient) {
    this.http = http;
    this.baseUrlExpedientesAlumnos = `${keys.API_BASE_URL}/api/v1/expedientes-alumnos`;
    this.baseUrlIntegracion = `${keys.API_BASE_URL}/api/v1/integracion`;
    this.baseUrlAnotaciones = `${keys.API_BASE_URL}/api/v1/anotaciones`;
  }

  //#region EXPEDIENTES
  advancedSearch(criteria: Criteria): Observable<QueryResponse> {
    return this.http.post<QueryResponse>(
      `${this.baseUrlExpedientesAlumnos}/query`,
      criteria
    );
  }

  getAlumno(idRefIntegracionAlumno: number): Observable<AlumnoInfoDto> {
    return this.http.get<AlumnoInfoDto>(
      `${this.baseUrlExpedientesAlumnos}/alumno/${idRefIntegracionAlumno}`
    );
  }

  getDatosErpAlumno(id: number): Observable<DatosErpResponse> {
    return this.http.get<DatosErpResponse>(
      `${this.baseUrlExpedientesAlumnos}/${id}/datos-erp`
    );
  }

  getAlumnoPuedeTitularseEnPlan(criteria: Criteria): any {
    return this.http.post<QueryResponse>(
      `${this.baseUrlExpedientesAlumnos}/alumno-puede-titularse-en-plan`,
      criteria
    );
  }

  updateExpediente(payload: EditExpedienteAlumno): Observable<unknown> {
    return this.http.put(
      `${this.baseUrlExpedientesAlumnos}/${payload.idExpedienteAlumno}`,
      payload
    );
  }

  tieneExpedienteBloqueado(id: number): Observable<boolean> {
    return this.http.get<boolean>(
      `${this.baseUrlExpedientesAlumnos}/${id}/tiene-expediente-bloqueado`
    );
  }

  getExpedienteAlumno(id: number): Observable<ExpedienteAlumnoDto> {
    return this.http.get<ExpedienteAlumnoDto>(
      `${this.baseUrlExpedientesAlumnos}/${id}`
    );
  }

  getConfiguracionExpedienteUniversidad(
    id: number
  ): Observable<ConfiguracionExpedienteUniversidadDto> {
    return this.http.get<ConfiguracionExpedienteUniversidadDto>(
      `${this.baseUrlExpedientesAlumnos}/${id}/configuracion-universidad`
    );
  }

  getEspecializacionesByIdExpediente(
    id: number
  ): Observable<ExpedienteEspecializacionDto[]> {
    return this.http.get<ExpedienteEspecializacionDto[]>(
      `${this.baseUrlExpedientesAlumnos}/${id}/especializaciones`
    );
  }
  //#endregion

  //#region INTEGRACIÓN
  matriculasAdvancedSearch(
    parameters: AlumnoMatriculasParameters
  ): Observable<MatriculaErpAcademicoModel[]> {
    return this.http.post<MatriculaErpAcademicoModel[]>(
      `${this.baseUrlIntegracion}/erp-academico/alumno/${parameters.idIntegracionAlumno}/matriculas`,
      parameters
    );
  }

  getStudentUniversitesBindingTypes(
    code: string
  ): Observable<UniversityBindingType[]> {
    return this.http.get<UniversityBindingType[]>(
      `${this.baseUrlIntegracion}/commons/${code}/student-university-binding-types`
    );
  }

  getCertificados(
    idRefIntegracionAlumno: string,
    idRefPlan: string
  ): Observable<SolicitudDto[]> {
    return this.http.get<SolicitudDto[]>(
      `${this.baseUrlIntegracion}/expedicion-titulos/estados-solicitudes?idIntegracionAlumno=${idRefIntegracionAlumno}&idPlan=${idRefPlan}&excluirCanceladas=true`
    );
  }

  getUniversities(code: string): Observable<Universities> {
    return this.http.get<Universities>(
      `${this.baseUrlIntegracion}/commons/universities/${code}`
    );
  }

  getGrafoPlan(request: RequestNodoPlan): Observable<GrafoDto> {
    return this.http.post<GrafoDto>(
      `${this.baseUrlIntegracion}/erp-academico/grafo-plan`,
      request
    );
  }

  generateSolicitudTituloCertificado(
    request: GenerateSolicitudTitulo
  ): Observable<void> {
    return this.http.post<void>(
      `${this.baseUrlIntegracion}/expedicion-titulos/generar-solicitud-certificado`,
      request
    );
  }

  generateSolicitudTituloCertificadoMasivo(
    request: GenerateSolicitudTitulo
  ): Observable<void> {
    return this.http.post<void>(
      `${this.baseUrlIntegracion}/expedicion-titulos/generar-solicitud-certificado-masivo`,
      request
    );
  }

  //#endregion

  //#region ANOTACIONES
  getAnotaciones(
    criteria: Criteria
  ): Observable<PagedListResponseMessage<AnotacionDto>> {
    return this.http.post<PagedListResponseMessage<AnotacionDto>>(
      `${this.baseUrlAnotaciones}/query`,
      criteria
    );
  }

  saveAnotacion(payload: Observacion): Observable<void> {
    return this.http.post<void>(`${this.baseUrlAnotaciones}`, payload);
  }

  getAnotacionById(id: number): Observable<AnotacionDto> {
    return this.http.get<AnotacionDto>(`${this.baseUrlAnotaciones}/${id}`);
  }

  updateAnotacion(payload: Observacion): Observable<void> {
    return this.http.put<void>(`${this.baseUrlAnotaciones}`, payload);
  }

  deleteAnotacion(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrlAnotaciones}/${id}`);
  }
  //#endregion

  hasInfoGrafo(grafoPlan: GrafoDto): void {
    this.hasInfoGrafo$.next(grafoPlan);
  }
}
