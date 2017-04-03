import { Injectable } from '@angular/core'
import { Http, Response } from '@angular/http'
import { Observable } from 'rxjs/Observable'
import 'rxjs/add/operator/map'
import 'rxjs/add/operator/do'
import 'rxjs/add/operator/catch'

import { Camera } from '../models/camera'
import { Config } from '../../config'

@Injectable()
export class CameraService {
  constructor(private http: Http, private config: Config) {
  }

  getAll(): Observable<Camera[]> {
    return this.http.get(`${this.config.baseUrl}/api/camera`)
      .map((res: Response) => (res.json()))
      .map((data: Array<any>) => data.map((element) => new Camera(element)))
  }

  getById(id: String): Observable<Camera> {
    return this.http.get(`${this.config.baseUrl}/api/camera/${id}`)
      .map((res: Response) => (res.json()))
      .map((data) => new Camera(data))
  }

  save(camera: Camera): Observable<Camera> {
    if (camera.isNew()) {
      return this.create(camera)
    } else {
      return this.update(camera.DeviceId, camera)
    }
  }

  update(id: String, camera: Camera): Observable<Camera> {
    return this.http.put(`${this.config.baseUrl}/api/camera`, camera)
      .map((res: Response) => (res.json()))
      .map((data) => new Camera(data))
  }

  delete(id: String) {
    return this.http.delete(`${this.config.baseUrl}/api/camera/${id}`)
  }

  create(camera: Camera) {
    return this.http.post(`${this.config.baseUrl}/api/camera`, camera)
      .map((res: Response) => (res.json()))
      .map((data) => new Camera(data))
  }
}
