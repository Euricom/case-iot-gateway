import { OnInit, Component } from '@angular/core'
import { ActivatedRoute, Router } from '@angular/router'
import { ToastsManager } from 'ng2-toastr/ng2-toastr'
import { Location } from '@angular/common'

import { LazyBone } from '../../models/lazyBone'
import { LazyBoneService } from '../../services/lazyBoneService'

@Component({
  selector: 'overview',
  templateUrl: './lazyBoneView.component.html',
})
export class LazyBonesViewComponent implements OnInit {

  lazyBones: LazyBone[]
  lazyBone: LazyBone = new LazyBone({})
  selectedRowIndex: Number = undefined
  isAddMode = false

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private location: Location,
    // private formBuilder: FormBuilder,
    private lazyBoneService: LazyBoneService,
    private toastr: ToastsManager,
  ) {
    this.refresh()
  }

  setAddMode(): void {
    this.selectedRowIndex = undefined
    this.isAddMode = true
    this.lazyBone = new LazyBone({})
  }

  cancelEdit(): void {
    this.selectedRowIndex = undefined
    this.refresh()
  }

  onSubmit(): void {
    this.lazyBoneService.save(this.lazyBone)
      .subscribe(
      (data) => {
        this.isAddMode = false
        this.toastr.info('Lazy bone updated successfully')
        this.refresh()
      },
      )
  }

  delete(lazyBone: LazyBone, event: Event): void {
    event.stopPropagation()
    this.lazyBoneService.delete(lazyBone.Name)
      .subscribe(
      (data) => {
        this.selectedRowIndex = undefined
        this.toastr.info('Lazy bone removed successfully')
        this.refresh()
      },
      )
  }

  ngOnInit(): void {
    this.lazyBone = new LazyBone({})
  }

  refresh() {
    this.lazyBoneService.getAll()
      .subscribe(
      (data) => {
        this.lazyBones = data
      },
      )
  }

  testConnection(lazyBone: LazyBone, event: Event) {
    event.stopPropagation()
    if (!this.validate(lazyBone)) {
      return
    }
    this.toastr.info('testing connection, please wait')
    this.lazyBoneService.testConnection(lazyBone.Name)
      .subscribe(
      (data) => {
        if (data) {
          this.toastr.info('Connection succesfull')
        } else {
          this.toastr.error('Connection failed')
        }
      },
      )
  }

  getCurrentState(lazyBone: LazyBone, event: Event) {
    event.stopPropagation()
    if (!this.validate(lazyBone)) {
      return
    }
    this.toastr.info('getting state, please wait')
    this.lazyBoneService.getCurrentState(lazyBone.Name)
      .subscribe(
      (data) => {
        this.toastr.info(data)
      },
      )
  }

  switch(lazyBone: LazyBone, state: String, event: Event) {
    event.stopPropagation()
    if (!this.validate(lazyBone)) {
      return
    }
    this.toastr.info('switching, please wait')
    this.lazyBoneService.switch(lazyBone.Name, state)
      .subscribe(
      (data) => {
        this.toastr.info(data)
      },
      )
  }

  testChangeLightIntensity(lazyBone: LazyBone, event: Event) {
    event.stopPropagation()
    if (!this.validate(lazyBone)) {
      return
    }
    this.toastr.info('changing light intensity 3 times, please wait')
    this.lazyBoneService.testChangeLightIntensity(lazyBone.Name)
      .subscribe(
      (data) => {
        this.toastr.info(data)
      },
      )
  }

  setClickedRow(i: Number, lazyBone: LazyBone) {
    this.selectedRowIndex = i
    this.lazyBone = lazyBone
  }

  validate(lazyBone: LazyBone) {
    if (!lazyBone.Address || !lazyBone.Port) {
      this.toastr.error('Cannot test without valid ip address and valid port')
      return false
    }
    return true
  }

}
