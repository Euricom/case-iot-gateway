import { Injectable } from '@angular/core'
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { Observable } from 'rxjs/Observable'

@Injectable()
export class StorageService {
    private roles = new BehaviorSubject<string[]>([]);

    constructor() {
        var roles = localStorage.getItem('roles');
        if (roles) {
            this.roles.next(roles.split(','));
        }
    }

    setUsername(username: string): void {
        localStorage.setItem('username', username);
    }

    removeUsername(): void {
        localStorage.removeItem('username');
    }

    getUsername(): string {
        return localStorage.getItem('username');
    }

    setToken(token: string): void {
        localStorage.setItem('token', token);
    }

    removeToken(): void {
        localStorage.removeItem('token');
    }

    setRoles(roles: string[]): void {
        localStorage.setItem('roles', roles.join(','));
        this.roles.next(roles);
    }

    removeRoles(): void {
        localStorage.removeItem('roles');
        this.roles.next([]);
    }

    getRoles(): BehaviorSubject<string[]> {
        return this.roles;
    }
}