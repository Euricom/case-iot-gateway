import { Component } from '@angular/core'
import { ButtonsModule } from 'ng2-bootstrap'
import { Router } from '@angular/router'

@Component({
  selector: 'overview',
  templateUrl: './settingsView.component.html',
})
export class SettingsViewComponent {

  constructor(private router: Router) {

  }
}
