import * as help from '@helpers/commons-helpers';

export enum FormConsolidarRequisitoControls {
  requisitoAdicional = 'requisitoAdicional',
  fileUpload = 'fileUpload',
  esDocumentacionFisica = 'esDocumentacionFisica',
  enviadaPorAlumno = 'enviadaPorAlumno',
  texto = 'texto',
  fecha = 'fecha',
  idioma = 'idioma',
  nivelIdioma = 'nivelIdioma',
  causaEstadoRequisito = 'causaEstadoRequisito',
  requisitoDocumento = 'requisitoDocumento'
}

export enum EstadoRequisitoExpediente {
  NoProcesado = 'No Procesado',
  Pendiente = 'Pendiente',
  Rechazado = 'Rechazado',
  Validado = 'Validado'
}

export interface ConsolidacionRequisitoExpedienteDocumentoDto {
  id: number;
  fichero?: string;
  fechaFichero?: Date;
  idRefDocumento?: string;
  ficheroValidado: boolean;
}

export interface ConfiguracionExpedienteUniversidadDto {
  id: number;
  nombreUniversidad: string;
  codigoDocumental: string;
  tamanyoMaximoFichero: number;
}

export class ConsolidacionRequisitoExpedienteModel {
  id: number;
  esDocumentacionFisica: boolean;
  enviadaPorAlumno: boolean;
  idRefIdioma: string;
  nombreIdioma: string;
  siglasIdioma: string;
  nivelIdioma: string;
  texto: string;
  fecha: string | null;
  idRequisitoExpediente: number;
  idExpedienteAlumno: number;
  idCausaEstadoRequisito: number | null;
  static create(form: any): ConsolidacionRequisitoExpedienteModel {
    const result = new ConsolidacionRequisitoExpedienteModel();
    result.esDocumentacionFisica = form.esDocumentacionFisica;
    result.enviadaPorAlumno = form.enviadaPorAlumno;
    result.idRefIdioma = form.idioma?.value.toString();
    result.nombreIdioma = form.idioma?.text;
    result.siglasIdioma = form.idioma?.data.siglas;
    result.nivelIdioma = form.nivelIdioma;
    result.idCausaEstadoRequisito = form.causaEstadoRequisito?.value;
    result.texto = form.texto;
    result.fecha = help.dateToInputValue(form.fecha);
    return result;
  }
}
