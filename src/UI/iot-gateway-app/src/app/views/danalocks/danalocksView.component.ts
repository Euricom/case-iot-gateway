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
  isAddMode = false

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

  setAddMode(): void {
    this.selectedRowIndex = undefined
    this.isAddMode = true
    this.danalock = new DanaLock({})
  }

  cancelEdit(): void {
    this.selectedRowIndex = undefined
    this.refresh()
  }

  onSubmit(): void {
    this.danaLockService.save(this.danalock)
      .subscribe(
      (data) => {
        this.isAddMode = false
        this.toastr.info('DanaLock updated successfully')
        this.refresh()
      },
      )
  }

  delete(danaLock: DanaLock, event: Event): void {
    event.stopPropagation()
    this.danaLockService.delete(danaLock.Name)
      .subscribe(
      (data) => {
        this.selectedRowIndex = undefined
        this.toastr.info('DanaLock removed successfully')
        this.refresh()
      },
      )
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
      )
  }

  testConnection(danaLock: DanaLock, event: Event) {
    event.stopPropagation()
    if (!this.validate(danaLock)) {
      this.toastr.error('Cannot test connection without valid Node ID')
      return
    }
    this.danaLockService.testConnection(danaLock.Name)
      .subscribe(
      (data) => {
        this.toastr.info(data)
      },
      )
  }

  isLocked(danaLock: DanaLock, event: Event) {
    event.stopPropagation()
    if (!this.validate(danaLock)) {
      this.toastr.error('Cannot get danalock state without valid Node ID')
      return
    }
    this.danaLockService.isLocked(danaLock.Name)
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

  switch(danaLock: DanaLock, state: String, event: Event) {
    event.stopPropagation()
    if (!this.validate(danaLock)) {
      this.toastr.error('Cannot test connection without valid Node ID')
      return
    }
    this.danaLockService.switch(danaLock.Name, state)
      .subscribe(
      (data) => {
        this.toastr.info(data)
      },
      )
  }

  setClickedRow(i: Number, danaLock: DanaLock) {
    this.selectedRowIndex = i
    this.danalock = danaLock
  }

  validate(danalock: DanaLock): boolean {
    if (!danalock.NodeId) {
      return false
    }
    return true
  }

}
