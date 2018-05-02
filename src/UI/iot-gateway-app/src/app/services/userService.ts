import { Injectable } from '@angular/core'
import { Http, Response } from '@angular/http'
import { Observable } from 'rxjs/Observable'
import 'rxjs/add/operator/map'
import 'rxjs/add/operator/do'
import 'rxjs/add/operator/catch'

import { AuthHttp } from 'angular2-jwt'

import { User } from '../models/user'
import { Config } from '../../config'
import { Role } from '../models/role';

@Injectable()
export class UserService {
  constructor(private authHttp: AuthHttp, private config: Config) {
  }

  getAll(): Observable<User[]> {
    return this.authHttp.get(`/api/users`)
      .map((res: Response) => (res.json()))
      .map((data: Array<any>) => data.map((element) => new User(element)))
  }

  get(): Observable<User> {
    return this.authHttp.get(`/api/users/me`)
      .map((res: Response) => (res.json()))
      .map((data) => new User(data))
  }

  update(user: User): Observable<User> {
    return this.authHttp.put(`/api/users`, user)
      .map((res: Response) => (res.json()))
      .map((data) => new User(data))
  }

  delete(username: String) {
    return this.authHttp.delete(`/api/users/${username}`)
  }

  create(user: User) {
    return this.authHttp.post(`/api/users`, user)
      .map((res: Response) => (res.json()))
      .map((data) => new User(data))
  }

  generateAccessToken(username: String) {
    return this.authHttp.put(`/api/users/${username}/access-token`, null)
      .map((res: Response) => (res.json()))
  }

  getRoles(): Observable<Role[]> {
    return this.authHttp.get(`/api/users/roles`)
      .map((res: Response) => (res.json()))
      .map((data: Array<any>) => data.map((element) => new Role(element)))
  }
}
