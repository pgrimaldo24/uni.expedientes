import { Component, OnInit, ViewChild } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormControl,
  FormGroup,
  Validators
} from '@angular/forms';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import {
  DocumentoRequisitoControls,
  DocumentoRequisitoModel,
  RequisitoDocumentoModel
} from '@pages/requisitos/requesitos.models';
import { RequisitosService } from '@pages/requisitos/requisitos.service';
import { Guid } from 'guid-typescript';
import { BlockUIService } from 'ng-block-ui';

@Component({
  selector: 'app-documentos-requisitos-form',
  templateUrl: './documentos-requisitos-form.component.html',
  styleUrls: ['./documentos-requisitos-form.component.scss']
})
export class DocumentosRequisitosFormComponent implements OnInit {
  public controls = DocumentoRequisitoControls;
  public form: FormGroup;
  public blockIdentity = Guid.create().toString();
  public creating: boolean;
  idRequisitoExpediente: number;
  idDocumentoExpediente: number;
  private onSuccess: () => void;

  @ViewChild('content', { static: false }) modalRef: NgbModalRef;

  constructor(
    private modal: NgbModal,
    private fb: FormBuilder,
    private blockUI: BlockUIService,
    private requisitoSrv: RequisitosService
  ) {}

  ngOnInit(): void {}

  open(item: RequisitoDocumentoModel, onSuccess = () => {}): void {
    this.onSuccess = onSuccess;
    this.form = this.createForm();
    this.idRequisitoExpediente = item.idRequisitoExpediente;
    this.idDocumentoExpediente = item.id;
    this.creating = true;
    this.form.patchValue({
      [DocumentoRequisitoControls.documentoSecurizado]: item.documentoSecurizado
    });

    this.modal.open(this.modalRef, {
      ariaLabelledBy: 'modal-basic-title',
      backdrop: 'static',
      size: 'xl',
      centered: true,
      backdropClass: 'light-blue-backdrop'
    });
    if (item.id) {
      this.creating = false;
      this.patchForm(item);
    }
  }

  patchForm(item: RequisitoDocumentoModel): void {
    this.form.patchValue({
      [DocumentoRequisitoControls.nombreDocumento]: item.nombreDocumento,
      [DocumentoRequisitoControls.documentoObligatorio]:
        item.documentoObligatorio,
      [DocumentoRequisitoControls.documentoEditable]: item.documentoEditable,
      [DocumentoRequisitoControls.requiereAceptacionAlumno]:
        item.requiereAceptacionAlumno
    });
  }

  onSubmit(): void {
    this.form.markAllAsTouched();
    if (this.form.invalid) {
      return;
    }
    const payload = DocumentoRequisitoModel.create(this.form.value);
    payload.idRequisitoExpediente = this.idRequisitoExpediente;
    if (!this.creating) {
      payload.id = this.idDocumentoExpediente;
    }
    this.blockUI.start(this.blockIdentity);
    if (this.creating) {
      this.requisitoSrv.addDocumentosRequisito(payload).subscribe(() => {
        this.blockUI.stop(this.blockIdentity);
        this.dismissModal();
        this.onSuccess();
      });
    } else {
      this.requisitoSrv.updateDocumentosRequisito(payload).subscribe(() => {
        this.blockUI.stop(this.blockIdentity);
        this.dismissModal();
        this.onSuccess();
      });
    }
  }

  createForm(): FormGroup {
    return this.fb.group({
      [DocumentoRequisitoControls.nombreDocumento]: new FormControl(null, [
        Validators.required
      ]),
      [DocumentoRequisitoControls.documentoObligatorio]: new FormControl(false),
      [DocumentoRequisitoControls.documentoEditable]: new FormControl(false),
      [DocumentoRequisitoControls.requiereAceptacionAlumno]: new FormControl(
        false
      ),
      [DocumentoRequisitoControls.documentoSecurizado]: new FormControl(false)
    });
  }

  ctrlField(name: string): AbstractControl {
    return this.form.get(name);
  }

  ctrlInvalid(name: string): boolean {
    const field = this.ctrlField(name);
    return field.invalid && (field.touched || field.dirty);
  }

  dismissModal(): void {
    this.modal.dismissAll();
  }
}
