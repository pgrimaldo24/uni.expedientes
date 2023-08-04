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

export interface Tipo {
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
  tipo: Tipo;
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

export interface DocumentoAlumnoModel {
  id: number;
  emailAlumno: string;
  displayName: string;
  documentosAcademicos: DocumentoAcademicoModel[];
}

export interface DocumentoAcademicoModel {
  idPlan: number;
  displayNamePlan: string;
  periodoAcademicoMatriculas: PeriodoAcademicoMatriculaModel[];
  expediente: ExpedienteAcademicoModel;
}

export interface PeriodoAcademicoMatriculaModel {
  idPeriodoAcademico: number;
  displayNamePeriodoAcademico: string;
  matriculas: MatriculaModel[];
}

export interface MatriculaModel {
  idMatricula: number;
  displayNameMatricula: string;
  condicionesConsolidadas: CondicionConsolidadaModel[];
}

export interface CondicionConsolidadaModel {
  id: number;
  isEstadoNoProcesada: boolean;
  isEstadoValidada: boolean;
  isEstadoPendiente: boolean;
  isEstadoRechazada: boolean;
  condicionMatricula: CondicionMatriculaModel;
}

export interface CondicionMatriculaModel {
  id: number;
  nombre: string;
  displayName: string;
}

export interface ExpedienteAcademicoModel {
  id: number;
  idIntegracion: string;
  fechaApertura: string | null;
  displayName: string;
  idRefTipoVinculacion: string;
  idRefVersionPlan: string;
  idRefIntegracionAlumno: string;
  idRefPlan: string;
  viaAccesoPlan: ViaAccesoPlanAcademicoModel;
}

export interface ViaAccesoPlanAcademicoModel {
  id: number;
  viaAcceso: ViaAccesoAcademicoModel;
}

export interface ViaAccesoAcademicoModel {
  id: number;
  nombre: string;
  descripcion: string;
  esVigente: boolean;
  displayNameSuperViaAcceso: string;
  hasSuperViaAcceso: boolean;
  displayNameFromSuperViaAcceso: string;
  displayNameClasificacionSuperViaAcceso: string;
}

export enum EntityType {
  FOLDER = 'folder',
  FILE = 'file',
  ROOT = 'root'
}

export class DeudaClienteParameters {
  idIntegracionUniversidad: string;
  idIntegracionAlumno: string;
  constructor(idIntegracionUniversidad: string, idIntegracionAlumno: string) {
    this.idIntegracionAlumno = idIntegracionAlumno;
    this.idIntegracionUniversidad = idIntegracionUniversidad;
  }
}

export interface DeudaClienteModel {
  idCliente: string;
  nombre: string;
  codigoPais: string;
  fechaSolicitud: string;
  saldo: number;
  saldoVencidoFecha: number;
  divisaEmpresa: string;
  facturas: FacturaModel[];
}

export interface FacturaModel {
  numeroPedido: string;
  tipoDocumento: string;
  numeroDocumento: string;
  fechaRegistro: string;
  fechaRegistroIso: Date;
  idEntidadOperacion: string;
  importe: number;
  importeSimbolo: string;
  importePendiente: number;
  importePendienteSimbolo: string;
  terminoPago: string;
  metodoPago: string;
  recibos: ReciboModel[];
}

export interface ReciboModel {
  tipoDocumento: string;
  importe: number;
  importePendiente: number;
}
