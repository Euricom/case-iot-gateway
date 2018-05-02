import { OnInit, Component } from '@angular/core'
import { ActivatedRoute, Router } from '@angular/router'
import { ToastsManager } from 'ng2-toastr/ng2-toastr'
import { Location } from '@angular/common'
import { Settings } from '../../models/settings'
import { AuthService } from '../../services/authService'
import { ChangePassword } from '../../models/changePassword'

@Component({
  selector: 'overview',
  templateUrl: './accountView.component.html',
})
export class AccountViewComponent {
  currentPassword: string
  password: string
  password2: string

  constructor(private router: Router,
    private route: ActivatedRoute,
    private location: Location,
    private toastr: ToastsManager,
    private authService: AuthService) {

  }

  onSubmitPassword(): void {

    // validate
    if (this.password && this.password2) {
      if (this.password !== this.password2) {
        this.toastr.error('passwords do not match')
        return
      } else {
        this.authService.changePassword({ Old: this.currentPassword, New: this.password })
          .subscribe(
            (data) => {
              this.toastr.info('Password saved successfully')
            },
            (err) => {
              this.toastr.error('error occurred' + err)
            })
      }
    }
  }
}
