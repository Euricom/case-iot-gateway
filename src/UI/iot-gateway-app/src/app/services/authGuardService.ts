// auth-guard.service.ts

import { Injectable } from '@angular/core'
import { Router } from '@angular/router'
import { CanActivate } from '@angular/router'
import { AuthService } from './authService'

@Injectable()
export class AuthGuardService implements CanActivate {

  constructor(private authService: AuthService,
    private router: Router) { }

  canActivate() {
    if (this.authService.isLoggedIn()) {
      return true
    } else {
      this.router.navigateByUrl('/unauthorized')
      return false
    }
  }
}
