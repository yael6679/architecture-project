import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { AddDonorDto, Donor } from '../models/donor.model';
import { environment } from '../../environment/environment';


@Injectable({
  providedIn: 'root'
})
export class DonorService {

   private http = inject(HttpClient);
  private apiUrl = `${environment.serverUrl}/api/Doner`;

  getAllDonors(): Observable<Donor[]> {
    return this.http.get<Donor[]>(this.apiUrl);
  }

    addDonor(donor: AddDonorDto): Observable<Donor> {
    return this.http.post<Donor>(this.apiUrl, donor);
      }
    deleteDonor(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
  
  updateDonor(donor: Donor): Observable<Donor> {
    return this.http.put<Donor>(`${this.apiUrl}`, donor);
  }

donorWithGifts(): Observable<Donor[]> {
    return this.http.get<Donor[]>(`${this.apiUrl}/withGifts`);
  }

getDonorsByName(name: string): Observable<Donor[]> {
  return this.http.get<Donor[]>(`${this.apiUrl}/doner/name`, {
    params: { name: name }
  });
}

getDonorsByEmail(email: string): Observable<Donor[]> {
  return this.http.get<Donor[]>(`${this.apiUrl}/doner/email`, {
    params: { email: email }
  });
}

getDonorsByGift(giftName: string): Observable<Donor[]> {
  return this.http.get<Donor[]>(`${this.apiUrl}/doner/gift`, {
    params: { giftName: giftName }
  });
}

getAllDonorsWithGifts(): Observable<any[]> {
  return this.http.get<any[]>(`${this.apiUrl}/withGifts`);
}

}