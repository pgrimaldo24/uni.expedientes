export class Observacion {
  id: number;
  idExpedienteAlumno: number;
  esPublica: boolean;
  esRestringida: boolean;
  rolesAnotaciones: RolAnotacion[];
  resumen: string;
  mensaje: string;
  static create(form: Observacion): Observacion {
    const result = new Observacion();
    result.resumen = form.resumen;
    result.mensaje = form.mensaje;
    return result;
  }
}

export class RolAnotacion {
  rol: string;
  rolName: string;
}

export enum ObservacionControl {
  resumen = 'resumen',
  mensaje = 'mensaje',
  ambito = 'ambito',
  roles = 'roles'
}

export enum Ambito {
  publica = 1,
  privada = 2,
  restringida = 3
}
