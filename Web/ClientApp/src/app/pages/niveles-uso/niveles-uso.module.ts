import { BlockUIModule } from 'ng-block-ui';
import { ComboboxModule } from '@tools/combobox/combobox.module';
import { TranslateModule } from '@ngx-translate/core';
import {
  NgbButtonsModule,
  NgbTooltipModule,
  NgbModalModule
} from '@ng-bootstrap/ng-bootstrap';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NivelesUsoPopupAddComponent } from './popup-add/niveles-uso-popup-add.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@NgModule({
  imports: [
    CommonModule,
    NgbButtonsModule,
    NgbTooltipModule,
    NgbModalModule,
    FormsModule,
    ReactiveFormsModule,
    TranslateModule,
    ComboboxModule,
    BlockUIModule.forRoot()
  ],
  exports: [NivelesUsoPopupAddComponent],
  providers: [],
  declarations: [NivelesUsoPopupAddComponent]
})
export class NivelesUsoModule {}
