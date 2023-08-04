import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ConfirmationModalComponent } from '@tools/confirmation-modal/confirmation-modal/confirmation-modal.component';
import { NgbModalModule } from '@ng-bootstrap/ng-bootstrap';
import { TranslateModule } from '@ngx-translate/core';

@NgModule({
  declarations: [ConfirmationModalComponent],
  imports: [CommonModule, NgbModalModule, TranslateModule],
  exports: [ConfirmationModalComponent]
})
export class ConfirmationModalModule {}
