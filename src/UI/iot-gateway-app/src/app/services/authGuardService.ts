// auth-guard.service.ts

import { Injectable } from '@angular/core'
import { Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router'
import { CanActivate } from '@angular/router'
import { AuthService } from './authService'

@Injectable()
export class AuthGuardService implements CanActivate {

  constructor(private authService: AuthService,
    private router: Router) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    if (this.authService.isLoggedIn()) {
      if (route.data) {
        return this.authService.hasRole(Object.keys(route.data).map(key => route.data[key]));
      }
      return true;
    } else {
      this.authService.setLoggedOut();
      this.router.navigateByUrl('/unauthorized')
      return false
    }
  }
}
