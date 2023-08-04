export enum FormNivelesUsoCtrls {
  tipoNivelUsoCombo = 'tipoNivelUsoCombo',
  universidadCombo = 'universidadCombo',
  tipoEstudioCombo = 'tipoEstudioCombo',
  estudioCombo = 'estudioCombo',
  planEstudioCombo = 'planEstudioCombo',
  tipoAsignaturaCombo = 'tipoAsignaturaCombo',
  asignaturaPlanCombo = 'asignaturaPlanCombo',
  idTipoNivelUso = 'idTipoNivelUso'
}

export interface CreateNivelUsoPayload {
  idTipoNivelUso: number;
  idRefUniversidad: string;
  idRefTipoEstudio: string;
  idRefEstudio: string;
  idRefPlanEstudio: string;
  idRefTipoAsignatura: string;
  idRefAsignatura: string;
  nombreNivelUso: string;
  acronimoUniversidad: string;
  nombreTipoEstudio: string;
  nombreEstudio: string;
  nombrePlan: string;
  nombreTipoAsignatura: string;
  nombreAsignatura: string;
  esUniversidad: boolean;
  esTipoEstudio: boolean;
  esEstudio: boolean;
  esPlanEstudio: boolean;
  esTipoAsignatura: boolean;
  esAsignaturaPlan: boolean;
}

export interface TipoNivelUsoListItemDto {
  id: number;
  nombre: string;
  orden: number;
  esUniversidad: boolean;
  esTipoEstudio: boolean;
  esEstudio: boolean;
  esPlanEstudio: boolean;
  esTipoAsignatura: boolean;
  esAsignaturaPlan: boolean;
}

export interface UniversidadSearchParameters {
  filterNombre: string;
  search: string;
  index: number;
  count: number;
}

export interface Payload {
  filterIdUniversidad?: string;
  search: string;
  index: string;
  count: string;
}

export interface UniversidadModel {
  codigoOficial: string;
  id: number;
  displayName: string;
  acronimo: string;
  idIntegracion: string;
}

export interface TipoEstudioModel {
  id: number;
  codigo: string;
  nombre: string;
  displayName: string;
  displayNameNivelUso: string;
}

export interface EstudioModel {
  id: number;
  codigo: string;
  nombre: string;
  displayName: string;
}

export interface PlanEstudioModel {
  id: number;
  codigo: string;
  nombre: string;
  displayName: string;
}

export interface TipoAsignaturaModel {
  id: number;
  orden: number;
  nombre: string;
}

export interface AsignaturaPlanModel {
  id: number;
  displayName: string;
  displayNameNivelUso: string;
}

export interface DeleteNivelesUsoByTipoPayload {
  idVersionEscala: number;
  idTipoNivelUso: number;
}
