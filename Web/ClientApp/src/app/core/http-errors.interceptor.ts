import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { AlertHandlerService } from '@tools/alert/alert-handler.service';
import { AlertType } from '@tools/alert/models';
import { BlockUIService } from 'ng-block-ui';
import { ResponseError } from '@cal/response-message';

@Injectable()
export class HttpErrorsInterceptor implements HttpInterceptor {
  constructor(
    private alertService: AlertHandlerService,
    private blockUIService: BlockUIService
  ) {}

  intercept(
    request: HttpRequest<unknown>,
    next: HttpHandler
  ): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      tap(
        () => {},
        (error: unknown) => {
          this.blockUIService.resetGlobal();
          if (error instanceof HttpErrorResponse) {
            switch (error.status) {
              case 200:
                this.handleError200(error);
                break;
              case 400: // Bad Request
                this.alertService.error(error.error.message, false);
                break;
              case 401: // Unauthorized
              case 403: // Forbiden
                this.alertService.error(
                  'No tiene permisos para realizar esta operación, o bien, su sesión ha expirado',
                  true
                );
                break;
              case 404: // Not Found
                this.alertService.error('Elemento no encontrado', false);
                break;
              case 409: // Conflict
              case 422: // Unprocessable Entity
                this.handleValidationError(error);
                break;
              default:
                this.alertService.error('Error interno', false);
                break;
            }
          }
        }
      )
    );
  }

  private handleError200(error: HttpErrorResponse): void {
    if (!error.message) {
      return;
    }
    this.alertService.custom({
      autoClose: false,
      message: error.message,
      type: AlertType.error
    });
  }

  private handleValidationError(error: HttpErrorResponse): void {
    const response = <ResponseError>error.error;
    if (response.details && response.details.length > 0) {
      this.alertService.custom({
        autoClose: false,
        title: 'Error de validación',
        messages: response.details,
        type: AlertType.error
      });
    } else {
      this.alertService.error(response.message, false);
    }
  }
}
