import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ReconocimientoClasificacionDto } from '../calificaciones-tab/calificaciones-model';
import { DatosErpResponse, ExpedienteAlumnoDto } from '../expediente-models';
import { FormResumenControls, GrafoDto } from './resumen.models';

@Component({
  selector: 'app-resumen-tab',
  templateUrl: './resumen-tab.component.html',
  styleUrls: ['./resumen-tab.component.scss']
})
export class ResumenTabComponent implements OnInit {
  @Input() expedienteAlumno: ExpedienteAlumnoDto;
  @Input() datosErp: DatosErpResponse;
  @Input() grafoPlan: GrafoDto;
  @Input() reconocimientos: ReconocimientoClasificacionDto;
  public resumenForm: FormGroup;
  public fullScreen: boolean;

  constructor(private fb: FormBuilder) {}

  ngOnInit(): void {
    this.resumenForm = this.createResumenForm();
  }

  createResumenForm(): FormGroup {
    return this.fb.group({
      [FormResumenControls.trayecto]: null
    });
  }

  setFullScreen(): void {
    this.fullScreen = !this.fullScreen;
  }
}
