import { HitosDto } from './calificaciones-tab/calificaciones-model';

export enum FormFiltroExpedienteControls {
  idExpediente = 'idExpediente',
  universidad = 'universidad',
  planesEstudio = 'planesEstudio',
  idRefPlan = 'idRefPlan',
  nombreAlumno = 'nombreAlumno',
  primerApellido = 'primerApellido',
  segundoApellido = 'segundoApellido',
  nroDocIdentificacion = 'nroDocIdentificacion',
  idRefIntegracion = 'idRefIntegracion',
  tipoSeguimiento = 'tipoSeguimiento',
  fechaDesde = 'fechaDesde',
  fechaHasta = 'fechaHasta',
  usuario = 'usuario',
  descripcionSeguimiento = 'descripcionSeguimiento'
}

export interface AlumnoComboResponse {
  id: number;
  idIntegracion: string;
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

export interface Data {
  id: number;
  idRefIntegracionAlumno: string;
  idRefPlan: string;
  idRefNodo: string;
  alumnoDisplayName: string;
  planEstudioDisplayName: string;
  fechaApertura: Date;
  fechaFinalizacion: Date;
  universidadDisplayName: string;
  centroEstudioDisplayName: string;
  tipoEstudioDisplayName: string;
  estudioDisplayName: string;
  nombrePlan: string;
  nombreEstudio: string;
  displayNameDocumentoIdentificacionAlumno: string;
  displayNameNombreAlumno: string;
  tituloDisplayName: string;
  countSeguimientos: number;
  countAnotaciones: number;
  idUniversidad: number;
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

/////////////////////////////////////////////////////////////////////
export interface Alumno {
  id: number;
  idIntegracion: string;
  displayName: string;
}

export interface Universidad {
  id: number;
  nombre: string;
  idIntegracion: string;
  displayName: string;
}

export interface Centro {
  id: number;
  nombre: string;
  displayName: string;
  universidad: Universidad;
}

export interface AreaAcademica {
  id: number;
  displayName: string;
  centro: Centro;
}

export interface Tipo {
  id: number;
  nombre: string;
  displayName: string;
}

export interface Estudio {
  id: number;
  nombre: string;
  codigoOficial: string;
  displayName: string;
  areaAcademica: AreaAcademica;
  tipo: Tipo;
}

export interface Titulo {
  id: number;
  codigo: string;
  nombre: string;
  displayName: string;
}

export interface VersionesPlane {
  id: number;
  nro: number;
  fechaInicio: string;
}

export interface Plan {
  id: number;
  codigo: string;
  nombre: string;
  version: string;
  displayName: string;
  estudio: Estudio;
  titulo: Titulo;
  versionesPlanes: VersionesPlane[];
}

export interface ViaAcceso {
  id: number;
  nombre: string;
  esVigente: boolean;
  displayNameSuperViaAcceso: string;
  hasSuperViaAcceso: boolean;
  displayName: string;
}

export interface Tipo {
  id: number;
}

export interface Nodo {
  id: number;
  nombre: string;
  hitos: HitosDto[];
  tipo: Tipo;
  versionesPlanes: VersionesPlane[];
}

export interface ViaAccesoPlan {
  viaAcceso: ViaAcceso;
  nodo: Nodo;
}

export interface DatosErpResponse {
  id: number;
  idIntegracion: string;
  idRefIntegracionAlumno: string;
  idRefTipoVinculacion: string;
  idRefPlan: string;
  idRefVersionPlan: string;
  alumno: Alumno;
  plan: Plan;
  viaAccesoPlan: ViaAccesoPlan;
  especializaciones: ExpedienteEspecializacionDto[];
  titulacionAcceso: TitulacionAccesoDto;
}

export interface ExpedienteAlumnoDto {
  id: number;
  idRefIntegracionAlumno: string;
  idRefPlan: string;
  idRefVersionPlan: string;
  idRefNodo: string;
  alumnoNombre: string;
  alumnoApellido1: string;
  alumnoApellido2: string;
  idRefTipoDocumentoIdentificacionPais: string;
  alumnoNroDocIdentificacion: string;
  alumnoEmail: string;
  idRefViaAccesoPlan: string;
  docAcreditativoViaAcceso: string;
  idRefIntegracionDocViaAcceso: string;
  fechaSubidaDocViaAcceso: string;
  idRefTipoVinculacion: string;
  nombrePlan: string;
  idRefUniversidad: string;
  acronimoUniversidad: string;
  idRefCentro: string;
  idRefAreaAcademica: string;
  idRefTipoEstudio: string;
  idRefEstudio: string;
  nombreEstudio: string;
  idRefTitulo: string;
  fechaApertura: Date | null;
  fechaFinalizacion: Date | null;
  fechaTrabajoFinEstudio: string | null;
  tituloTrabajoFinEstudio: string;
  fechaExpedicion: string | null;
  notaMedia: number | null;
  fechaPago: string | null;
  displayNameIdIntegracion: string;
  displayNameDocumentoIdentificacionAlumno: string;
  displayNameNombreAlumno: string;
  expedientesEspecializaciones: ExpedienteEspecializacionDto[];
  titulacionAcceso: TitulacionAccesoDto;
  alumno: AlumnoInfoDto;
  consolidacionesRequisitosExpedientes: ConsolidacionRequisitoExpedienteDto[];
  estado: EstadoExpedienteDto;
}

export interface EstadoExpedienteDto {
  id: number;
  nombre: string;
  color: string;
}
export interface ExpedienteEspecializacionDto {
  id: number;
  idRefEspecializacion: number;
}

export class TitulacionAccesoDto {
  titulo: string;
  institucionDocente: string;
  nroSemestreRealizados: number | null;
  tipoEstudio: string;
  idRefTerritorioInstitucionDocente: string;
  fechaInicioTitulo: Date | null;
  fechafinTitulo: Date | null;
  codigoColegiadoProfesional: string;
  idRefInstitucionDocente: string;
  codeCountryInstitucionDocente: string;
}

export class EducationalInstitutions {
  code: string;
  name: string;
  url: string;
  countryCode: string;
  countryName: string;
}

export enum FormInstitucionDocente {
  paisInstitucionDocente = 'pais',
  institucionDocente = 'institucionDocente',
  otro = 'otro',
  detalleOtro = 'detalleOtro'
}

export enum FormTitulacionAcceso {
  titulo = 'titulo',
  tipoEstudio = 'tipoEstudio',
  institucionDocente = 'institucionDocente',
  codeinstitucionDocente = 'codeinstitucionDocente',
  idRefTerritorioInstitucionDocente = 'idRefTerritorioInstitucionDocente',
  territorioInstitucionDocente = 'territorioInstitucionDocente',
  fechaInicioTitulo = 'fechaInicioTitulo',
  fechafinTitulo = 'fechafinTitulo',
  nroSemestreRealizados = 'nroSemestreRealizados',
  codigoColegiadoProfesional = 'codigoColegiadoProfesional'
}

export class Country {
  isoCode: string;
  name: string;
  iso3166Alpha3: string;
  iso3166Numeric: string;
  phoneCode: string;
  preferredDivisions: string[];
}

export class EditExpedienteAlumno {
  idExpedienteAlumno: number;
  idRefVersionPlan: string;
  nroVersion: number;
  idRefNodo: string;
  porIntegracion: boolean;
  idRefViaAcceso: string;
  titulacionAcceso: TitulacionAccesoDto;
}

export class UniversityBindingType {
  code: string;
  sniesCode: string;
  name: string;
}

export interface Universities {
  code: string;
  name: string;
  countryCode: string;
  countryName: string;
  acronym: string;
  logoImage: string;
  address: string;
  phoneNumber: string;
  phoneNumber1: string;
  phoneNumber2: string;
  faxNumber: string;
  faxNumber1: string;
  faxNumber2: string;
  legalNoticeUrl: string;
  privacyPolicyUrl: string;
  legalNoticeContent: string;
  privacyPolicyContent: string;
  cookiePolicyUrl: string;
  cookiePolicyContent: string;
  contactEmail: string;
  companyId: number;
  companyCode: string;
  defaultCulture: string;
}

export interface UniversidadModel {
  id?: number;
  nombre?: string;
  idIntegracion?: string;
}

export class AnotacionDto {
  id: number;
  fecha: Date;
  idRefCuentaSeguridad: string;
  esPublica: boolean;
  esRestringida: boolean;
  rolesAnotaciones: RolAnotacionDto[];
  resumen: string;
  mensaje: string;
  fechaModificacion: Date;
  idRefCuentaSeguridadModificacion: string;
  nombreUsuario: string;
  expedienteAlumno: ExpedienteAlumnoAnotacionDto;
}

export class RolAnotacionDto {
  rol: string;
  rolName: string;
}

export class ExpedienteAlumnoAnotacionDto {
  id: number;
}

export class SolicitudDto {
  id: number;
  codigoSolicitud: string;
  fechaSolicitud: Date;
  fechaPago: Date;
  fechaEstado: Date;
  idRefUniversidad: string;
  universidad: UniversidadSolicitudDto;
  tipo: TipoSolicitudDto;
  estado: EstadoSolicitudDto;
}

export class UniversidadSolicitudDto {
  idUniversidad: number;
  acronimo: string;
}

export class TipoSolicitudDto {
  id: number;
  refCodigoTipoSolicitud: string;
  nombre: string;
}

export class EstadoSolicitudDto {
  id: number;
  nombre: string;
}

export class AlumnoInfoDto {
  idAlumno: number;
  idIntegracionAlumno: string;
  displayName: string;
  foto: string;
  sexo: string;
  celular: string;
  nacionalidad: string;
  fechaNacimiento: string;
  email: string;
  tipoDocumentoIdentificacionPais: string;
  nroDocIdentificacion: string;
  idUniversidadIntegracion: string;
  acronimoUniversidad: string;
  documentosIdentificacion: DocumentoIdentificacionDto[];
  documentosAlumno: DocumentosAlumnoDto[];
  expedientes: ExpedienteDto[];
  matriculas: MatriculaDto[];
}

export class DocumentoIdentificacionDto {
  id: number;
  displayName: string;
  numero: string;
}

export class DocumentosAlumnoDto {
  id: number;
  descripcion: string;
}

export interface ExpedienteDto {
  id: number;
  fechaApertura: Date;
  fechaFinalizacion: Date;
  nombrePlan: string;
  nombreEstudio: string;
  acronimoUniversidad: string;
  consolidacionesRequisitosExpedientes: ConsolidacionRequisitoExpedienteDto[];
}

export interface ConsolidacionRequisitoExpedienteDto {
  id: number;
  esDocumentacionFisica: boolean;
  documentacionRecibida: boolean;
  enviadaPorAlumno: boolean;
  idRefIdioma: string;
  siglasIdioma: string;
  nombreIdioma: string;
  nivelIdioma: string;
  texto: string;
  fecha: Date;
  isEstadoNoProcesada: boolean;
  isEstadoValidada: boolean;
  isEstadoPendiente: boolean;
  isEstadoRechazada: boolean;
  requisitoExpediente: RequisitoExpedienteDto;
  estadoRequisitoExpediente: EstadoRequisitoExpedienteDto;
  tipoRequisitoExpediente: TipoRequisitoExpedienteDto;
}

export interface RequisitoExpedienteDto {
  id: number;
  nombre: string;
  orden: number;
  descripcion: string;
  requiereDocumentacion: boolean;
  esLogro: boolean;
  certificadoIdioma: boolean;
  requiereTextoAdicional: boolean;
  bloqueado: boolean;
  estaRestringida: boolean;
  requisitosExpedientesDocumentos: RequisitoExpedienteDocumentoDto[];
  rolesRequisitosExpedientes: RolRequisitoExpedienteDto[];
}

export interface RequisitoExpedienteDocumentoDto {
  id: number;
  nombreDocumento: string;
  documentoObligatorio: boolean;
}

export interface RolRequisitoExpedienteDto {
  id: number;
  rol: string;
  rolName: string;
}

export interface EstadoRequisitoExpedienteDto {
  id: number;
  nombre: string;
}

export interface TipoRequisitoExpedienteDto {
  id: number;
  nombre: string;
}

export interface MatriculaDto {
  id: number;
  idIntegracion: string;
  displayName: string;
  idRefExpedienteAlumno: string;
  totalCreditosAsignaturasMatriculadasActivas: number;
  tipo: TipoMatriculaDto;
  regionEstudio: RegionEstudioDto;
  estado: EstadoMatriculaDto;
  planOfertado: PlanOfertadoDtoDto;
}

export interface TipoMatriculaDto {
  displayName: string;
}

export interface RegionEstudioDto {
  displayName: string;
}

export interface EstadoMatriculaDto {
  displayName: string;
}

export interface PlanOfertadoDtoDto {
  periodoAcademico: PeriodoAcademicoDto;
  plan: PlanDto;
}

export interface PeriodoAcademicoDto {
  displayName: string;
  fechaInicio: string;
  anyoAcademico: AnyoAcademicoDto;
}

export interface AnyoAcademicoDto {
  displayName: string;
}

export interface PlanDto {
  displayName: string;
}

export class AlumnoMatriculasParameters {
  index: number;
  count: number;
  idIntegracionAlumno: string;
}

export interface MatriculaErpAcademicoModel {
  alumnoDisplayName: string;
  alumnoId: number;
  anyoAcademicoDisplayName: string;
  centroEstudioDisplayName: string;
  countAmplicacionesMatriculas: number;
  countAsignaturaMatriculadas: number;
  countBajas: number;
  countConsolidacionesCondicionesMatriculas: number;
  countVariacionesMatriculas: number;
  displayName: string;
  esMatriculaAsignable: boolean;
  esValidoDarAltaMatricula: boolean;
  esValidoDarBajaMatricula: boolean;
  esValidoRecuperarMatricula: boolean;
  estudioDisplayName: string;
  fechaInicioPeriodoAcademicoMatricula: Date;
  id: number;
  idIntegracion: string;
  idPersonaLogged: number;
  idUniversidad: number;
  periodoAcademicoDisplayName: string;
  planDisplayName: string;
  tipoEstudioDisplayName: string;
  tipoMatriculaDisplayName: string;
  tituloDisplayName: string;
  totalCreditosAsignaturasMatriculadasActivas: number;
  universidadDisplayName: string;
  regionEstudioDisplayName: string;
  estado: EstadoMatriculaErpAcademicoModel;
  idRefExpedienteAlumno: number;
}

export interface EstadoMatriculaErpAcademicoModel {
  displayName: string;
}

export enum MultiselectProperty {
  text = 'text',
  value = 'value',
  object = 'object',
  int = 'int',
  string = 'string'
}
