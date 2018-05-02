import { Component, OnInit, OnDestroy } from '@angular/core'
import { AuthService } from '../../services/authService'
import { ToastsManager } from 'ng2-toastr/ng2-toastr'
import { Router, ActivatedRoute } from '@angular/router'
import { Location } from '@angular/common'
import { Observable } from 'rxjs/Observable';
import { StorageService } from '../../services/storageService';
import { Role } from '../../models/role';
import { Subscription } from 'rxjs/Subscription';

@Component({
  selector: 'navigation',
  templateUrl: './navigation.component.html',
  styleUrls: ['./navigation.component.css']
})
export class NavigationComponent implements OnInit, OnDestroy {
  public isAuthenticated = false
  private roles: string[];
  private roleSubscription: Subscription;

  constructor(private authService: AuthService,
    private toastr: ToastsManager,
    private router: Router,
    private route: ActivatedRoute,
    private location: Location,
    private storageService: StorageService) {
  }

  ngOnInit(): void {
    this.authService.getLoggedIn().subscribe((username: String) => {
      if (username) {
        this.isAuthenticated = true
        this.toastr.success(`Welcome ${username}`)
      } else {
        this.isAuthenticated = false
      }
    })

    if (this.authService.isLoggedIn()) {
      this.authService.setLoggedIn('admin')
    }

    this.storageService.getRoles().subscribe(r => {
      this.roles = r;
    });
  }

  ngOnDestroy(): void {
    this.storageService.getRoles().unsubscribe();
  }

  login(): void {
    this.router.navigateByUrl('/login')
  }

  logout(): void {
    this.authService.setLoggedOut()
    this.router.navigateByUrl('/login')
  }

  currentPageIsLogin() {
    return this.location.path() === '/login'
  }

  hasRole(currentRoles: string[], roles: string[]): boolean {
    if (currentRoles) {
      return roles.some(function (item) {
        return currentRoles.includes(item);
      });
    }
    return false;
  }
}
