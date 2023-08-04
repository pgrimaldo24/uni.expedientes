import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { TranslateService } from '@ngx-translate/core';
import {
  DatosErpResponse,
  ExpedienteAlumnoDto
} from '@pages/expediente-alumno/expediente-models';

@Component({
  selector: 'app-popup-cronologia',
  templateUrl: './popup-cronologia.component.html',
  styleUrls: ['./popup-cronologia.component.scss']
})
export class PopupCronologiaComponent implements OnInit {
  @Input() expedienteAlumno: ExpedienteAlumnoDto;
  @Input() datosErp: DatosErpResponse;
  @ViewChild('content') modalContent: ElementRef;
  public titulo: string;
  public esPrimeraMatricula: boolean;

  constructor(
    private modalService: NgbModal,
    private translateService: TranslateService
  ) {}

  ngOnInit(): void {}

  loadModal(esPrimeraMatricula: boolean): void {
    this.modalService.open(this.modalContent, {
      ariaLabelledBy: 'modal-basic-title',
      backdrop: 'static',
      centered: true
    });
    this.esPrimeraMatricula = esPrimeraMatricula;
    const tituloPopup = esPrimeraMatricula
      ? 'informacionAccesoTitle'
      : 'trabajoFinEstudioTitle';
    this.titulo = this.translateService.instant(
      `pages.expedienteTabs.resumen.${tituloPopup}`
    );
  }

  closeModal(modal: NgbModalRef): void {
    modal.close();
  }
}
