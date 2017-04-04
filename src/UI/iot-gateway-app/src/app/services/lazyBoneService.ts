import { Injectable } from '@angular/core'
import { Http, Response } from '@angular/http'
import { Observable } from 'rxjs/Observable'
import 'rxjs/add/operator/map'
import 'rxjs/add/operator/do'
import 'rxjs/add/operator/catch'

import { LazyBone } from '../models/lazyBone';
import { Config } from '../../config'

@Injectable()
export class LazyBoneService {
  constructor(private http: Http, private config: Config) {
  }

  getAll(): Observable<LazyBone[]> {
    return this.http.get(`${this.config.baseUrl}/api/lazyBone`)
      .map((res: Response) => (res.json()))
      .map((data: Array<any>) => data.map((element) => new LazyBone(element)))
  }

  getById(id: String): Observable<LazyBone> {
    return this.http.get(`${this.config.baseUrl}/api/lazyBone/${id}`)
      .map((res: Response) => (res.json()))
      .map((data) => new LazyBone(data))
  }

  save(lazyBone: LazyBone): Observable<LazyBone> {
    if (lazyBone.isNew()) {
      return this.create(lazyBone)
    } else {
      return this.update(lazyBone.DeviceId, lazyBone)
    }
  }

  update(id: String, lazyBone: LazyBone): Observable<LazyBone> {
    return this.http.put(`${this.config.baseUrl}/api/lazyBone`, lazyBone)
      .map((res: Response) => (res.json()))
      .map((data) => new LazyBone(data))
  }

  delete(id: String) {
    return this.http.delete(`${this.config.baseUrl}/api/lazyBone/${id}`)
  }

  create(lazyBone: LazyBone) {
    return this.http.post(`${this.config.baseUrl}/api/lazyBone`, lazyBone)
      .map((res: Response) => (res.json()))
      .map((data) => new LazyBone(data))
  }

  testConnection(id: String) {
    return this.http.get(`${this.config.baseUrl}/api/lazyBone/testconnection/${id}`)
      .map((res: Response) => (res.json()))
  }
}
