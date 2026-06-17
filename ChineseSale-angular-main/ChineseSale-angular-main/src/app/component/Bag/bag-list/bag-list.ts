import { Component, inject, signal, OnInit, computed, PLATFORM_ID } from '@angular/core';
import { TableModule } from 'primeng/table';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { Router, RouterLink } from '@angular/router';
import { BagService } from '../../../service/bag.service';
import { UserService } from '../../../service/user.service';
import { CreateBagDto } from '../../../models/bag.model';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { FormsModule } from '@angular/forms';
import { TooltipModule } from 'primeng/tooltip';

@Component({
  selector: 'app-bag',
  standalone: true,
  imports: [
    TableModule, CommonModule, ButtonModule, DialogModule,
    InputTextModule, FormsModule, TooltipModule, RouterLink
  ],
  templateUrl: './bag-list.html',
  styleUrl: './bag-list.scss',
})
export class Baglist implements OnInit {
  private bagService = inject(BagService);
  private userService = inject(UserService);
  private router = inject(Router);
  private platformId = inject(PLATFORM_ID);

  paymentInfo = { cardNumber: '', expiry: '', cvv: '', holderName: '' };
  bag = signal<any[]>([]);
  displayPaymentDialog = signal<boolean>(false);
  isCheckingOut = signal(false);

  totalPrice = computed(() =>
    this.bag().reduce((sum, item) =>
      sum + (item.gift?.price || item.Gift?.price || 0) * (item.quantity || item.Quantity || 0), 0)
  );

  ngOnInit() {
    if (isPlatformBrowser(this.platformId)) {
      this.initUserBag();
    }
  }

  private getUserId(): number | null {
    const user = this.userService.currentUser();
    return user?.id ?? null;
  }

  private initUserBag() {
    const userId = this.getUserId();
    if (!userId) {
      this.router.navigate(['/login']);
      return;
    }
    this.loadBag(userId);
  }

  loadBag(userId: number) {
    this.bagService.getBagsByUserId(userId).subscribe({
      next: (data) => this.bag.set(data),
      error: () => this.bag.set([])
    });
  }

  plusQuent(item: any) {
    const payload: CreateBagDto = {
      idUser: Number(item.idUser || item.IdUser),
      idGift: Number(item.idGift || item.IdGift),
      quantity: 1
    };

    this.bagService.addBag(payload).subscribe({
      next: () => {
        const giftId = payload.idGift;
        this.bag.update(items => items.map(i =>
          (i.idGift || i.IdGift) === giftId
            ? { ...i, quantity: (Number(i.quantity || i.Quantity) || 0) + 1, Quantity: (Number(i.Quantity || i.quantity) || 0) + 1 }
            : i
        ));
      },
      error: () => {}
    });
  }

  minusQuent(item: any) {
    const currentQty = Number(item.quantity || item.Quantity || 0);
    if (currentQty <= 1) return;

    const payload: CreateBagDto = {
      idUser: Number(item.idUser || item.IdUser),
      idGift: Number(item.idGift || item.IdGift),
      quantity: -1
    };

    this.bagService.addBag(payload).subscribe({
      next: () => {
        const giftId = payload.idGift;
        this.bag.update(items => items.map(i =>
          (i.idGift || i.IdGift) === giftId
            ? { ...i, quantity: currentQty - 1, Quantity: currentQty - 1 }
            : i
        ));
      },
      error: () => {}
    });
  }

  deleteBag(id: number) {
    this.bagService.deleteBag(id).subscribe({
      next: () => this.bag.update(prev => prev.filter(item => (item.id || item.Id) !== id)),
      error: () => {}
    });
  }

  openPaymentDialog() {
    if (this.bag().length === 0) {
      return;
    }
    this.displayPaymentDialog.set(true);
  }

  processPurchase() {
    const userId = this.getUserId();
    if (!userId) return;

    this.isCheckingOut.set(true);
    this.displayPaymentDialog.set(false);

    this.bagService.ProcessCheckout(userId).subscribe({
      next: () => {
        this.isCheckingOut.set(false);
        this.bag.set([]);
        this.router.navigate(['/order-history']);
      },
      error: () => {
        this.isCheckingOut.set(false);
      }
    });
  }
}
