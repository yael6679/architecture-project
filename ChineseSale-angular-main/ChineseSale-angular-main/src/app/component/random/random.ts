import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { RandomService } from '../../service/random.service';
import { GiftService } from '../../service/gift.service';
import { Gift } from '../../models/gift.model';

@Component({
  selector: 'app-random',
  standalone: true,
  imports: [CommonModule, ButtonModule],
  templateUrl: './random.html',
  styleUrl: './random.scss',
})
export class Random implements OnInit {
  private randomService = inject(RandomService);
  private giftService = inject(GiftService);

  gifts = signal<Gift[]>([]);
  drawingId = signal<number | null>(null);
  drawResults = signal<Record<number, string>>({});

  ngOnInit() {
    this.giftService.getAllGifts().subscribe({
      next: (data) => this.gifts.set(data),
      error: () => this.gifts.set([])
    });
  }

  onExecuteDraw(giftId: number) {
    this.drawingId.set(giftId);
    this.randomService.runDraw(giftId).subscribe({
      next: (winner) => {
        this.drawingId.set(null);
        const name = winner.user
          ? `${winner.user.firstName} ${winner.user.lastName}`
          : `#${winner.idUser}`;
        this.drawResults.update(prev => ({ ...prev, [giftId]: name }));
      },
      error: () => {
        this.drawingId.set(null);
      }
    });
  }

  getDrawResult(giftId: number): string | null {
    return this.drawResults()[giftId] ?? null;
  }
}
