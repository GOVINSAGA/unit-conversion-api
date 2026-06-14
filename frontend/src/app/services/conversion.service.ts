import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface UnitCategoryInfo {
  category: string;
  units: string[];
}

export interface ConversionResponse {
  value: number;
  fromUnit: string;
  toUnit: string;
  result: number;
  category: string;
}

@Injectable({
  providedIn: 'root'
})
export class ConversionService {
  // If running locally in dev mode (4200), hit the .NET server on 5038
  // If running via Docker Compose (where Nginx serves the UI), use the API exposed on 8080
  private readonly apiUrl = window.location.port === '4200' 
    ? 'http://localhost:5038/api' 
    : 'http://localhost:8080/api';
  private readonly http = inject(HttpClient);

  getUnits(): Observable<UnitCategoryInfo[]> {
    return this.http.get<UnitCategoryInfo[]>(`${this.apiUrl}/units`);
  }

  convert(value: number, from: string, to: string): Observable<ConversionResponse> {
    const params = new HttpParams()
      .set('value', value.toString())
      .set('from', from)
      .set('to', to);
      
    return this.http.get<ConversionResponse>(`${this.apiUrl}/convert`, { params });
  }
}
