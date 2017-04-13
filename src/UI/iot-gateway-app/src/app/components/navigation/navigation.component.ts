import { Component, OnInit } from '@angular/core'
import { AuthService } from '../../services/authService'
import { ToastsManager } from 'ng2-toastr/ng2-toastr'
import { Router } from '@angular/router'

@Component({
  selector: 'navigation',
  templateUrl: './navigation.component.html',
})
export class NavigationComponent implements OnInit {
  private isAuthenticated = false

  constructor(private authService: AuthService,
    private toastr: ToastsManager,
    private router: Router) {
  }

  ngOnInit(): void {
    this.authService.getLoggedIn().subscribe((username: String) => {
      if (username) {
        this.isAuthenticated = true
        this.toastr.success(`Welcome ${username}`)
      } else {
        this.isAuthenticated = false
      }
    })
  }

  login(): void {
    this.router.navigateByUrl('/login')
  }

  logout(): void {
    this.authService.setLoggedOut()
    this.router.navigateByUrl('/login')
  }
}
