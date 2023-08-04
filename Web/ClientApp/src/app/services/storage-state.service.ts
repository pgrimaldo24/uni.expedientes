import { Injectable } from '@angular/core';
import { Clause } from '@cal/criteria';

import { IdentityClaimsModel } from '@models/security.models';
import { DataLoadEvent } from '../component-tools/data-table/models';
import { SecurityService } from './security.service';

@Injectable({
  providedIn: 'root'
})
export class StorageStateService {
  constructor(private securityService: SecurityService) {}

  getUserKey(profile: IdentityClaimsModel, key: string): string {
    return `${profile.sub}-${key}`;
  }

  setState(key: string, state: unknown): void {
    const profile = this.securityService.getProfile;
    if (profile == null) {
      return;
    }
    const userKey = this.getUserKey(profile, key);
    const parseJSON = JSON.stringify(state);
    window.localStorage.setItem(userKey, parseJSON);
  }
  getState(key: string): DataLoadEvent | Clause[] {
    const profile = this.securityService.getProfile;
    if (profile == null) {
      return;
    }
    const userKey = this.getUserKey(profile, key);
    const cache = window.localStorage.getItem(userKey);
    return JSON.parse(cache);
  }
}
