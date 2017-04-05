import { Injectable } from '@angular/core'
import { Http, Response } from '@angular/http'
import { Observable } from 'rxjs/Observable'
import 'rxjs/add/operator/map'
import 'rxjs/add/operator/do'
import 'rxjs/add/operator/catch'

import { DanaLock } from '../models/danaLock'
import { Config } from '../../config'

@Injectable()
export class DanaLockService {
  constructor(private http: Http, private config: Config) {
  }

  getAll(): Observable<DanaLock[]> {
    return this.http.get(`${this.config.baseUrl}/api/danalock`)
      .map((res: Response) => (res.json()))
      .map((data: Array<any>) => data.map((element) => new DanaLock(element)))
  }

  getById(id: String): Observable<DanaLock> {
    return this.http.get(`${this.config.baseUrl}/api/danalock/${id}`)
      .map((res: Response) => (res.json()))
      .map((data) => new DanaLock(data))
  }

  save(danaLock: DanaLock): Observable<DanaLock> {
    if (danaLock.isNew()) {
      return this.create(danaLock)
    } else {
      return this.update(danaLock.DeviceId, danaLock)
    }
  }

  update(id: String, danaLock: DanaLock): Observable<DanaLock> {
    return this.http.put(`${this.config.baseUrl}/api/danalock`, danaLock)
      .map((res: Response) => (res.json()))
      .map((data) => new DanaLock(data))
  }

  delete(id: String) {
    return this.http.delete(`${this.config.baseUrl}/api/danalock/${id}`)
  }

  create(danaLock: DanaLock) {
    return this.http.post(`${this.config.baseUrl}/api/danalock`, danaLock)
      .map((res: Response) => (res.json()))
      .map((data) => new DanaLock(data))
  }

  testConnection(id: String) {
    return this.http.get(`${this.config.baseUrl}/api/danalock/testconnection/${id}`)
      .map((res: Response) => (res.json()))
  }

  isLocked(id: String) {
    return this.http.get(`${this.config.baseUrl}/api/danalock/isLocked/${id}`)
      .map((res: Response) => (res.json()))
  }

  switch(id: String, state: String) {
    return this.http.put(`${this.config.baseUrl}/api/danalock/switch?deviceid=${id}&state=${state}`, null)
      .map((res: Response) => (res.json()))
  }
}
