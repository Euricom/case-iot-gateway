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
        this.isAddMode = false
        this.toastr.info('Wallmount updated successfully')
        this.refresh()
      },
      (err) => {
        this.toastr.error('error occurred' + err)
      })
  }

  delete(wallmount: Wallmount, event: Event): void {
    event.stopPropagation()
    this.wallmountService.delete(wallmount.Name)
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
    this.wallmountService.testConnection(wallmount.Name)
      .subscribe(
      (data) => {
        this.toastr.info(data)
      },
      )
  }

  getState(wallmount: Wallmount) {
    if (!this.validate(wallmount)) {
      this.toastr.error('Cannot get wallmount state without valid Node ID')
      return
    }
    this.wallmountService.getState(wallmount.Name)
      .subscribe(
      (data) => {
        if (data === 'True') {
          this.toastr.info('DanaLock door is locked')
        } else if (data === 'False') {
          this.toastr.info('DanaLock door is unlocked')
        }
      },
      )
  }

  switch(wallmount: Wallmount, state: String) {
    if (!this.validate(wallmount)) {
      this.toastr.error('Cannot test connection without valid Node ID')
      return
    }
    this.wallmountService.switch(wallmount.Name, state)
      .subscribe(
      (data) => {
        this.toastr.info(data)
      },
      (err) => {
        this.toastr.error('error occurred' + err)
      })
  }

  setClickedRow(i: Number, wallmount: Wallmount) {
    this.selectedRowIndex = i
    this.wallmount = wallmount
  }

  validate(danalock: Wallmount): boolean {
    if (!danalock.NodeId) {
      return false
    }
    return true
  }

}
