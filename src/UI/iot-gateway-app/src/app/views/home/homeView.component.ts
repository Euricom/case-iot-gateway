import { Component } from '@angular/core'
import { ButtonsModule } from 'ng2-bootstrap'
import { Router } from '@angular/router'

@Component({
  selector: 'overview',
  templateUrl: './homeView.component.html',
})
export class HomeViewComponent {

  constructor(private router: Router) {

  }
}
