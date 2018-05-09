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
import { Credentials } from '../models/credentials'
import { ChangePassword } from '../models/changepassword'
import { AuthHttp } from 'angular2-jwt'
import { StorageService } from '../services/storageService';
import { Subscription } from 'rxjs/Subscription';

@Injectable()
export class AuthService {

  private loggedInUsername: String
  private subject: Subject<String> = new Subject<String>()

  constructor(
    private http: Http,
    private authHttp: AuthHttp,
    private config: Config,
    private storageService: StorageService) {
    if (tokenNotExpired()) {
      this.setLoggedIn('admin')
    } else {
      this.setLoggedOut()
    }
  }

  getLoggedIn(): Observable<String> {
    return this.subject.asObservable()
  }

  setLoggedIn(username: String) {
    this.loggedInUsername = username
    this.subject.next(this.loggedInUsername)
  }

  setLoggedInByPuk() {
    this.loggedInUsername = 'admin'
    this.subject.next(this.loggedInUsername)
  }

  setLoggedOut() {
    this.logout()
    this.loggedInUsername = undefined
    this.subject.next(this.loggedInUsername)
  }

  isLoggedIn() {
    return tokenNotExpired();
  }

  login(credentials: Credentials, puk): Observable<any> {

    if (credentials && credentials.Username && credentials.Password) {
      return this.http.post(`/api/security/login/`, credentials)
        .map(res => res.json())
    } else {
      if (puk) {
        return this.http.post(`/api/security/loginByPUK/`, { puk })
          .map(res => res.json())
      }
    }
  }

  changePassword(change: ChangePassword) {
    return this.authHttp.put(`/api/security/password`, change)
  }

  logout() {
    this.loggedInUsername = undefined

    this.storageService.removeToken()
    this.storageService.removeUsername()
    this.storageService.removeRoles();
  }

  hasRole(roles: string[]): boolean {
    var currentRoles = this.storageService.getRoles().getValue();
    if (this.isLoggedIn()) {
      return roles.some(function (item) {
        return currentRoles.includes(item);
      });
    }
    return false;
  }
}
