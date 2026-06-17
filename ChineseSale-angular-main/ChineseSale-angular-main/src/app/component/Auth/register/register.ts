import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { CardModule } from 'primeng/card';
import { UserService } from '../../../service/user.service';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    CommonModule, FormsModule, ButtonModule, InputTextModule,
    PasswordModule, CardModule, RouterLink
  ],
  templateUrl: './register.html',
  styleUrl: './register.scss'
})
export class Register {
  private registerService = inject(UserService);
  private router = inject(Router);

  isLoading = signal(false);
  formError = signal('');

  registerData = {
    FirstName: '',
    LastName: '',
    Email: '',
    Password: '',
    PhoneNumber: '',
    confirmPassword: ''
  };

  onRegister() {
    this.formError.set('');

    if (!this.registerData.FirstName || !this.registerData.LastName || !this.registerData.Email || !this.registerData.Password) {
      this.formError.set('נא למלא את כל השדות הנדרשים');
      return;
    }

    if (this.registerData.Password !== this.registerData.confirmPassword) {
      this.formError.set('הסיסמאות אינן תואמות');
      return;
    }

    this.isLoading.set(true);

    this.registerService.registerUser(this.registerData).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.router.navigate(['/login']);
      },
      error: (err) => {
        this.isLoading.set(false);
        let errorMessage = 'אירעה שגיאה ברישום';

        if (err.error?.errors) {
          const firstKey = Object.keys(err.error.errors)[0];
          errorMessage = err.error.errors[firstKey][0];
        } else if (err.error?.message) {
          errorMessage = err.error.message;
        } else if (typeof err.error === 'string') {
          errorMessage = err.error;
        }

        this.formError.set(errorMessage);
      }
    });
  }
}
