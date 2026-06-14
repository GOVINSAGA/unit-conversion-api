import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ConversionService, UnitCategoryInfo } from './services/conversion.service';
import { catchError, debounceTime, distinctUntilChanged, Subject, switchMap, tap, of } from 'rxjs';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  private conversionService = inject(ConversionService);

  categories: UnitCategoryInfo[] = [];
  selectedCategory: UnitCategoryInfo | null = null;
  
  fromUnit: string = '';
  toUnit: string = '';
  inputValue: number = 1;
  resultValue: number | null = null;
  
  error: string | null = null;
  isLoading: boolean = false;

  private conversionSubject = new Subject<void>();

  ngOnInit() {
    this.loadUnits();

    // Set up reactive conversion with debounce
    this.conversionSubject.pipe(
      debounceTime(300),
      tap(() => this.error = null),
      switchMap(() => {
        if (!this.isValidForConversion()) return of(null);
        
        this.isLoading = true;
        return this.conversionService.convert(this.inputValue, this.fromUnit, this.toUnit).pipe(
          catchError(err => {
            this.error = err.error?.detail || 'An error occurred during conversion.';
            return of(null);
          })
        );
      })
    ).subscribe(res => {
      this.isLoading = false;
      if (res) {
        this.resultValue = res.result;
      } else if (!this.error) {
        this.resultValue = null;
      }
    });
  }

  loadUnits() {
    this.conversionService.getUnits().subscribe({
      next: (data) => {
        this.categories = data;
        if (this.categories.length > 0) {
          this.selectCategory(this.categories[0]);
        }
      },
      error: (err) => {
        this.error = 'Failed to load units from the API. Is the backend running?';
        console.error(err);
      }
    });
  }

  selectCategory(category: UnitCategoryInfo) {
    this.selectedCategory = category;
    if (category.units.length >= 2) {
      this.fromUnit = category.units[0];
      this.toUnit = category.units[1];
    } else {
      this.fromUnit = category.units[0] || '';
      this.toUnit = category.units[0] || '';
    }
    this.triggerConversion();
  }

  onCategoryChange(event: Event) {
    const select = event.target as HTMLSelectElement;
    const cat = this.categories.find(c => c.category === select.value);
    if (cat) {
      this.selectCategory(cat);
    }
  }

  onInputChange() {
    this.triggerConversion();
  }

  swapUnits() {
    const temp = this.fromUnit;
    this.fromUnit = this.toUnit;
    this.toUnit = temp;
    this.triggerConversion();
  }

  triggerConversion() {
    this.conversionSubject.next();
  }

  private isValidForConversion(): boolean {
    return !!this.fromUnit && !!this.toUnit && typeof this.inputValue === 'number' && !isNaN(this.inputValue);
  }
}
