import { OnInit, Component, ViewChild, ElementRef } from '@angular/core'
import { ActivatedRoute, Router } from '@angular/router'
import { ToastsManager } from 'ng2-toastr/ng2-toastr'
import { Location } from '@angular/common'

import { Node } from '../../models/node'
import { ZwaveService } from '../../services/zwaveService'
import { ModalComponent } from '../../components/modal/modal.component';

@Component({
  templateUrl: './zwaveView.component.html',
})
export class ZwaveViewComponent implements OnInit {
  @ViewChild('modal') el: ElementRef;

  node: Node;

  formSubmitted = false
  // form: FormGroup

  nodes: Node[]

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private location: Location,
    // private formBuilder: FormBuilder,
    private zwaveService: ZwaveService,
    private toastr: ToastsManager,
  ) {
    this.refresh()
  }

  ngOnInit(): void {


  }

  refresh() {
    this.zwaveService.getAll()
      .subscribe(
        (data) => {
          this.nodes = data
        },
        (err) => {
          this.toastr.error('error occurred' + err)
        })
  }

  softReset() {
    this.zwaveService.softReset().subscribe((data) => {
      this.toastr.info('Soft reset finished')
    }, )
  }

  heal() {
    this.zwaveService.heal().subscribe((data) => {
      this.toastr.info('Heal initiated')
    }, )
  }

  addNode(secure) {
    this.zwaveService.addNode(secure).subscribe((data) => {
      this.toastr.info('Awaiting manual action, please click the button on the device to include.')
    }, )
  }

  removeNode() {
    this.zwaveService.removeNode().subscribe((data) => {
      this.toastr.info('Awaiting manual action, please click the button on the device to exclude.')
    }, )
  }

  onClickCancel() {
    this.location.back()
  }

  show(node: Node, event: Event) {
    event.stopPropagation()
    this.node = node;
    (<any>this.el).show();
  }
}
