import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DonorService } from '../../../service/donor.service';
import { Donor } from '../../../models/donor.model';
import { ButtonModule } from 'primeng/button';
import { AccordionModule } from 'primeng/accordion';
import { InputTextModule } from 'primeng/inputtext';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { SelectButtonModule } from 'primeng/selectbutton';
import { AddDonor } from '../add-donor/add-donor';
import { UpdateDonor } from '../update-donor/update-donor';

@Component({
  selector: 'app-donor',
  standalone: true,
  imports: [
    CommonModule, ButtonModule, AddDonor, UpdateDonor,
    AccordionModule, FormsModule, SelectButtonModule, InputTextModule,
    IconFieldModule, InputIconModule
  ],
  templateUrl: './DonorComponent.html',
  styleUrl: './DonorComponent.scss',
})
export class DonorComponent implements OnInit {
  private donorService = inject(DonorService);

  donors = signal<Donor[]>([]);
  selectedDonor = signal<Donor | null>(null);
  searchType: 'name' | 'email' | 'gift' = 'name';

  searchOptions = [
    { label: 'שם', value: 'name' as const },
    { label: 'אימייל', value: 'email' as const },
    { label: 'מתנה', value: 'gift' as const }
  ];

  ngOnInit() {
    this.loadDonors();
  }

  loadDonors() {
    this.donorService.donorWithGifts().subscribe({
      next: (data) => this.donors.set(this.normalizeDonors(data)),
      error: () => this.donors.set([])
    });
  }

  private normalizeDonors(data: Donor | Donor[] | null | undefined): Donor[] {
    if (!data) return [];
    const list = Array.isArray(data) ? data : [data];
    return list.map(d => ({
      ...d,
      eMail: d.eMail ?? (d as any).email ?? (d as any).EMail ?? ''
    }));
  }

  deleteDonor(donor: Donor) {
    if (donor.gifts && donor.gifts.length > 0) {
      return;
    }

    this.donorService.deleteDonor(donor.id).subscribe({
      next: () => this.donors.update(curr => curr.filter(d => d.id !== donor.id)),
      error: () => {}
    });
  }

  openUpdate(donor: Donor) {
    this.selectedDonor.set(donor);
  }

  onSearch(event: Event) {
    const term = (event.target as HTMLInputElement).value.trim();
    if (!term) {
      this.loadDonors();
      return;
    }

    const handlers = {
      name: () => this.donorService.getDonorsByName(term),
      email: () => this.donorService.getDonorsByEmail(term),
      gift: () => this.donorService.getDonorsByGift(term)
    };

    handlers[this.searchType]().subscribe({
      next: (data) => this.donors.set(this.normalizeDonors(data)),
      error: () => this.donors.set([])
    });
  }
}
