import { Injectable } from '@angular/core'
import { Http, Response } from '@angular/http'
import { Observable } from 'rxjs/Observable'
import 'rxjs/add/operator/map'
import 'rxjs/add/operator/do'
import 'rxjs/add/operator/catch'

import { AuthHttp } from 'angular2-jwt'

import { DanaLock } from '../models/danaLock'
import { Config } from '../../config'

@Injectable()
export class DanaLockService {
  constructor(private authHttp: AuthHttp, private config: Config) {
  }

  getAll(): Observable<DanaLock[]> {
    return this.authHttp.get(`/api/danalock`)
      .map((res: Response) => (res.json()))
      .map((data: Array<any>) => data.map((element) => new DanaLock(element)))
  }

  getById(id: String): Observable<DanaLock> {
    return this.authHttp.get(`/api/danalock/${id}`)
      .map((res: Response) => (res.json()))
      .map((data) => new DanaLock(data))
  }

  save(danaLock: DanaLock): Observable<DanaLock> {
    if (danaLock.isNew()) {
      return this.create(danaLock)
    } else {
      return this.update(danaLock)
    }
  }

  update(danaLock: DanaLock): Observable<DanaLock> {
    return this.authHttp.put(`/api/danalock`, danaLock)
      .map((res: Response) => (res.json()))
      .map((data) => new DanaLock(data))
  }

  delete(id: String) {
    return this.authHttp.delete(`/api/danalock/${id}`)
  }

  create(danaLock: DanaLock) {
    return this.authHttp.post(`/api/danalock`, danaLock)
      .map((res: Response) => (res.json()))
      .map((data) => new DanaLock(data))
  }

  testConnection(id: String) {
    return this.authHttp.get(`/api/danalock/${id}/testconnection`)
      .map((res: Response) => (res.json()))
  }

  isLocked(id: String) {
    return this.authHttp.get(`/api/danalock/${id}/isLocked`)
      .map((res: Response) => (res.json()))
  }

  switch(id: String, state: String) {
    return this.authHttp.put(`/api/danalock/${id}/switch/${state}`, null)
      .map((res: Response) => (res.json()))
  }
}
