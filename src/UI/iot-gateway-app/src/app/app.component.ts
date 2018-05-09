import { Component, ViewContainerRef } from '@angular/core'
import { ToastsManager } from 'ng2-toastr/ng2-toastr'
import { Router } from '@angular/router'
import { EventAggregator } from './services/eventAggregator'

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent {

  // Sets initial value to true to show loading spinner on first load
  loading = true

  constructor(
    private router: Router,
    private toastr: ToastsManager,
    private eventAggregator: EventAggregator,
    private vcr: ViewContainerRef) {

    toastr.setRootViewContainerRef(vcr)
  }
}
