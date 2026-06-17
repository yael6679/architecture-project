import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { ToolbarModule } from 'primeng/toolbar';
import { InputTextModule } from 'primeng/inputtext';
import { CategoryService } from '../../../service/category.service';
import { Category, AddCategoryDto } from '../../../models/category.model';


@Component({
  selector: 'app-category-list',
  standalone: true,
  imports: [CommonModule, FormsModule, TableModule, ButtonModule, DialogModule, ToolbarModule, InputTextModule],
  templateUrl: './category-list.html',
  styleUrl: './category-list.scss'
})
export class CategoryList implements OnInit {
  private categoryService = inject(CategoryService);
  isSaving: boolean = false;

  categories = signal<Category[]>([]);
  
  newCategory: AddCategoryDto = { name: '', color: '#3b82f6' };
  selectedCategory: Category = { id: 0, name: '', color: '' };

  addDialog: boolean = false;
  editDialog: boolean = false;
  submitted: boolean = false;

  ngOnInit() {
    this.loadCategories();
  }

  loadCategories() {
    this.categoryService.getAllCategory().subscribe(data => this.categories.set(data));
  }

  openAddDialog() {
    this.newCategory = { name: '', color: '#3b82f6' }; 
    this.submitted = false;
    this.addDialog = true;
  }

  saveNewCategory() {
    if (this.isSaving) return;
    this.submitted = true;
    if (!this.newCategory.name?.trim() || !this.newCategory.color) {
      return;
    }
      this.isSaving = true;
      this.categoryService.addCategory(this.newCategory).subscribe({
        next: () => {
          this.addDialog = false;
          this.loadCategories();
          this.isSaving = false;
        },
        error: () => {
          this.isSaving = false;  
        }
      });
  }

  openEditDialog(category: Category) {
    this.selectedCategory = { ...category };
    this.submitted = false;
    this.editDialog = true;
  }

  saveUpdateCategory() {
    this.submitted = true;
    if (!this.selectedCategory.name?.trim()) {
      return;
    }
    this.categoryService.updateCategory(this.selectedCategory).subscribe({
        next: () => {
          this.editDialog = false;
          this.loadCategories();
        },
        error: () => {}
      });
  }

  deleteCategory(id: number) {
    this.categoryService.deleteCategory(id).subscribe({
     next: () => this.loadCategories(),
     error: () => {}
    });
  }
}
