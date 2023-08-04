import { Component, Input } from '@angular/core';
import { ExpedienteAlumnoDto } from '../expediente-models';
import {
  AsignaturaExpedienteDto,
  FormCalificacionesControls,
  ReconocimientoClasificacionDto,
  RequerimientoPlanDto
} from './calificaciones-model';
import { FormBuilder, FormGroup } from '@angular/forms';
import { AsignaturaDto, GrafoDto } from '../resumen-tab/resumen.models';
import { AlertHandlerService } from '@src/app/component-tools/alert/alert-handler.service';
import { AlertType } from '@src/app/component-tools/alert/models';

@Component({
  selector: 'app-calificaciones-tab',
  templateUrl: './calificaciones-tab.component.html',
  styleUrls: ['./calificaciones-tab.component.scss']
})
export class CalificacionesTabComponent {
  @Input() expedienteAlumno: ExpedienteAlumnoDto;
  @Input() grafoPlan: GrafoDto;
  @Input() reconocimientos: ReconocimientoClasificacionDto;
  public calificacionesForm: FormGroup;
  public requerimientoPlan: RequerimientoPlanDto;
  public asignaturasExpediente: AsignaturaExpedienteDto[];
  public asignaturasTrayecto: AsignaturaDto[];

  constructor(
    private fb: FormBuilder,
    private alertService: AlertHandlerService
  ) {}

  ngOnInit(): void {
    this.calificacionesForm = this.createCalificacionesForm();
    if (this.reconocimientos?.mensajesError.length) {
      this.alertService.custom({
        autoClose: false,
        messages: this.reconocimientos.mensajesError,
        type: AlertType.error
      });
    }
  }

  createCalificacionesForm(): FormGroup {
    return this.fb.group({
      [FormCalificacionesControls.trayecto]: null
    });
  }
}
