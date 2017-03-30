import { Component } from '@angular/core'
import { ButtonsModule } from 'ng2-bootstrap'
import { Router } from '@angular/router'

@Component({
  selector: 'overview',
  templateUrl: './hardwareView.component.html',
})
export class HardwareViewComponent {

  constructor(private router: Router) {

  }
}
