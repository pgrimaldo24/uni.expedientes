export enum FormFiltroRequisitoControls {
  condicionExpediente = 'condicionExpediente',
  estaVigente = 'estaVigente',
  requeridoTitulo = 'requeridoTitulo',
  requiereMatricularse = 'requiereMatricularse',
  requeridoPagoTasas = 'requeridoPagoTasas',
  estadoExpediente = 'estadoExpediente',
  requiereDocumentacion = 'requiereDocumentacion',
  documentacionProtegida = 'documentacionProtegida',
  modoRequerimientoDocumentacion = 'modoRequerimientoDocumentacion'
}

export enum DocumentoRequisitoControls {
  nombreDocumento = 'nombreDocumento',
  documentoObligatorio = 'documentoObligatorio',
  documentoEditable = 'documentoEditable',
  requiereAceptacionAlumno = 'requiereAceptacionAlumno',
  documentoSecurizado = 'documentoSecurizado'
}

export enum ModoSeleccion {
  Todos,
  Si,
  No
}

export interface TipoModoSeleccion {
  id: number;
  nombre: string;
}

export interface EstadoExpedienteDto {
  id: number;
  nombre: string;
}

export interface RequisitoExpedienteDto {
  id: number;
  nombre: string;
  orden: number;
  descripcion: string;
  estaVigente: boolean;
  requeridaParaTitulo: boolean;
  requiereMatricularse: boolean;
  requiereDocumentacion: boolean;
  enviarEmailAlumno: boolean;
  requeridaParaPago: boolean;
  estaRestringida: boolean;
  esSancion: boolean;
  esLogro: boolean;
  esCertificado: boolean;
  certificadoIdioma: boolean;
  requiereTextoAdicional: boolean;
  bloqueado: boolean;
  idRefModoRequerimientoDocumentacion?: number;
  modoRequerimientoDocumentacion?: string;
  estadoExpediente: EstadoExpedienteDto;
}

export class RequisitoExpedienteMasivoDto {
  idsRequisitos: number[];
}

export class RequisitoModel {
  nombre: string;
  orden: number;
  static create(form: RequisitoModel): RequisitoModel {
    const result = new RequisitoModel();
    result.nombre = form.nombre;
    result.orden = +form.orden;
    return result;
  }
}

export class DocumentoRequisitoModel {
  id: number;
  nombreDocumento: string;
  documentoObligatorio: boolean;
  documentoEditable: boolean;
  requiereAceptacionAlumno: boolean;
  documentoSecurizado: boolean;
  idRequisitoExpediente: number;
  static create(form: DocumentoRequisitoModel): DocumentoRequisitoModel {
    const result = new DocumentoRequisitoModel();
    result.nombreDocumento = form.nombreDocumento;
    result.documentoObligatorio = form.documentoObligatorio;
    result.documentoEditable = form.documentoEditable;
    result.requiereAceptacionAlumno = form.requiereAceptacionAlumno;
    result.documentoSecurizado = form.documentoSecurizado;
    return result;
  }
}

export interface RequisitoDto {
  id: number;
  nombre: string;
  orden: number;
  descripcion: string;
  estaVigente: boolean;
  requeridaParaTitulo: boolean;
  requiereDocumentacion: boolean;
  enviarEmailAlumno: boolean;
  requeridaParaPago: boolean;
  estaRestringida: boolean;
  esSancion: boolean;
  esLogro: boolean;
  esCertificado: boolean;
  certificadoIdioma: boolean;
  requiereTextoAdicional: boolean;
  bloqueado: boolean;
  idRefModoRequerimientoDocumentacion: number | null;
  nombreModoRequerimientoDocumentacion: string;
  modoRequerimientoDocumentacion: string;
  estadoExpediente: EstadoExpedienteDto;
  requisitosExpedientesDocumentos: RequisitoExpedienteDocumentoDto[];
  rolesRequisitosExpedientes: RolRequisitoExpedienteDto[];
  requisitosExpedientesFilesType: RequisitoExpedienteFileTypeDto[];
  requisitosExpedientesRequerimientosTitulos: RequisitoExpedienteRequerimientoTituloDto[];
}

export interface EstadoExpedienteDto {
  id: number;
  nombre: string;
}

export interface RequisitoExpedienteDocumentoDto {
  id: number;
  nombreDocumento: string;
  documentoObligatorio: boolean;
  documentoEditable: boolean;
  documentoSecurizado: boolean;
  requiereAceptacionAlumno: boolean;
  idRefPlantilla: string;
}

export interface RolRequisitoExpedienteDto {
  id: number;
  rol: string;
  rolName: string;
}

export interface RequisitoExpedienteFileTypeDto {
  id: number;
  idRefFileType: string;
}

export interface RequisitoExpedienteRequerimientoTituloDto {
  id: number;
  requiereMatricularse: boolean;
  tipoRelacionExpediente: TipoRelacionExpedienteDto;
}

export interface TipoRelacionExpedienteDto {
  id: number;
  nombre: string;
  esLogro: boolean;
}

export class RequisitoDocumentoModel {
  id: number;
  nombreDocumento: string;
  documentoObligatorio: boolean;
  documentoEditable: boolean;
  documentoSecurizado: boolean;
  requiereAceptacionAlumno: boolean;
  idRequisitoExpediente: number;
}

export interface DocumentoRequisitoDto {
  id: number;
  nombreDocumento: string;
  documentoObligatorio: boolean;
  documentoEditable: boolean;
  documentoSecurizado: boolean;
  requiereAceptacionAlumno: boolean;
  idRefPlantilla: string;
}

export class DocumentoRequisitoMasivoModel {
  idsDocumentos: number[];
}

export interface FileTypeAcademicoModel {
  id: number;
  name: string;
  extension: string;
  displayName: string;
  checked: boolean;
}

export class EditRequisitoModel {
  id: number;
  nombre: string;
  orden: number;
  idRefUniversidad: string;
  descripcion: string;
  estaVigente: boolean;
  requeridaParaTitulo: boolean;
  requiereDocumentacion: boolean;
  enviarEmailAlumno: boolean;
  requeridaParaPago: boolean;
  estaRestringida: boolean;
  esSancion: boolean;
  esLogro: boolean;
  esCertificado: boolean;
  requiereTextoAdicional: boolean;
  documentoSecurizado: boolean;
  idRefModoRequerimientoDocumentacion: number | null;
  idEstadoExpediente: number;
  idsFilesTypes: string[];
  roles: string[];
  requisitosExpedientesRequerimientosTitulos: RequisitoExpedienteRequerimientoTituloModel;
  static create(form: EditRequisitoModelDto): EditRequisitoModel {
    const result = new EditRequisitoModel();
    result.nombre = form.nombre;
    result.descripcion = form.descripcion;
    result.estaVigente = form.estaVigente;
    result.requeridaParaTitulo = form.requeridaParaTitulo;
    result.requiereDocumentacion = form.requiereDocumentacion;
    result.enviarEmailAlumno = form.enviarEmailAlumno;
    result.requeridaParaPago = form.requeridaParaPago;
    result.estaRestringida = form.estaRestringida;
    result.requiereTextoAdicional = form.requiereTextoAdicional;
    result.esCertificado = form.esCertificado;
    result.documentoSecurizado = form.documentacionProtegida;
    result.idEstadoExpediente = form.estadoExpediente?.value;
    result.idRefModoRequerimientoDocumentacion =
      form.modoDeRequerimientoDeDocumentacion?.value;
    result.roles = form.roles?.map((rol) => rol.value);
    result.requisitosExpedientesRequerimientosTitulos = RequisitoExpedienteRequerimientoTituloModel.create(
      form
    );
    return result;
  }
}

export class EditRequisitoModelDto {
  nombre: string;
  descripcion: string;
  estaVigente: boolean;
  requeridaParaTitulo: boolean;
  requiereDocumentacion: boolean;
  enviarEmailAlumno: boolean;
  requeridaParaPago: boolean;
  estaRestringida: boolean;
  requiereTextoAdicional: boolean;
  esCertificado: boolean;
  documentacionProtegida: boolean;
  requiereMatricularse: boolean;
  estadoExpediente: any;
  modoDeRequerimientoDeDocumentacion: any;
  tipoExpedienteAdicional: any;
  roles: any;
}

export class RequisitoExpedienteRequerimientoTituloModel {
  requiereMatricularse: boolean;
  idTipoRelacionExpediente: number;
  static create(
    form: EditRequisitoModelDto
  ): RequisitoExpedienteRequerimientoTituloModel {
    const result = new RequisitoExpedienteRequerimientoTituloModel();
    result.requiereMatricularse = form.requiereMatricularse;
    result.idTipoRelacionExpediente = form.tipoExpedienteAdicional?.value;
    return result;
  }
}
