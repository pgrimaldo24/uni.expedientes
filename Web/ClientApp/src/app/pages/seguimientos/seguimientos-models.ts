import { VersionesPlane } from '@pages/expediente-alumno/expediente-models';

export interface AlumnoComboResponse {
  id: number;
  idIntegracion: string;
  displayName: string;
}

export interface TrabajadorAcademicoResponse {
  id: number;
  persona: PersonaAcademicoResponse;
  displayName: string;
}

export interface PersonaAcademicoResponse {
  id: number;
  nombre: string;
  apellido1: string;
  apellido2: string;
  email: string;
  idGestorCepal: string;
  idSeguridad: string;
  displayName: string;
}

export interface EstudioCombo {
  id: number;
  nombre: string;
  displayName: string;
}

export interface TituloCombo {
  id: number;
  codigo: string;
  nombre: string;
  displayName: string;
}

export interface PlanDeEstudioComboResponse {
  id: number;
  codigo: string;
  nombre: string;
  version: string;
  displayName: string;
  estudio: EstudioCombo;
  titulo: TituloCombo;
  versionesPlanes: VersionesPlane[];
}

export interface Meta {
  totalCount: number;
}

export interface TipoSeguimiento {
  id: number;
  nombre: string;
}

export interface ExpedienteAlumno {
  id: number;
  idRefIntegracionAlumno: string;
  idRefPlan: string;
  planEstudioDisplayName: string;
  alumnoDisplayName: string;
  nombrePlan: string;
}

export interface Data {
  id: number;
  fecha: string;
  descripcion: string;
  nombreTrabajador: string;
  tipoSeguimiento: TipoSeguimiento;
  expedienteAlumno: ExpedienteAlumno;
  idRefTrabajador: string;
}

export interface QueryResponse {
  meta: Meta;
  data: Data[];
}

export interface Where {
  op: string;
  field: string;
  value: string;
  clauses: string[];
}

export interface BodyQuery {
  count: number;
  order: string;
  offset: number;
  limit: number;
  groupBy: string;
  where: Where[];
  distinct: boolean;
  projection: string;
}

export interface UniversidadModel {
  id?: number;
  nombre?: string;
  idIntegracion?: string;
}
