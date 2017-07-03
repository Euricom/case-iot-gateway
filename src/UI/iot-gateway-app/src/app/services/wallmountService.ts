import { Injectable } from '@angular/core'
import { Http, Response } from '@angular/http'
import { Observable } from 'rxjs/Observable'
import 'rxjs/add/operator/map'
import 'rxjs/add/operator/do'
import 'rxjs/add/operator/catch'

import { AuthHttp } from 'angular2-jwt'

import { Wallmount } from '../models/wallmount'
import { Config } from '../../config'

@Injectable()
export class WallmountService {
  constructor(private authHttp: AuthHttp, private config: Config) {
  }

  getAll(): Observable<Wallmount[]> {
    return this.authHttp.get(`/api/wallmount`)
      .map((res: Response) => (res.json()))
      .map((data: Array<any>) => data.map((element) => new Wallmount(element)))
  }

  getById(id: String): Observable<Wallmount> {
    return this.authHttp.get(`/api/wallmount/${id}`)
      .map((res: Response) => (res.json()))
      .map((data) => new Wallmount(data))
  }

  save(wallmount: Wallmount): Observable<Wallmount> {
    if (wallmount.isNew()) {
      return this.create(wallmount)
    } else {
      return this.update(wallmount)
    }
  }

  update(wallmount: Wallmount): Observable<Wallmount> {
    return this.authHttp.put(`/api/wallmount`, wallmount)
      .map((res: Response) => (res.json()))
      .map((data) => new Wallmount(data))
  }

  delete(id: String) {
    return this.authHttp.delete(`/api/wallmount/${id}`)
  }

  create(wallmount: Wallmount) {
    return this.authHttp.post(`/api/wallmount`, wallmount)
      .map((res: Response) => (res.json()))
      .map((data) => new Wallmount(data))
  }

  testConnection(id: String) {
    return this.authHttp.get(`/api/wallmount/testconnection/${id}`)
      .map((res: Response) => (res.json()))
  }

  getState(id: String) {
    return this.authHttp.get(`/api/wallmount/getState/${id}`)
      .map((res: Response) => (res.json()))
  }

  switch(id: String, state: String) {
    return this.authHttp.put(`/api/wallmount/switch?devicename=${id}&state=${state}`, null)
      .map((res: Response) => (res.json()))
  }
}
