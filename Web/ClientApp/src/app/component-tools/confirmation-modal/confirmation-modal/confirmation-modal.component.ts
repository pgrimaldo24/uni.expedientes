import {
  Component,
  OnInit,
  ViewChild,
  AfterViewChecked,
  ChangeDetectorRef,
  Input
} from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-confirmation-modal',
  templateUrl: './confirmation-modal.component.html',
  styleUrls: ['./confirmation-modal.component.scss']
})
export class ConfirmationModalComponent implements OnInit, AfterViewChecked {
  title: string;
  confirmationMessage: string;
  confirmLabel: string;
  callbackConfirm: () => void;

  @ViewChild('content') modalContent: unknown;
  @Input() onlyClose = false;
  constructor(
    private cdRef: ChangeDetectorRef,
    private translate: TranslateService,
    private modalService: NgbModal
  ) {}

  ngOnInit(): void {}

  ngAfterViewChecked(): void {
    this.cdRef.detectChanges();
  }

  show(
    callbackConfirm?: () => void,
    title?: string,
    confirmationMessage?: string,
    confirmLabel?: string
  ): void {
    this.callbackConfirm = callbackConfirm ? callbackConfirm : () => {};
    this.title = title ? title : this.translate.instant('common.appName');
    this.confirmationMessage = confirmationMessage
      ? confirmationMessage
      : this.translate.instant('messages.confirm');
    this.confirmLabel = confirmLabel
      ? confirmLabel
      : this.translate.instant('common.confirmar');
    this.modalService.open(this.modalContent, {
      ariaLabelledBy: 'modal-basic-title',
      backdrop: 'static',
      centered: true
    });
  }

  cancelar(modal: Window): void {
    modal.close();
  }

  confirmation(modal: Window, evento: Event): void {
    evento.preventDefault();
    this.callbackConfirm();
    modal.close();
  }
}
