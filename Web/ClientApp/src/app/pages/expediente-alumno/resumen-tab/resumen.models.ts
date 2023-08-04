export enum FormResumenControls {
  trayecto = 'trayecto'
}

export enum GrafoColores {
  green = '#4EB24E',
  yellow = '#F7AC2C',
  red = '#F9484E',
  blue = '#5C96BC',
  white = 'white',
  gray = '#c1c3c6',
  orange = 'orange'
}
export interface TipoSituacionEstadoExpedienteDto {
  id: number;
  descripcion: string;
  fechaInicio: string;
  fechaFin?: Date;
}

export interface TipoSituacionEstadoDto {
  id: number;
  nombre: string;
}

export interface HitoConseguidoDto {
  id: number;
  nombre: string;
  fechaInicio: string;
  fechaFin?: Date;
  tipoConseguido: TipoHitoConseguidoDto;
}

export interface TipoHitoConseguidoDto {
  id: number;
  nombre: string;
  icono: string;
  esPrimeraMatricula: boolean;
  esTrabajoFinEstudio: boolean;
}

export interface GrafoDto {
  tienePosicionGrafica: boolean;
  nodos: NodoDto[];
  esNodoInicial: boolean;
}

export interface NodoDto {
  id: number;
  nombre: string;
  tipo: TipoNodoDto;
  hijos: NodoDto[];
  arcosSalientes: ArcoDto[];
}

export interface TipoNodoDto {
  id: number;
  nombre: string;
  esInicial: boolean;
  esIntermedio: boolean;
  esFinal: boolean;
}

export interface ArcoDto {
  id: number;
  nombre: string;
  bloques: BloqueDto[];
  nodoDestino: NodoDestinoDto;
}

export interface BloqueDto {
  id: number;
  nombre: string;
  minCreditos: number;
  asignaturas: AsignaturaDto[];
  subBloques: BloqueDto[];
}

export interface NodoDestinoDto {
  id: number;
}

export interface AsignaturaDto {
  id: number;
  idAsignaturaPlan: number;
  idNodoDestino: number;
  nombre: string;
  codigo: string;
  creditos: number;
  tipoAsignatura: TipoAsignaturaDto;
  found: boolean;
}

export interface TipoAsignaturaDto {
  id: number;
  nombre: string;
  esReconocimiento: boolean;
}

export class RequestNodoPlan {
  filterIdPlan: number;
}

export interface NodoTrayectoDto {
  id: number;
  nodoDestino: NodoDestinoDto[];
}
