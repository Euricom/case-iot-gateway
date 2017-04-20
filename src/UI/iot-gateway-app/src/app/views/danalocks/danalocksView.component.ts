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

  danalocks: DanaLock[]
  danalock: DanaLock = new DanaLock({})
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
    this.danaLockService.save(this.danalock)
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
    this.danalock = new DanaLock({})
  }

  refresh() {
    this.danaLockService.getAll()
      .subscribe(
      (data) => {
        this.danalocks = data
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
    this.danaLockService.testConnection(danaLock.Name)
      .subscribe(
      (data) => {
        this.toastr.info(data)
      },
      (err) => {
        this.toastr.error('error occurred' + err)
      })
  }

  isLocked(danaLock: DanaLock) {
    if (!danaLock.NodeId) {
      this.toastr.error('Cannot get danalock state without valid Node ID')
      return
    } this.danaLockService.isLocked(danaLock.Name)
      .subscribe(
      (data) => {
        if (data === 'True') {
          this.toastr.info('DanaLock door is locked')
        } else if (data === 'False') {
          this.toastr.info('DanaLock door is unlocked')
        }
      },
      (err) => {
        this.toastr.error('error occurred while requesting door lock state' + err)
      })
  }

  switch(danaLock: DanaLock, state: String) {
    if (!danaLock.NodeId) {
      this.toastr.error('Cannot test connection without valid Node ID')
      return
    }
    this.danaLockService.switch(danaLock.Name, state)
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
    this.danalock = danaLock
  }

}
