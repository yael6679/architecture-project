import { afterNextRender, inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { tap } from 'rxjs/operators';
import { environment } from '../../environment/environment';
import { AddUserDto, AuthResponse, LogInUserDto } from '../models/user.model';


@Injectable({
  providedIn: 'root'
})
export class UserService {
   private http = inject(HttpClient);
  private apiUrl = `${environment.serverUrl}/api/Auth`;

currentUser = signal<AuthResponse | null>(null);

  constructor() {
    afterNextRender(() => {
      const data = localStorage.getItem('user');
      if (data) {
        try {
          const parsed = JSON.parse(data);
          const userData = parsed.user ? parsed.user : parsed;
          this.currentUser.set(userData);
        } catch (e) {
          console.error('שגיאה בקריאת הנתונים', e);
        }
      }
    });
  }
  
    registerUser(user: AddUserDto): Observable<AddUserDto> {
    return this.http.post<AddUserDto>(`${this.apiUrl}/register`, user);
      }

    LogInUser(user: LogInUserDto): Observable<AuthResponse> {
    return this.http.post<any>(`${this.apiUrl}/login`, user).pipe(
      tap((response: any) => {
        const apiUser = response.user ?? response;
        const userData: AuthResponse = {
          id: apiUser.id ?? apiUser.Id,
          firstName: apiUser.firstName ?? apiUser.FirstName,
          lastName: apiUser.lastName ?? apiUser.LastName,
          email: apiUser.email ?? apiUser.Email,
          phoneNumber: apiUser.phoneNumber ?? apiUser.PhoneNumber,
          role: apiUser.role ?? apiUser.Role ?? 'User',
          token: response.token ?? response.Token ?? ''
        };
        this.currentUser.set(userData);
        localStorage.setItem('user', JSON.stringify({ user: userData }));
        if (userData.token) {
          localStorage.setItem('token', userData.token);
        }
      })
    );
  }

      logout() {
  localStorage.removeItem('user');
    localStorage.removeItem('token');
    this.currentUser.set(null);
}
}