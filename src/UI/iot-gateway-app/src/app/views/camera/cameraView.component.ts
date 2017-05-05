import { OnInit, Component } from '@angular/core'
import { ActivatedRoute, Router } from '@angular/router'
import { ToastsManager } from 'ng2-toastr/ng2-toastr'
import { Location } from '@angular/common'

import { Camera } from '../../models/camera'
import { CameraService } from '../../services/cameraService'

@Component({
  selector: 'overview',
  templateUrl: './cameraView.component.html',
})
export class CameraViewComponent implements OnInit {

  cameras: Camera[]
  camera: Camera = new Camera({})
  selectedRowIndex: Number = undefined
  isAddMode = false

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private location: Location,
    // private formBuilder: FormBuilder,
    private cameraService: CameraService,
    private toastr: ToastsManager,
  ) {
    this.refresh()
  }

  setAddMode(): void {
    this.selectedRowIndex = undefined
    this.isAddMode = true
    this.camera = new Camera({})
  }

  cancelEdit(): void {
    this.selectedRowIndex = undefined
    this.refresh()
  }

  onSubmit(): void {
    this.cameraService.save(this.camera)
      .subscribe(
      (data) => {
        this.isAddMode = false
        this.toastr.info('camera updated successfully')
        this.refresh()
      },
      )
  }

  delete(camera: Camera, event: Event): void {
    event.stopPropagation()
    this.cameraService.delete(camera.Name)
      .subscribe(
      (data) => {
        this.selectedRowIndex = undefined
        this.toastr.info('Camera removed successfully')
        this.refresh()
      },
      )
  }

  ngOnInit(): void {
    this.camera = new Camera({})
  }

  refresh() {
    this.cameraService.getAll()
      .subscribe(
      (data) => {
        this.cameras = data
      },
      )
  }

  testConnection(camera: Camera, event: Event) {
    event.stopPropagation()
    if (!camera.Address) {
      this.toastr.error('Cannot test connection without valid ip address')
      return
    }
    this.toastr.info('testing connection, please wait')
    this.cameraService.testConnection(camera.Name)
      .subscribe(
      (data) => {
        this.toastr.info(data)
      },
      )
  }

  setClickedRow(i: Number, camera: Camera) {
    this.selectedRowIndex = i
    this.camera = camera
  }
}
