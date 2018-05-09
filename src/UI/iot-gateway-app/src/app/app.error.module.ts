import { NgModule, ErrorHandler, OnInit, Injector } from '@angular/core'
import { Response } from '@angular/http'
import { Router } from '@angular/router'
import { ToastsManager, ToastModule } from 'ng2-toastr/ng2-toastr'
import { AuthService } from './services/authService';

@NgModule({
  providers: [{ provide: CustomErrorHandler, useClass: CustomErrorHandler }],
})
export class CustomErrorHandler implements OnInit, ErrorHandler {

  router: Router;
  toastr: ToastsManager;
  authService: AuthService;

  constructor(
    private injector: Injector) {
    this.router = this.injector.get(Router);
    this.toastr = this.injector.get(ToastsManager);
    this.authService = this.injector.get(AuthService);
  }

  ngOnInit() {

  }

  handleError(error: any) {
    console.error(error);
    if (error instanceof Response) {
      switch (error.status) {
        case 0:
          this.toastr.error(`Failed to connect to server. Bad connectivity or server down.`);
          break;
        case 401:
          this.toastr.error(`Token expired.`);
          if (this.router) {
            this.authService.setLoggedOut();
            this.router.navigateByUrl('/unauthorized')
          }
          break
        default:
          this.toastr.error(error.statusText)
      }
    } else {
      this.toastr.error(error)
    }
  }
}
