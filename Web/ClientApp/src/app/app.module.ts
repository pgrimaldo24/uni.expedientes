import { HttpClient, HttpClientModule } from '@angular/common/http';
import { APP_INITIALIZER, LOCALE_ID, NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { OAuthModule } from 'angular-oauth2-oidc';

import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { BlockUIModule } from 'ng-block-ui';
import { QuicklinkModule } from 'ngx-quicklink';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { SharedModule } from '@pages/shared/shared.module';
import { AppConfigService } from '@services/app-config.service';
import { SecurityService } from '@services/security.service';
import { CoreModule } from '@src/app/core/core.module';
import { ComboboxModule } from '@tools/combobox/combobox.module';
import { DownloadFileService } from '@services/download-file.service';
import { ToastrModule } from 'ngx-toastr';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgbModule, NgbTooltipModule } from '@ng-bootstrap/ng-bootstrap';
import { ExpedienteModule } from '@pages/expediente-alumno/expediente.module';
import { SeguimientosModule } from '@pages/seguimientos/seguimientos.module';
import { AlumnoModule } from '@pages/alumno/alumno.module';
import { CommonModule } from '@angular/common';
import { DEFAULT_CURRENCY_CODE } from './core/pipes/CustomCurrencyPipe';
import { NavegatorService } from './services/navegator.service';

const appInitializerFn = (appConfig: AppConfigService) => {
  return () => {
    return appConfig.loadAppConfig();
  };
};

@NgModule({
  declarations: [AppComponent],
  imports: [
    CommonModule,
    QuicklinkModule,
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    OAuthModule.forRoot({
      resourceServer: { sendAccessToken: true }
    }),
    FormsModule,
    AppRoutingModule,
    ComboboxModule,
    BlockUIModule.forRoot(),
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: createTranslateLoader,
        deps: [HttpClient]
      }
    }),
    SharedModule,
    CoreModule,
    NgbTooltipModule,
    NgbModule,
    SeguimientosModule,
    ExpedienteModule,
    AlumnoModule,
    ToastrModule.forRoot(),
    BrowserAnimationsModule
  ],
  exports: [QuicklinkModule],
  providers: [
    AppConfigService,
    {
      provide: APP_INITIALIZER,
      useFactory: appInitializerFn,
      multi: true,
      deps: [AppConfigService]
    },
    { provide: LOCALE_ID, useValue: NavegatorService.getNavigatorLanguage() },
    { provide: DEFAULT_CURRENCY_CODE, useValue: 'EUR' },
    SecurityService,
    DownloadFileService
  ],
  bootstrap: [AppComponent]
})
export class AppModule {}

export function createTranslateLoader(http: HttpClient): TranslateHttpLoader {
  return new TranslateHttpLoader(http, 'assets/i18n/', '.json');
}
