import { OnInit, Component } from '@angular/core'
import { ActivatedRoute, Router } from '@angular/router'
import { ToastsManager } from 'ng2-toastr/ng2-toastr'
import { Location } from '@angular/common'

import { Wallmount } from '../../models/wallmount'
import { WallmountService } from '../../services/wallmountService'

@Component({
  selector: 'overview',
  templateUrl: './wallmountView.component.html',
})
export class WallMountViewComponent implements OnInit {

  wallmounts: Wallmount[]
  wallmount: Wallmount = new Wallmount({})
  selectedRowIndex: Number = undefined
  isAddMode = false

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private location: Location,
    // private formBuilder: FormBuilder,
    private wallmountService: WallmountService,
    private toastr: ToastsManager,
  ) {
    this.refresh()
  }

  setAddMode(): void {
    this.selectedRowIndex = undefined
    this.isAddMode = true
    this.wallmount = new Wallmount({})
  }

  cancelEdit(): void {
    this.selectedRowIndex = undefined
    this.refresh()
  }

  onSubmit(): void {
    this.wallmountService.save(this.wallmount)
      .subscribe(
      (data) => {
        this.selectedRowIndex = undefined
        this.isAddMode = false
        this.toastr.info('Wallmount updated successfully')
        this.refresh()
      },
      )
  }

  delete(wallmount: Wallmount, event: Event): void {
    event.stopPropagation()
    this.wallmountService.delete(wallmount.DeviceId)
      .subscribe(
      (data) => {
        this.selectedRowIndex = undefined
        this.toastr.info('Wallmount removed successfully')
        this.refresh()
      },
      )
  }

  ngOnInit(): void {
    this.wallmount = new Wallmount({})
  }

  refresh() {
    this.wallmountService.getAll()
      .subscribe(
      (data) => {
        this.wallmounts = data
      },
      )
  }

  testConnection(wallmount: Wallmount, event: Event) {
    event.stopPropagation()
    if (!this.validate(wallmount)) {
      this.toastr.error('Cannot test connection without valid Node ID')
      return
    }
    this.wallmountService.testConnection(wallmount.DeviceId)
      .subscribe(
      (data) => {
        this.toastr.info(data)
      },
      )
  }

  getState(wallmount: Wallmount, event: Event) {
    event.stopPropagation()
    if (!this.validate(wallmount)) {
      this.toastr.error('Cannot get wallmount state without valid Node ID')
      return
    }
    this.wallmountService.getState(wallmount.DeviceId)
      .subscribe(
      (data) => {
        if (data === 'True') {
          this.toastr.info('Wallmount is ON')
        } else if (data === 'False') {
          this.toastr.info('Wallmount is OFF')
        }
      },
      )
  }

  switch(wallmount: Wallmount, state: String, event: Event) {
    event.stopPropagation()
    if (!this.validate(wallmount)) {
      this.toastr.error('Cannot test connection without valid Node ID')
      return
    }
    this.wallmountService.switch(wallmount.DeviceId, state)
      .subscribe(
      (data) => {
        this.toastr.info(data)
      },
      )
  }

  setClickedRow(i: Number, wallmount: Wallmount) {
    this.selectedRowIndex = i
    this.wallmount = wallmount
  }

  validate(wallmount: Wallmount): boolean {
    if (!wallmount.NodeId) {
      return false
    }
    return true
  }

}
