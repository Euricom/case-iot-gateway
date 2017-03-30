import { Component } from '@angular/core'
import { ButtonsModule } from 'ng2-bootstrap'
import { Router } from '@angular/router'

@Component({
  selector: 'overview',
  templateUrl: './cameraView.component.html',
})
export class CameraViewComponent {

  constructor(private router: Router) {

  }
}
