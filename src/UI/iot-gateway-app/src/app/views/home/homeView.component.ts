import { Component, OnInit } from '@angular/core'
import { ButtonsModule } from 'ng2-bootstrap'
import { Router } from '@angular/router'
import { RouterInitializer } from '@angular/router/src/router_module';
import { StorageService } from '../../services/storageService';

@Component({
  selector: 'overview',
  templateUrl: './homeView.component.html',
})
export class HomeViewComponent implements OnInit {
  username: string;

  ngOnInit(): void {
    this.username = this.storageService.getUsername();
  }

  constructor(private router: RouterInitializer,
    private storageService: StorageService) {

  }
}
