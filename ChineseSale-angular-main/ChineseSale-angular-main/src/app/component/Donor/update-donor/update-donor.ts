import { Component, effect, EventEmitter, inject, input, Output, signal } from '@angular/core';
import { DonorService } from '../../../service/donor.service';
import { FormsModule } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { Donor } from '../../../models/donor.model';

const EMAIL_REGEX = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

@Component({
  selector: 'app-update-donor',
  standalone: true,
  imports: [ButtonModule, DialogModule, InputTextModule, FormsModule],
  templateUrl: './update-donor.html',
  styleUrl: './update-donor.scss',
})
export class UpdateDonor {
  private donorService = inject(DonorService);

  visible = signal(false);
  saveError = signal('');
  submitted = false;
  @Output() donorSaved = new EventEmitter<void>();

  donor = {
    id: 0,
    firstName: '',
    lastName: '',
    eMail: ''
  };

  donorTest = input<Donor | null>(null);

  constructor() {
    effect(() => {
      const data = this.donorTest();
      if (data) {
        this.donor = {
          id: data.id,
          firstName: data.firstName,
          lastName: data.lastName,
          eMail: data.eMail
        };
        this.submitted = false;
        this.saveError.set('');
        this.visible.set(true);
      }
    });
  }

  saveUpdate() {
    this.submitted = true;
    this.saveError.set('');
    const { firstName, lastName, eMail } = this.donor;

    if (!firstName.trim() || !lastName.trim()) return;
    if (!eMail.trim() || !EMAIL_REGEX.test(eMail.trim())) return;

    this.donorService.updateDonor({
      id: this.donor.id,
      firstName: firstName.trim(),
      lastName: lastName.trim(),
      eMail: eMail.trim()
    }).subscribe({
      next: () => {
        this.donorSaved.emit();
        this.visible.set(false);
        this.submitted = false;
      },
      error: (err) => {
        let detail = 'עדכון התורם נכשל';
        if (err.status === 401 || err.status === 403) {
          detail = 'נדרשת הרשאת Admin. התחבר/י מחדש.';
        } else if (err.error?.message) {
          detail = err.error.message;
        } else if (typeof err.error === 'string' && err.error.trim()) {
          detail = err.error;
        }
        this.saveError.set(detail);
      }
    });
  }
}
