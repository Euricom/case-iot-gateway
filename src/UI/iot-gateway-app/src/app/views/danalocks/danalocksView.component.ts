import { OnInit, Component } from '@angular/core'
import { ActivatedRoute, Router } from '@angular/router'
import { ToastsManager } from 'ng2-toastr/ng2-toastr'
import { Location } from '@angular/common'

import { DanaLock } from '../../models/danaLock'
import { DanaLockService } from '../../services/danaLockService'

@Component({
  selector: 'overview',
  templateUrl: './danaLocksView.component.html',
})
export class DanaLocksViewComponent implements OnInit {

  danaLocks: DanaLock[]
  danaLock: DanaLock = new DanaLock({})
  selectedRowIndex: Number = undefined

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private location: Location,
    // private formBuilder: FormBuilder,
    private danaLockService: DanaLockService,
    private toastr: ToastsManager,
  ) {
    this.refresh()
  }

  onSubmit(): void {
    this.danaLockService.update(this.danaLock.DeviceId, this.danaLock)
      .subscribe(
      (data) => {
        this.toastr.info('DanaLock updated successfully')
        this.refresh()
      },
      (err) => {
        this.toastr.error('error occurred' + err)
      })
  }

  ngOnInit(): void {
    this.danaLock = new DanaLock({})
  }

  refresh() {
    this.danaLockService.getAll()
      .subscribe(
      (data) => {
        this.danaLocks = data
      },
      (err) => {
        this.toastr.error('error occurred' + err)
      })
  }

  testConnection(danaLock: DanaLock) {
    if (!danaLock.NodeId) {
      this.toastr.error('Cannot test connection without valid Node ID')
      return
    }
    this.danaLockService.testConnection(danaLock.DeviceId)
      .subscribe(
      (data) => {
        this.toastr.info(data)
      },
      (err) => {
        this.toastr.error('error occurred' + err)
      })
  }

  getCurrentState(danaLock: DanaLock) {
    if (!danaLock.NodeId) {
      this.toastr.error('Cannot get danalock state valid Node ID')
      return
    } this.danaLockService.getCurrentState(danaLock.DeviceId)
      .subscribe(
      (data) => {
        this.toastr.info(data)
      },
      (err) => {
        this.toastr.error('error occurred' + err)
      })
  }

  switch(danaLock: DanaLock, state: String) {
    if (!danaLock.NodeId) {
      this.toastr.error('Cannot test connection without valid Node ID')
      return
    }
    this.danaLockService.switch(danaLock.DeviceId, state)
      .subscribe(
      (data) => {
        this.toastr.info(data)
      },
      (err) => {
        this.toastr.error('error occurred' + err)
      })
  }

  setClickedRow(i: Number, danaLock: DanaLock) {
    this.selectedRowIndex = i
    this.danaLock = danaLock
  }

}
