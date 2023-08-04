import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { keys } from '@src/keys';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AppConfigService {
  private appConfig: AppConfig;

  private baseUrl: string;
  constructor(private http: HttpClient) {
    this.http = http;
    this.baseUrl = keys.API_BASE_URL;
  }

  loadAppConfig(): Promise<void> {
    return this.http
      .get('./appConfig.json')
      .toPromise()
      .then((data) => {
        this.appConfig = data as AppConfig;
      });
  }

  getConfig(): AppConfig {
    return this.appConfig;
  }

  getVersion(): Observable<string> {
    return this.http.get(`${this.baseUrl}/api/v1/about/version`, {
      responseType: 'text'
    });
  }
}
export class AppConfig {
  securityUri: string;
  clientId: string;
  urlErpAcademico: string;
}
