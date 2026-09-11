import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class StorageService {

  private readonly RECENT_SEARCHES_KEY = 'nuvia_recent_searches';
  private readonly MAX_RECENT = 8;

  getRecentSearches(): string[] {
    const data = localStorage.getItem(this.RECENT_SEARCHES_KEY);
    return data ? JSON.parse(data) : [];
  }

  addRecentSearch(query: string): void {
    const searches = this.getRecentSearches();
    const filtered = searches.filter(s => s.toLowerCase() !== query.toLowerCase());
    const updated = [query, ...filtered].slice(0, this.MAX_RECENT);
    localStorage.setItem(this.RECENT_SEARCHES_KEY, JSON.stringify(updated));
  }

  clearRecentSearches(): void {
    localStorage.removeItem(this.RECENT_SEARCHES_KEY);
  }
}