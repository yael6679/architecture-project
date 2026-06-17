import { Component, EventEmitter, inject, Output, signal } from '@angular/core';
import { DonorService } from '../../../service/donor.service';
import { FormsModule } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';

const EMAIL_REGEX = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

@Component({
  selector: 'app-add-donor',
  standalone: true,
  imports: [FormsModule, InputTextModule, ButtonModule, DialogModule],
  templateUrl: './add-donor.html',
  styleUrl: './add-donor.scss',
})
export class AddDonor {
  private donorService = inject(DonorService);

  visible = signal(false);
  saveError = signal('');
  submitted = false;
  @Output() donorSaved = new EventEmitter<void>();

  newDonor = {
    firstName: '',
    lastName: '',
    eMail: ''
  };

  showDialog() {
    this.submitted = false;
    this.saveError.set('');
    this.newDonor = { firstName: '', lastName: '', eMail: '' };
    this.visible.set(true);
  }

  saveDonor() {
    this.submitted = true;
    this.saveError.set('');
    const { firstName, lastName, eMail } = this.newDonor;

    if (!firstName.trim()) return;
    if (!lastName.trim()) return;
    if (!eMail.trim() || !EMAIL_REGEX.test(eMail.trim())) return;

    this.donorService.addDonor({
      firstName: firstName.trim(),
      lastName: lastName.trim(),
      eMail: eMail.trim()
    }).subscribe({
      next: () => {
        this.donorSaved.emit();
        this.visible.set(false);
        this.submitted = false;
        this.newDonor = { firstName: '', lastName: '', eMail: '' };
      },
      error: (err) => {
        this.saveError.set(this.resolveErrorMessage(err, 'הוספת התורם נכשלה'));
      }
    });
  }

  private resolveErrorMessage(err: { status?: number; error?: unknown }, fallback: string): string {
    if (err.status === 401 || err.status === 403) {
      return 'נדרשת הרשאת Admin. התחבר/י מחדש.';
    }
    const body = err.error;
    if (typeof body === 'string' && body.trim()) return body;
    if (body && typeof body === 'object') {
      const message = (body as { message?: string }).message;
      if (message) return message;
      const validationErrors = Object.values(body as Record<string, unknown>)
        .flatMap(v => (Array.isArray(v) ? v : [v]))
        .filter(v => typeof v === 'string') as string[];
      if (validationErrors.length) return validationErrors.join(', ');
    }
    return fallback;
  }
}
