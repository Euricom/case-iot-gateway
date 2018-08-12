import { OnInit, Component } from '@angular/core'
import { ActivatedRoute, Router } from '@angular/router'
import { ToastsManager } from 'ng2-toastr/ng2-toastr'
import { Location } from '@angular/common'

import { User } from '../../models/user'
import { UserService } from '../../services/userService'
import { Role } from '../../models/role';

@Component({
    selector: 'overview',
    templateUrl: './UsersView.component.html',
})
export class UsersViewComponent implements OnInit {
    roles: Role[]
    users: User[]
    user: User = new User({})
    selectedRowIndex: Number = undefined
    isAddMode = false

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private location: Location,
        private userService: UserService,
        private toastr: ToastsManager,
    ) {
        this.refresh()
    }

    setAddMode(): void {
        this.selectedRowIndex = undefined
        this.isAddMode = true
        this.user = new User({})
    }

    cancelEdit(): void {
        event.stopPropagation();
        this.selectedRowIndex = undefined
        this.isAddMode = false;
        this.refresh()
    }

    onSubmit(): void {
        var save;
        if (this.selectedRowIndex != undefined) {
            save = this.userService.update(this.user);
        } else {
            save = this.userService.create(this.user);
        }

        save.subscribe(
            (data) => {
                this.isAddMode = false
                this.toastr.info('User updated successfully')
                this.refresh()
            },
        )
    }

    delete(user: User, event: Event): void {
        event.stopPropagation()
        this.userService.delete(user.Username)
            .subscribe(
                (data) => {
                    this.selectedRowIndex = undefined
                    this.toastr.info('User removed successfully')
                    this.refresh()
                },
        )
    }

    ngOnInit(): void {
        this.user = new User({})
    }

    refresh() {
        this.userService.getAll()
            .subscribe(
                (data) => {
                    this.users = data
                },
        )

        this.userService.getRoles()
            .subscribe(
                (data) => {
                    this.roles = data
                },
        )
    }

    generateAccessToken(user: User, event: Event) {
        event.stopPropagation()

        this.userService.generateAccessToken(user.Username)
            .subscribe(
                (data) => {
                    user.AccessToken = data;
                },
        )
    }

    generateCommandToken(user: User, event: Event) {
        event.stopPropagation()

        this.userService.generateCommandToken(user.AccessToken)
            .subscribe(
                (data) => {
                    this.toastr.info(`Command token: ${data}`)
                },
        )
    }

    setClickedRow(i: Number, User: User) {
        this.selectedRowIndex = i
        this.user = User
    }

    validate(user: User): boolean {
        if (!user.Username) {
            return false
        }
        return true
    }

    addRole(user: User, role: string, event) {
        if (event.target.checked) {
            if (!user.Roles) {
                user.Roles = [];
            }

            user.Roles.push(role);
        } else {
            var index = user.Roles.indexOf(role, 0);
            if (index > -1) {
                user.Roles.splice(index, 1);
            }
        }
    }
}
