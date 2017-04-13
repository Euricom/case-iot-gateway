import { Component, OnInit } from '@angular/core'
import { AuthService } from '../../services/authService'
import { ToastsManager } from 'ng2-toastr/ng2-toastr'
import { Router, ActivatedRoute } from '@angular/router'
import { Location } from '@angular/common'

@Component({
  selector: 'navigation',
  templateUrl: './navigation.component.html',
})
export class NavigationComponent implements OnInit {
  private isAuthenticated = false

  constructor(private authService: AuthService,
    private toastr: ToastsManager,
    private router: Router,
    private route: ActivatedRoute,
    private location: Location) {
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

    if (this.authService.isLoggedIn()) {
      this.authService.setLoggedIn('admin')
    }
  }

  login(): void {
    this.router.navigateByUrl('/login')
  }

  logout(): void {
    this.authService.setLoggedOut()
    this.router.navigateByUrl('/login')
  }

  currentPageIsLogin() {
    return this.location.path() === '/login'
  }
}
