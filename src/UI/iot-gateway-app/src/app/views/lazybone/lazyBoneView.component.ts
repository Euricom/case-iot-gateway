import { Component } from '@angular/core'
import { ButtonsModule } from 'ng2-bootstrap'
import { Router } from '@angular/router'

@Component({
  selector: 'overview',
  templateUrl: './lazyBoneView.component.html',
})
export class LazyBonesViewComponent {

  constructor(private router: Router) {

  }
}
