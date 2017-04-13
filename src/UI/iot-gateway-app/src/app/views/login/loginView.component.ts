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

    const credentials: Credentials = new Credentials({})
    credentials.Username = this.username
    credentials.Password = this.password

    this.authService.login(credentials)
      .subscribe(
      (data) => {
        localStorage.setItem('token', data)
        this.authService.setLoggedIn(credentials.Username)
        this.router.navigateByUrl('/settings')
      },
      (error) => {
        this.toastr.error('login failed: ' + error)
      },
    )
  }
}
