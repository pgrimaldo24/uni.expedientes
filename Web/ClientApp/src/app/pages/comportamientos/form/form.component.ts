import { Component, OnInit, ViewChild } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormControl,
  FormGroup,
  Validators
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { AlertHandlerService } from '@src/app/component-tools/alert/alert-handler.service';
import { Guid } from 'guid-typescript';
import { BlockUIService } from 'ng-block-ui';
import {
  ComportamientoExpedienteDto,
  RequisitoComportamientoExpedienteDto,
  NivelUsoComportamientoExpedienteDto,
  EditComportamientoExpedienteDto
} from '../comportamientos.models';
import { ComportamientosService } from '../comportamientos.service';
import { ComportamientoControl } from './comportamiento-control.mode';
import { TableRequisitosComportamientosComponent } from '../table-requisitos-comportamientos/table-requisitos-comportamientos.component';
import { NivelesUsoComportamientosComponent } from '../niveles-uso-comportamientos/niveles-uso-comportamientos.component';
import { DataRow } from '@src/app/component-tools/data-table/models';

@Component({
  selector: 'app-form',
  templateUrl: './form.component.html',
  styleUrls: ['./form.component.scss']
})
export class FormComponent implements OnInit {
  public controls = ComportamientoControl;
  public form: FormGroup;
  public blockIdentity = Guid.create().toString();
  idComportamiento: number;
  comportamiento: ComportamientoExpedienteDto;
  @ViewChild('tableRequisitosComportamientos', { static: true })
  tableRequisitosComportamientos: TableRequisitosComportamientosComponent;
  requisitosComportamientosRows: DataRow[] = [];
  nivelesUso: NivelUsoComportamientoExpedienteDto[] = [];
  @ViewChild('nivelesUsoComportamientoComportamientos', { static: true })
  nivelesUsoComportamientoComportamientos: NivelesUsoComportamientosComponent;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private comportamientoService: ComportamientosService,
    private blockUI: BlockUIService,
    private translateService: TranslateService,
    private alertService: AlertHandlerService
  ) {
    this.form = this.createForm();
  }

  ngOnInit(): void {
    this.initializeEdit();
  }

  createForm(): FormGroup {
    return this.fb.group({
      [ComportamientoControl.nombre]: new FormControl(null, [
        Validators.required
      ]),
      [ComportamientoControl.descripcion]: new FormControl(null),
      [ComportamientoControl.estaVigente]: new FormControl(true)
    });
  }

  initializeEdit(): void {
    this.route.params.subscribe(({ id }) => {
      if (id) {
        this.idComportamiento = id;
        this.loadComportamiento(id);
      }
    });
  }

  loadComportamiento(id: number): void {
    this.blockUI.start(this.blockIdentity);
    this.tableRequisitosComportamientos.block();
    this.nivelesUsoComportamientoComportamientos.block();
    this.comportamientoService.getComportamiento(id).subscribe((value) => {
      this.blockUI.stop(this.blockIdentity);
      this.comportamiento = value;
      this.patchForm(value);
      this.tableRequisitosComportamientos.unblock();
      this.tableRequisitosComportamientos.update(this.comportamiento);
      this.nivelesUsoComportamientoComportamientos.unblock();
      this.nivelesUsoComportamientoComportamientos.update(this.comportamiento);
    });
  }

  patchForm(value: ComportamientoExpedienteDto): void {
    this.form.patchValue({
      [ComportamientoControl.nombre]: value.nombre,
      [ComportamientoControl.estaVigente]: value.estaVigente,
      [ComportamientoControl.descripcion]: value.descripcion
    });
  }

  ctrlField(name: string): AbstractControl {
    return this.form.get(name);
  }

  ctrlInvalid(name: string): boolean {
    const field = this.ctrlField(name);
    return field.invalid && (field.touched || field.dirty);
  }

  onChangeRequisitos(rowsEvent: DataRow[]): void {
    this.requisitosComportamientosRows = rowsEvent;
    if (this.comportamiento) {
      this.initializeEdit();
    }
  }

  onChangeNivelesUso(nivelesUso: NivelUsoComportamientoExpedienteDto[]): void {
    this.nivelesUso = nivelesUso;
    if (this.comportamiento) {
      this.initializeEdit();
    }
  }

  onSubmit(): void {
    this.form.markAllAsTouched();
    if (this.form.invalid) {
      return;
    }
    if (this.comportamiento) {
      const payload = EditComportamientoExpedienteDto.create(
        this.form.value,
        this.comportamiento.id
      );
      this.blockUI.start(this.blockIdentity);
      this.comportamientoService.updateComportamiento(payload).subscribe(() => {
        this.alertService.success(
          this.translateService.instant('messages.success')
        );
        this.blockUI.stop(this.blockIdentity);
        this.initializeEdit();
      });
    } else {
      if (this.requisitosComportamientosRows.length == 0) {
        this.alertService.error(
          this.translateService.instant(
            'pages.formularioComportamientos.validations.RequisitoatLeastOneSelected'
          )
        );
        return;
      }
      if (this.nivelesUso.length == 0) {
        this.alertService.error(
          this.translateService.instant(
            'pages.formularioComportamientos.validations.NivelUsoatLeastOneSelected'
          )
        );
        return;
      }
      const requisitoComportamientoPayload: RequisitoComportamientoExpedienteDto[] = [];
      this.requisitosComportamientosRows.forEach((row) => {
        if (row.object.requisitoExpediente.id) {
          requisitoComportamientoPayload.push(row.object);
        }
      });
      const payload = ComportamientoExpedienteDto.create(
        this.form.value,
        requisitoComportamientoPayload,
        this.nivelesUso
      );
      this.blockUI.start(this.blockIdentity);
      this.comportamientoService
        .addComportamiento(payload)
        .subscribe((result) => {
          this.alertService.success(
            this.translateService.instant('messages.success')
          );
          this.blockUI.stop(this.blockIdentity);
          this.router.navigate([`/comportamientos/edit/${result}`]);
        });
    }
  }
}
