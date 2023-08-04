import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CustomCurrencyPipe } from './CustomCurrencyPipe';
import { CustomDateCulturePipe } from './CustomDateCulturePipe';

@NgModule({
  imports: [CommonModule],
  exports: [CustomCurrencyPipe, CustomDateCulturePipe],
  declarations: [CustomCurrencyPipe, CustomDateCulturePipe]
})
export class PipesModule {}
