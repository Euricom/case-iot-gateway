import { Injectable } from '@angular/core'
import { Http, Response } from '@angular/http'
import { Observable } from 'rxjs/Observable'
import 'rxjs/add/operator/map'
import 'rxjs/add/operator/do'
import 'rxjs/add/operator/catch'

import { AuthHttp } from 'angular2-jwt'

import { Settings } from '../models/settings'
import { Config } from '../../config'

@Injectable()
export class SettingsService {
  constructor(private authHttp: AuthHttp, private config: Config) {
  }

  getSettings(): Observable<Settings> {
    return this.authHttp.get(`/api/settings`)
      .map((res: Response) => (res.json()))
  }

  saveSettings(settings: Settings): Observable<Settings> {
    return this.authHttp.put(`/api/settings`, settings)
  }
}
