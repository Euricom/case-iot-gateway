import { Injectable } from '@angular/core'
import { Http, Response } from '@angular/http'
import { Observable } from 'rxjs/Observable'
import 'rxjs/add/operator/map'
import 'rxjs/add/operator/do'
import 'rxjs/add/operator/catch'

import { AuthHttp } from 'angular2-jwt'

import { Node } from '../models/node'
import { Config } from '../../config'

@Injectable()
export class ZwaveService {
  constructor(private authHttp: AuthHttp, private config: Config) {
  }

  getAll(): Observable<Node[]> {
    return this.authHttp.get(`/api/zwave/node`)
      .map((res: Response) => (res.json()))
      .map((data: Array<any>) => data.map((element) => new Node(element)))
  }

  softReset() {
    return this.authHttp.put(`/api/zwave/reset/soft`, undefined);
  }

  heal() {
    return this.authHttp.put('/api/zwave/heal', undefined);
  }

  addNode(secure) {
    return this.authHttp.post(`/api/zwave/node/` + secure, undefined);
  }

  removeNode() {
    return this.authHttp.delete(`/api/zwave/node`, undefined);
  }
}
