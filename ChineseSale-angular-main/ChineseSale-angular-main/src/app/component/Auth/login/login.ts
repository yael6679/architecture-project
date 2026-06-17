import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { CardModule } from 'primeng/card';
import { UserService } from '../../../service/user.service';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ButtonModule,
    InputTextModule,
    PasswordModule,
    CardModule,
    RouterLink
  ],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class Login {
  private userService = inject(UserService);
  private router = inject(Router);

  isLoading = signal(false);
  formError = signal('');

  loginData = {
    Email: '',
    Password: ''
  };

  onLogin() {
    this.formError.set('');

    if (!this.loginData.Email || !this.loginData.Password) {
      this.formError.set('נא להזין אימייל וסיסמה');
      return;
    }

    this.isLoading.set(true);

    this.userService.LogInUser(this.loginData).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.router.navigate(['/']);
      },
      error: (err) => {
        this.isLoading.set(false);
        const detail = err.status === 0
          ? 'לא ניתן להתחבר לשרver. ודא/י שה-API פועל.'
          : 'אימייל או סיסמה שגויים.';
        this.formError.set(detail);
      }
    });
  }
}
