import { Component, Input, OnInit } from '@angular/core';
import { DateToLocalStringPipe } from '@helpers/pipes-helpers';
import {
  ConsolidacionRequisitoExpedienteDto,
  ExpedienteAlumnoDto
} from '@pages/expediente-alumno/expediente-models';
import { TipoSituacionEstadoExpedienteDto } from '../resumen.models';
import { ResumenService } from '../resumen.service';

@Component({
  selector: 'app-resumen-situacion',
  templateUrl: './resumen-situacion.component.html',
  styleUrls: ['./resumen-situacion.component.scss']
})
export class ResumenSituacionComponent implements OnInit {
  @Input() expedienteAlumno: ExpedienteAlumnoDto;
  public tipoSituacionEstadoExpediente: TipoSituacionEstadoExpedienteDto;
  public consolidacionesRequisitosExpedientes: ConsolidacionRequisitoExpedienteDto[];
  public fecha: string;
  dateToLocalStringPipe = new DateToLocalStringPipe();
  constructor(private resumenService: ResumenService) {}

  ngOnInit(): void {
    this.getTipoSituacionEstadoByIdExpediente();
    this.getConsolidaciones();
  }

  getTipoSituacionEstadoByIdExpediente(): void {
    this.resumenService
      .getUltimoTipoSituacionEstadoByIdExpediente(this.expedienteAlumno.id)
      .subscribe((tipoSituacionEstadoExpediente) => {
        if (!tipoSituacionEstadoExpediente) return;

        this.tipoSituacionEstadoExpediente = tipoSituacionEstadoExpediente;
        this.fecha = this.dateToLocalStringPipe.transform(
          tipoSituacionEstadoExpediente.fechaInicio,
          'DD MMMM YYYY'
        );
        this.fecha = this.fecha.replace(/ /g, ' de ');
      });
  }

  getConsolidaciones(): void {
    if (!this.expedienteAlumno.consolidacionesRequisitosExpedientes) return;
    this.consolidacionesRequisitosExpedientes = this.expedienteAlumno.consolidacionesRequisitosExpedientes.filter(
      (cre) => cre.isEstadoNoProcesada || cre.isEstadoPendiente
    );
  }
}
