import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Location } from '@angular/common';
import * as helpers from '@helpers/commons-helpers';
import { Guid } from '@src/app/component-tools/combobox/models';
import { BlockUIService } from 'ng-block-ui';

@Component({
  selector: 'app-show',
  templateUrl: './show.component.html',
  styleUrls: ['./show.component.scss']
})
export class ShowComponent implements OnInit {
  isReadOnly = true;
  section: string;
  blockIdentity = Guid.create().toString();
  colapsadoTitulacion: boolean;
  colapsadoViaAccesoPlan: boolean;
  colapsadoTitulacionAcceso: boolean;
  idAlumno: number;
  data = null;
  tieneExpedienteBloqueado = null;
  tipoVinculacion = null;

  constructor(
    private activatedRoute: ActivatedRoute,
    private route: Router,
    private blockUIService: BlockUIService,
    private location: Location
  ) {}

  ngOnInit(): void {
    this.section = (this.location.getState() as { section: string })?.section;
    if (!this.section) this.section = 'expediente';
    this.blockUIService.start(this.blockIdentity);
    this.activatedRoute.params.subscribe(({ id }) => {
      if (id === undefined) {
        this.route.navigateByUrl('/');
      }
      this.idAlumno = id;
    });
  }

  showHideTitulacion(): void {
    this.colapsadoTitulacion = !this.colapsadoTitulacion;
    helpers.clearFocus();
  }

  showHideViaAccesoPlan(): void {
    this.colapsadoViaAccesoPlan = !this.colapsadoViaAccesoPlan;
    helpers.clearFocus();
  }

  showHideTitulacionAcceso(): void {
    this.colapsadoTitulacionAcceso = !this.colapsadoTitulacionAcceso;
    helpers.clearFocus();
  }
}
