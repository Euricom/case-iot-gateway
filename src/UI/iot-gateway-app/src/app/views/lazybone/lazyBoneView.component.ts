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

  onSubmit(): void {
    this.lazyBoneService.update(this.lazyBone)
      .subscribe(
      (data) => {
        this.toastr.info('lazy bone updated successfully')
        this.refresh()
      },
      (err) => {
        this.toastr.error('error occurred' + err)
      })
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
      (err) => {
        this.toastr.error('error occurred' + err)
      })
  }

  testConnection(lazyBone: LazyBone) {
    if (!lazyBone.Address) {
      this.toastr.error('Cannot test connection without valid ip address')
      return
    }
    this.toastr.info('testing connection, please wait')
    this.lazyBoneService.testConnection(lazyBone.Name)
    .subscribe(
      (data) => {
        if (data) {
          this.toastr.info('Connection succesfull')
        }
      },
      (err) => {
        this.toastr.error('error occurred' + err)
      })
  }

  getCurrentState(lazyBone: LazyBone) {
    if (!lazyBone.Address) {
      this.toastr.error('Cannot get lazy bone state without valid ip address')
      return
    }
    this.toastr.info('getting state, please wait')
    this.lazyBoneService.getCurrentState(lazyBone.Name)
    .subscribe(
      (data) => {
        this.toastr.info(data)
      },
      (err) => {
        this.toastr.error('error occurred' + err)
      })
  }

  switch(lazyBone: LazyBone, state: String) {
    if (!lazyBone.Address) {
      this.toastr.error('Cannot switch without valid ip address')
      return
    }
    this.toastr.info('switching, please wait')
    this.lazyBoneService.switch(lazyBone.Name, state)
    .subscribe(
      (data) => {
        this.toastr.info(data)
      },
      (err) => {
        this.toastr.error('error occurred' + err)
      })
  }

  setClickedRow(i: Number, lazyBone: LazyBone) {
    this.selectedRowIndex = i
    this.lazyBone = lazyBone
  }

}
