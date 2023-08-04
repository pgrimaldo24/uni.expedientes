import { DataRow } from '@src/app/component-tools/data-table/models';
import { AsignaturaDto } from '../resumen-tab/resumen.models';

export enum FormCalificacionesControls {
  trayecto = 'trayecto'
}

export enum IdTipoAsignaturaReconocimiento {
  Transversal = -1,
  Seminario = -2,
  Universitaria = -3
}

export interface RequerimientoPlanDto {
  id?: number;
  nombre?: string;
  creditosRequeridos: number;
  tiposAsignaturasRequerimiento: TipoAsignaturaRequerimientoDto[];
  trayectosPlanes?: TrayectoPlanDto[];
  esGenerico?: boolean;
  creditosObtenidos: number;
}

export interface TipoAsignaturaRequerimientoDto {
  id?: number;
  minCreditos: number;
  maxCreditos?: number;
  tipoAsignatura: TipoAsignaturaDto;
  creditosAsignaturas?: number;
  rowsBloques?: DataRow[];
}

export interface TipoAsignaturaDto {
  id: number;
  nombre: string;
  esReconocimiento: boolean;
}

export interface TrayectoPlanDto {
  id: number;
  esGenerico: boolean;
  nodoInicial: NodoInicialDto;
  nodoFinal: NodoFinalDto;
}

export interface NodoInicialDto {
  id: number;
  nombre: string;
}

export interface NodoFinalDto {
  id: number;
  nombre: string;
  nombreNodo: string;
  hitos: HitosDto[];
  tiposAsignaturasRequerimiento: TipoAsignaturaRequerimientoDto[];
}

export interface HitosDto {
  id: number;
  nombre: string;
  hitoEspecializacion: HitoEspecializacionDto;
  hitoTitulo: HitoTituloDto;
}

export interface HitoEspecializacionDto {
  id: number;
  especializacion: EspecializacionDto;
}

export interface EspecializacionDto {
  id: number;
  codigo: string;
  nombre: string;
}

export interface HitoTituloDto {
  id: number;
  titulo: TituloDto;
}

export interface TituloDto {
  id: number;
  codigo: string;
  nombre: string;
}

export interface AsignaturaExpedienteDto {
  id?: number;
  idRefAsignaturaPlan?: string;
  nombreAsignatura: string;
  codigoAsignatura?: string;
  idRefTipoAsignatura?: string;
  ects: number;
  calificacion?: string;
  anyoAcademicoInicio?: number;
  anyoAcademicoFin?: number;
  idRefCurso?: string;
  numeroCurso?: number;
  duracionPeriodo?: string;
  simboloIdiomaImparticion?: string;
  asignaturasCalificaciones?: AsignaturaCalificacionDto[];
  notFound?: boolean;
  esReconocimientoTransversal?: boolean;
}

export class RequestRequerimientoPlan {
  filterIdPlan: number;
  searchText: string;
  index: number;
  count: number;
}

export interface ArcoSalienteDto {
  creditosObtenidos: number;
  creditosRequeridos: number;
  anyAsignaturaMatriculada: boolean;
  bloques: BloqueArcoDto[];
}

export interface BloqueArcoDto {
  nombre: string;
  asignaturas: AsignaturaDto[];
  tiposAsignaturas: TipoAsignaturaRequerimientoDto[];
  subBloques: BloqueArcoDto[];
}

export interface TrayectoSeleccionado {
  requerimientoPlan: RequerimientoPlanDto;
  asignaturasTrayecto: AsignaturaDto[];
  asignaturasExpediente: AsignaturaExpedienteDto[];
  idsNodosTrayectos: number[];
}

export interface ReconocimientoClasificacionDto {
  asignaturasReconocimientos: AsignaturaReconocimientoDto[];
  mensajesError: string[];
}

export interface AsignaturaReconocimientoDto {
  idAsignaturaPlanErp?: number;
  nombreAsignatura: string;
  codigoAsignatura: string;
  idRefTipoAsignatura: string;
  nombreTipoAsignatura: string;
  calificacionAsignaturaErp: string;
  calificacion: string;
  ects: number;
  esExtensionUniversitaria: boolean;
  esTransversal: boolean;
  esAsignatura: boolean;
  esSeminario: boolean;
  esTransversalPrincipal: boolean;
}

export interface AsignaturaCalificacionDto {
  id: number;
  ciclo: string;
  anyoAcademico: string;
  calificacion: number;
  nombreCalificacion: string;
}
