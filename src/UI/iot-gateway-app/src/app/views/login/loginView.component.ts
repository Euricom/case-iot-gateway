import { OnInit, Component, Input } from '@angular/core'
import { ActivatedRoute, Router } from '@angular/router'
import { ToastsManager } from 'ng2-toastr/ng2-toastr'
import { AuthService } from '../../services/authService'
import { Credentials } from '../../models/credentials';
import { User } from '../../models/user';

@Component({
  selector: 'login',
  templateUrl: './loginView.component.html',
})

export class LoginViewComponent implements OnInit {

  username: String
  password: String

  failedLoginTimes = 0
  isPukLoginCollapsed: Boolean = true
  puk: String

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService,
    private toastr: ToastsManager,
  ) {
  }

  ngOnInit(): void {
  }

  onSubmit() {

    const self = this

    const credentials: Credentials = new Credentials({})
    credentials.Username = this.username
    credentials.Password = this.password

    if ((credentials.Username && credentials.Password) || this.puk) {
      this.authService.login(credentials, this.puk)
        .subscribe(
        (data) => {
          localStorage.setItem('token', data)

          if (credentials && credentials.Username && credentials.Password) {
            this.authService.setLoggedIn(credentials.Username)
          } else {
            this.authService.setLoggedInByPuk()
          }

          this.router.navigateByUrl('/settings')
        },
        (error) => {
          this.username = ''
          this.password = ''
          this.failedLoginTimes++
          this.toastr.error('login failed: ' + error)
        },
      )
    }
  }
}
