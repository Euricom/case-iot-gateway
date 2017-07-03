import { Injectable } from '@angular/core'
import { Http, Response } from '@angular/http'
import { Observable } from 'rxjs/Observable'
import 'rxjs/add/operator/map'
import 'rxjs/add/operator/do'
import 'rxjs/add/operator/catch'

import { AuthHttp } from 'angular2-jwt'

import { Log } from '../models/log'
import { LogLine } from '../models/logline'
import { Config } from '../../config'

@Injectable()
export class LogService {
  constructor(private authHttp: AuthHttp, private config: Config) {
  }

  queryLogs(): Observable<string[]> {
    return this.authHttp.get(`/api/logs`)
      .map((res: Response) => (res.json()))
  }

  getLog(day: string): Observable<Log> {
    return this.authHttp.get(`/api/logs/${day}`)
      .map((res: Response) => (res.json()))
  }

  getOpenZWaveLog(): Observable<string[]> {
    return this.authHttp.get(`/api/logs_openzwave/`)
      .map((res: Response) => (res.json()))
  }
}
