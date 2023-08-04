import { RequisitoDto } from '@pages/requisitos/requesitos.models';

export enum FormFiltroComportamientoControls {
  comportamientoExpediente = 'comportamientoExpediente',
  estaVigente = 'estaVigente',
  nivelUso = 'nivelUso',
  condiciones = 'condiciones',
  descripcion = 'descripcion'
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

export class ComportamientoExpedienteDto {
  id: number;
  nombre: string;
  estaVigente: boolean;
  descripcion: string;
  requisitosComportamientosExpedientes: RequisitoComportamientoExpedienteDto[];
  nivelesUsoComportamientosExpedientes: NivelUsoComportamientoExpedienteDto[];
  static create(
    form: ComportamientoExpedienteDto,
    requisitosComportamientosExpedientes: RequisitoComportamientoExpedienteDto[],
    nivelesUsoComportamientosExpedientes: NivelUsoComportamientoExpedienteDto[]
  ): ComportamientoExpedienteDto {
    const result = new ComportamientoExpedienteDto();
    result.nombre = form.nombre;
    result.estaVigente = form.estaVigente;
    result.descripcion = form.descripcion;
    result.requisitosComportamientosExpedientes = requisitosComportamientosExpedientes;
    result.nivelesUsoComportamientosExpedientes = nivelesUsoComportamientosExpedientes;
    return result;
  }
}

export class EditComportamientoExpedienteDto {
  id: number;
  nombre: string;
  estaVigente: boolean;
  descripcion: string;
  static create(
    form: EditComportamientoExpedienteDto,
    id: number
  ): EditComportamientoExpedienteDto {
    const result = new EditComportamientoExpedienteDto();
    result.id = id;
    result.nombre = form.nombre;
    result.estaVigente = form.estaVigente;
    result.descripcion = form.descripcion;
    return result;
  }
}

export class ComportamientoExpedienteMasivoDto {
  idsComportamientos: number[];
}

export class RequisitoComportamientoExpedienteMasivoDto {
  IdsRequisitosComportamientos: number[];
}

export interface RequisitoComportamientoExpedienteDto {
  id: number;
  requisitoExpediente: RequisitoDto;
  tipoRequisitoExpediente: TipoRequisitoExpedienteDto;
}

export class EditRequisitoComportamientoExpedienteDto {
  idRequisitoComportamiento: number;
  requisitoExpediente: RequisitoDto;
  tipoRequisitoExpediente: TipoRequisitoExpedienteDto;
  static create(
    requisitoComportamiento: RequisitoComportamientoExpedienteDto
  ): EditRequisitoComportamientoExpedienteDto {
    const result = new EditRequisitoComportamientoExpedienteDto();
    (result.idRequisitoComportamiento = requisitoComportamiento.id),
      (result.requisitoExpediente =
        requisitoComportamiento.requisitoExpediente);
    result.tipoRequisitoExpediente =
      requisitoComportamiento.tipoRequisitoExpediente;
    return result;
  }
}

export class CreateRequisitoComportamientoExpedienteDto {
  idComportamiento: number;
  requisitoExpediente: RequisitoDto;
  tipoRequisitoExpediente: TipoRequisitoExpedienteDto;
  static create(
    requisitoComportamiento: RequisitoComportamientoExpedienteDto,
    idComportamiento: number
  ): CreateRequisitoComportamientoExpedienteDto {
    const result = new CreateRequisitoComportamientoExpedienteDto();
    (result.idComportamiento = idComportamiento),
      (result.requisitoExpediente =
        requisitoComportamiento.requisitoExpediente);
    result.tipoRequisitoExpediente =
      requisitoComportamiento.tipoRequisitoExpediente;
    return result;
  }
}

export interface NivelUsoComportamientoExpedienteDto {
  id: number;
  idRefUniversidad: string;
  idRefTipoEstudio: string;
  idRefEstudio: string;
  idRefPlan: string;
  idRefTipoAsignatura: string;
  idRefAsignaturaPlan: string;
  tipoNivelUso: TipoNivelUsoDto;
  acronimoUniversidad: string;
  nombreTipoEstudio: string;
  nombreEstudio: string;
  nombrePlan: string;
  nombreTipoAsignatura: string;
  nombreAsignatura: string;
  displayName: string;
  index: number;
}

export interface TipoNivelUsoDto {
  id: number;
  nombre: string;
  esUniversidad: boolean;
  esTipoEstudio: boolean;
  esEstudio: boolean;
  esPlanEstudio: boolean;
  esTipoAsignatura: boolean;
  esAsignaturaPlan: boolean;
}

export class CreateNivelUsoComportamientoExpedienteDto {
  idComportamiento: number;
  nivelUsoComportamientoExpediente: NivelUsoComportamientoExpedienteDto;
}

export class GetNivelUsoComportamientoExpedienteDto {
  nivelUsoComportamientoExpediente: NivelUsoComportamientoExpedienteDto;
}

export interface TipoRequisitoExpedienteDto {
  id: number;
  nombre: string;
}

export enum RequisitoComportamientoExpedienteField {
  nombre = 'nombre',
  obligatoria = 'obligatoria',
  estaVigente = 'estaVigente',
  requeridaParaTitulo = 'requeridaParaTitulo',
  requeridaParaPago = 'requeridaParaPago',
  requiereDocumentacion = 'requiereDocumentacion',
  modoRequerimientoDocumentacion = 'modoRequerimientoDocumentacion'
}

export enum EntityType {
  FOLDER = 'folder',
  FILE = 'file',
  ROOT = 'root'
}
