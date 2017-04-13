import { Injectable } from '@angular/core'
import { Http, Response } from '@angular/http'
import { Observable } from 'rxjs/Observable'
import 'rxjs/add/operator/map'
import 'rxjs/add/operator/do'
import 'rxjs/add/operator/catch'
import { Subject } from 'rxjs/Subject'
import { tokenNotExpired } from 'angular2-jwt'
import { Config } from '../../config'
import { User } from '../models/user'

@Injectable()
export class AuthService {

  private loggedInUsername: String
  private subject: Subject<String> = new Subject<String>()

  constructor(private http: Http, private config: Config) {
    if (tokenNotExpired()) {
      this.setLoggedIn('admin')
    } else {
      this.setLoggedOut()
    }
  }

  getLoggedIn(): Observable<string> {
    return this.subject.asObservable()
  }

  setLoggedIn(username: String) {
    this.loggedInUsername = username
    this.subject.next(this.loggedInUsername)
  }

  setLoggedOut() {
    this.logout()
    this.loggedInUsername = undefined
    this.subject.next(this.loggedInUsername)
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
