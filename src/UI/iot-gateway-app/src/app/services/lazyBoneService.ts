import { Injectable } from '@angular/core'
import { Http, Response } from '@angular/http'
import { Observable } from 'rxjs/Observable'
import 'rxjs/add/operator/map'
import 'rxjs/add/operator/do'
import 'rxjs/add/operator/catch'

import { AuthHttp } from 'angular2-jwt'

import { LazyBone } from '../models/lazyBone'
import { Config } from '../../config'

@Injectable()
export class LazyBoneService {
  constructor(private authHttp: AuthHttp, private config: Config) {
  }

  getAll(): Observable<LazyBone[]> {
    return this.authHttp.get(`${this.config.baseUrl}/api/lazyBone`)
      .map((res: Response) => (res.json()))
      .map((data: Array<any>) => data.map((element) => new LazyBone(element)))
  }

  getById(id: String): Observable<LazyBone> {
    return this.authHttp.get(`${this.config.baseUrl}/api/lazyBone/${id}`)
      .map((res: Response) => (res.json()))
      .map((data) => new LazyBone(data))
  }

  save(lazyBone: LazyBone): Observable<LazyBone> {
    if (lazyBone.isNew()) {
      return this.create(lazyBone)
    } else {
      return this.update(lazyBone)
    }
  }

  update(lazyBone: LazyBone): Observable<LazyBone> {
    return this.authHttp.put(`${this.config.baseUrl}/api/lazyBone`, lazyBone)
      .map((res: Response) => (res.json()))
      .map((data) => new LazyBone(data))
  }

  delete(id: String) {
    return this.authHttp.delete(`${this.config.baseUrl}/api/lazyBone/${id}`)
  }

  create(lazyBone: LazyBone) {
    return this.authHttp.post(`${this.config.baseUrl}/api/lazyBone`, lazyBone)
      .map((res: Response) => (res.json()))
      .map((data) => new LazyBone(data))
  }

  testConnection(id: String) {
    return this.authHttp.get(`${this.config.baseUrl}/api/lazyBone/testconnection/${id}`)
      .map((res: Response) => (res.json()))
  }

  getCurrentState(id: String) {
    return this.authHttp.get(`${this.config.baseUrl}/api/lazyBone/getstate/${id}`)
      .map((res: Response) => (res.json()))
  }

  switch(id: String, state: String) {
    return this.authHttp.put(`${this.config.baseUrl}/api/lazyBone/switch?devicename=${id}&state=${state}`, null)
      .map((res: Response) => (res.json()))
  }

  testChangeLightIntensity(id: String) {
    return this.authHttp.put(`${this.config.baseUrl}/api/lazyBone/testchangelightintensity?devicename=${id}`, null)
      .map((res: Response) => (res.json()))
  }
}
