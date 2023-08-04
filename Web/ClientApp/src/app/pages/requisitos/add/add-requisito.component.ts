import { Component, OnInit, ViewChild } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormControl,
  FormGroup,
  Validators
} from '@angular/forms';
import { Router } from '@angular/router';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { Guid } from 'guid-typescript';
import { BlockUIService } from 'ng-block-ui';
import { RequisitoControl } from '../form/requisito-control.model';
import { RequisitoModel } from '../requesitos.models';
import { RequisitosService } from '../requisitos.service';

@Component({
  selector: 'app-add-requisito',
  templateUrl: './add-requisito.component.html',
  styleUrls: ['./add-requisito.component.scss']
})
export class AddRequisitoComponent implements OnInit {
  public controls = RequisitoControl;
  public blockIdentity = Guid.create().toString();
  public form: FormGroup;
  @ViewChild('content', { static: false }) modalRef: NgbModalRef;

  constructor(
    private modal: NgbModal,
    private blockUI: BlockUIService,
    private requisitoSrv: RequisitosService,
    private fb: FormBuilder,
    private router: Router
  ) {}

  ngOnInit(): void {}

  open(): void {
    this.form = this.createForm();
    this.modal.open(this.modalRef, {
      ariaLabelledBy: 'modal-basic-title',
      backdrop: 'static',
      size: 'md',
      centered: true,
      backdropClass: 'light-blue-backdrop'
    });
  }

  public onSubmit(): void {
    this.form.markAllAsTouched();
    if (this.form.invalid) {
      return;
    }
    const payload = RequisitoModel.create(this.form.value);
    this.blockUI.start(this.blockIdentity);
    this.requisitoSrv.addRequisito(payload).subscribe((result) => {
      this.blockUI.stop(this.blockIdentity);
      this.dismissModal();
      this.router.navigate([`/requisitos/edit/${result}`]);
    });
  }

  createForm(): FormGroup {
    return this.fb.group({
      [RequisitoControl.nombre]: new FormControl(null, [Validators.required]),
      [RequisitoControl.orden]: new FormControl(null, [Validators.required])
    });
  }

  dismissModal(): void {
    this.modal.dismissAll();
  }

  ctrlField(name: string): AbstractControl {
    return this.form.get(name);
  }

  ctrlInvalid(name: string): boolean {
    const field = this.ctrlField(name);
    return field.invalid && (field.touched || field.dirty);
  }
}
