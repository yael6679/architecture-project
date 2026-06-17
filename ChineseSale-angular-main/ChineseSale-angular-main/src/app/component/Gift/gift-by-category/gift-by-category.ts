import { Component, EventEmitter, input, Output } from '@angular/core';
import { Category } from '../../../models/category.model';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-gift-by-category',
  standalone: true,
  imports: [ButtonModule, CommonModule],
  templateUrl: './gift-by-category.html',
  styleUrl: './gift-by-category.scss',
})
export class GiftByCategory {

categories = input<Category[]>([]);

@Output() onCategoryChange = new EventEmitter<number | null>();

  selectedId: number | null = null;

  selectCategory(id: number | null) {
    this.selectedId = id;
    this.onCategoryChange.emit(id);
  }
}
