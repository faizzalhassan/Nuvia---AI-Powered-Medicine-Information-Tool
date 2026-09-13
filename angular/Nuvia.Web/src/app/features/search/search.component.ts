import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { MedicineService } from '../../core/services/medicine.service';
import { StorageService } from '../../core/services/storage.service';
import { MedicineInfo, RelatedVariant } from '../../shared/models/medicine.model';
import { NavbarComponent } from '../../shared/components/navbar/navbar.component';

@Component({
  selector: 'app-search',
  standalone: true,
  imports: [CommonModule, FormsModule, NavbarComponent],
  templateUrl: './search.component.html',
  styleUrl: './search.component.css'
})
export class SearchComponent implements OnInit, OnDestroy {

  query = '';
  isLoading = false;
  errorMessage = '';
  medicine: MedicineInfo | null = null;
  recentSearches: string[] = [];

  loaderMessages = [
    'Looking up medicine information...',
    'Analyzing with Google Gemini AI...',
    'Checking warnings and side effects...',
    'Preparing your results...',
    'Almost ready...'
  ];
  currentMessage = '';
  private messageInterval: any;
  private messageIndex = 0;
  private lastSearchedQuery = '';

  constructor(
    private medicineService: MedicineService,
    private storageService: StorageService,
    private router: Router,
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.recentSearches = this.storageService.getRecentSearches();

    this.route.queryParams.subscribe(params => {
      const q = params['q'];
      if (q && q.trim() && q.trim() !== this.lastSearchedQuery) {
        this.lastSearchedQuery = q.trim();
        this.query = q.trim();
        this.executeSearch();
      }
    });
  }

  ngOnDestroy() {
    this.stopMessageCycle();
  }

  search() {
    if (!this.query.trim()) return;

    this.lastSearchedQuery = this.query.trim();

    this.router.navigate(['/search'], {
      queryParams: { q: this.query.trim() },
      queryParamsHandling: 'replace'
    });

    this.executeSearch();
  }

  executeSearch() {
    this.isLoading = true;
    this.errorMessage = '';
    this.medicine = null;

    this.startMessageCycle();

    this.storageService.addRecentSearch(this.query.trim());
    this.recentSearches = this.storageService.getRecentSearches();

    this.medicineService.searchMedicine(this.query.trim()).subscribe({
      next: (data: MedicineInfo) => {
        this.medicine = data;
        this.isLoading = false;
        this.stopMessageCycle();
        this.cdr.detectChanges();
      },
      error: (err) => {
        if (err.status === 404) {
          this.errorMessage = 'No medicine found. Please check the name and try again.';
        } else {
          this.errorMessage = 'Something went wrong. Please try again.';
        }
        this.isLoading = false;
        this.stopMessageCycle();
        this.cdr.detectChanges();
      }
    });
  }

  searchVariant(variant: RelatedVariant) {
    this.query = variant.name;
    this.search();
  }

  startMessageCycle() {
    this.messageIndex = 0;
    this.currentMessage = this.loaderMessages[0];
    this.messageInterval = setInterval(() => {
      this.messageIndex = (this.messageIndex + 1) % this.loaderMessages.length;
      this.currentMessage = this.loaderMessages[this.messageIndex];
      this.cdr.detectChanges();
    }, 1000);
  }

  stopMessageCycle() {
    if (this.messageInterval) {
      clearInterval(this.messageInterval);
      this.messageInterval = null;
    }
    this.currentMessage = '';
  }

  searchRecent(term: string) {
    this.query = term;
    this.search();
  }

  clearRecent() {
    this.storageService.clearRecentSearches();
    this.recentSearches = [];
  }

  goHome() {
    this.router.navigate(['/']);
  }
}