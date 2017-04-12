import { Injectable } from '@angular/core'
import { Http, Response } from '@angular/http'
import { Observable } from 'rxjs/Observable'
import 'rxjs/add/operator/map'
import 'rxjs/add/operator/do'
import 'rxjs/add/operator/catch'

import { AuthHttp } from 'angular2-jwt'

import { Device } from '../models/device'
import { Config } from '../../config'

@Injectable()
export class HardwareService {
  constructor(private authHttp: AuthHttp, private config: Config) {
  }

  getAll(): Observable<Device[]> {
    return this.authHttp.get(`${this.config.baseUrl}/api/hardware`)
      .map((res: Response) => (res.json()))
      .map((data: Array<any>) => data.map((element) => new Device(element)))
  }

  getById(id: String): Observable<Device> {
    return this.authHttp.get(`${this.config.baseUrl}/api/hardware/${id}`)
      .map((res: Response) => (res.json()))
      .map((data) => new Device(data))
  }

  save(device: Device): Observable<Device> {
    if (device.isNew()) {
      return this.create(device)
    } else {
      return this.update(device.DeviceId, device)
    }
  }

  update(id: String, device: Device): Observable<Device> {
    return this.authHttp.put(`${this.config.baseUrl}/api/hardware/${id}`, device)
      .map((res: Response) => (res.json()))
      .map((data) => new Device(data))
  }

  delete(name: String) {
    return this.authHttp.delete(`${this.config.baseUrl}/api/hardware/${name}`)
  }

  create(device: Device) {
    return this.authHttp.post(`${this.config.baseUrl}/api/hardware`, device)
      .map((res: Response) => (res.json()))
      .map((data) => new Device(data))
  }
}
