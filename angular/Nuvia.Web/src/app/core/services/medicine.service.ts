import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { MedicineInfo } from '../../shared/models/medicine.model';

@Injectable({
  providedIn: 'root'
})
export class MedicineService {

  private baseUrl = environment.apiBaseUrl;

  constructor(private http: HttpClient) {}

  searchMedicine(query: string): Observable<MedicineInfo> {
    const params = new HttpParams().set('query', query);
    return this.http.get<MedicineInfo>(`${this.baseUrl}/medicines/search`, { params });
  }
}