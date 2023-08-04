import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { HttpErrorsInterceptor } from './http-errors.interceptor';
import { RolesDirective } from './roles.directive';
import { UnirEmailDirective } from './unir-email.directive';
import { UnirTrimLeftModelDirective } from './unir-trim-left-model.directive';
import { UnirTrimLeftDirective } from './unir-trim-left.directive';
import { UnirOnlyNumberDirective } from './unir-only-number.directive';

@NgModule({
  declarations: [
    RolesDirective,
    UnirEmailDirective,
    UnirTrimLeftModelDirective,
    UnirTrimLeftDirective,
    UnirOnlyNumberDirective
  ],
  imports: [CommonModule],
  exports: [
    RolesDirective,
    UnirEmailDirective,
    UnirTrimLeftModelDirective,
    UnirTrimLeftDirective,
    UnirOnlyNumberDirective
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpErrorsInterceptor,
      multi: true
    }
  ]
})
export class CoreModule {}
