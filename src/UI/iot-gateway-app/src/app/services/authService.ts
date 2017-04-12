import { Injectable } from '@angular/core'
import { Http, Response } from '@angular/http'
import { Observable } from 'rxjs/Observable'
import 'rxjs/add/operator/map'
import 'rxjs/add/operator/do'
import 'rxjs/add/operator/catch'

import { Config } from '../../config'
import { tokenNotExpired } from 'angular2-jwt'


@Injectable()
export class AuthService {
  constructor(private http: Http, private config: Config) {
  }

  isLoggedIn() {
    return tokenNotExpired()
  }

  login(credentials): Observable<any> {
    return this.http.post(`${this.config.baseUrl}/api/security/login/`, credentials)
      .map(res => res.json())
  }

  logout() {
    localStorage.removeItem('token')
  }
}
