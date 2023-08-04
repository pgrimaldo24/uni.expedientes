import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { DateToLocalStringPipe } from '@helpers/pipes-helpers';
import {
  DatosErpResponse,
  ExpedienteAlumnoDto
} from '@pages/expediente-alumno/expediente-models';
import { HitoConseguidoDto } from '../resumen.models';
import { ResumenService } from '../resumen.service';
import { PopupCronologiaComponent } from './popup-cronologia/popup-cronologia.component';

@Component({
  selector: 'app-resumen-cronologia',
  templateUrl: './resumen-cronologia.component.html',
  styleUrls: ['./resumen-cronologia.component.scss']
})
export class ResumenCronologiaComponent implements OnInit {
  @Input() expedienteAlumno: ExpedienteAlumnoDto;
  @Input() datosErp: DatosErpResponse;
  public hitosConseguidos: HitoConseguidoDto[];
  dateToLocalStringPipe = new DateToLocalStringPipe();
  @ViewChild('modalCronologia') modalCronologia: PopupCronologiaComponent;

  constructor(private resumenService: ResumenService) {}

  ngOnInit(): void {
    this.getHitosConseguidosByIdExpediente();
  }

  getHitosConseguidosByIdExpediente(): void {
    this.resumenService
      .getHitosConseguidosByIdExpediente(this.expedienteAlumno.id)
      .subscribe((hitosConseguidos) => {
        if (!hitosConseguidos.length) return;

        this.hitosConseguidos = hitosConseguidos;
        this.hitosConseguidos.forEach((hito) => {
          const fecha = this.dateToLocalStringPipe.transform(
            hito.fechaInicio,
            'DD MMMM YYYY'
          );
          hito.fechaInicio = fecha.replace(/ /g, ' de ');
        });
      });
  }

  loadModalCronologia(esPrimeraMatricula: boolean): void {
    this.modalCronologia.loadModal(esPrimeraMatricula);
  }
}
