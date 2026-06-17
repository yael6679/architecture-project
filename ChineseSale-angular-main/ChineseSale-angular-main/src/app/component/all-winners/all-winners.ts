import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Winner } from '../../models/user.model';
import { RandomService } from '../../service/random.service';
import { TableModule } from 'primeng/table';

@Component({
  selector: 'app-all-winners',
  standalone: true,
  imports: [CommonModule, TableModule],
  templateUrl: './all-winners.html',
  styleUrl: './all-winners.scss',
})
export class AllWinners implements OnInit {
  private randomService = inject(RandomService);
  allWinners = signal<Winner[]>([]);

  ngOnInit() {
    this.loadWinner();
  }

  loadWinner() {
    this.randomService.getWinners().subscribe({
      next: (data) => this.allWinners.set(data),
      error: () => this.allWinners.set([])
    });
  }
}
