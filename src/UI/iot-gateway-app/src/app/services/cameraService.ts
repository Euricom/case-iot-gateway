import { Injectable } from '@angular/core'
import { Http, Response } from '@angular/http'
import { Observable } from 'rxjs/Observable'
import 'rxjs/add/operator/map'
import 'rxjs/add/operator/do'
import 'rxjs/add/operator/catch'

import { AuthHttp } from 'angular2-jwt'

import { Camera } from '../models/camera'
import { Config } from '../../config'

@Injectable()
export class CameraService {
  constructor(private authHttp: AuthHttp, private config: Config) {
  }

  getAll(): Observable<Camera[]> {
    return this.authHttp.get(`/api/camera`)
      .map((res: Response) => (res.json()))
      .map((data: Array<any>) => data.map((element) => new Camera(element)))
  }

  getById(id: String): Observable<Camera> {
    return this.authHttp.get(`/api/camera/${id}`)
      .map((res: Response) => (res.json()))
      .map((data) => new Camera(data))
  }

  save(camera: Camera): Observable<Camera> {
    if (camera.isNew()) {
      return this.create(camera)
    } else {
      return this.update(camera)
    }
  }

  update(camera: Camera): Observable<Camera> {
    return this.authHttp.put(`/api/camera`, camera)
      .map((res: Response) => (res.json()))
      .map((data) => new Camera(data))
  }

  delete(id: String) {
    return this.authHttp.delete(`/api/camera/${id}`)
  }

  create(camera: Camera) {
    return this.authHttp.post(`/api/camera`, camera)
      .map((res: Response) => (res.json()))
      .map((data) => new Camera(data))
  }

  testConnection(id: String) {
    return this.authHttp.get(`/api/camera/testconnection/${id}`)
      .map((res: Response) => (res.json()))
  }
}
