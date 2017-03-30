import { Component } from '@angular/core'
import { ButtonsModule } from 'ng2-bootstrap'
import { Router } from '@angular/router'

@Component({
  selector: 'overview',
  templateUrl: './danalocksView.component.html',
})
export class DanaLocksViewComponent {

  constructor(private router: Router) {

  }
}
