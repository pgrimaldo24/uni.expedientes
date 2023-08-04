import { Criteria } from '@cal/criteria';
export interface GenerateSolicitudTitulo extends Criteria {
  TipoSolicitud: string;
  FechaSolicitud: Date;
  FechaPago: Date;
  IdsExpedientes: number[];
}

export enum GenerateSolicitudTituloControl {
  tipoSolicitud = 'TipoSolicitud',
  fechaSolicitud = 'FechaSolicitud',
  fechaPago = 'FechaPago'
}

export interface TiposSolicicitudes {
  id: number;
  refCodigoTipoSolicitud: string;
  nombre: string;
  conFechaPago: boolean;
  destinoAlumno: boolean;
  formatoTipoSolicitud: FormatoTipoSolicitud;
  displayName: string;
}

export interface FormatoTipoSolicitud {
  codigo: string;
}
