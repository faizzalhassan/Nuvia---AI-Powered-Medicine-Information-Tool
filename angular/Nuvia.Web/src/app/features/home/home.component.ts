import { Component, OnInit, OnDestroy, ChangeDetectorRef, ViewEncapsulation } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NavbarComponent } from '../../shared/components/navbar/navbar.component';
import { FooterComponent } from '../../shared/components/footer/footer.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, FormsModule, NavbarComponent, FooterComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
  encapsulation: ViewEncapsulation.None
})
export class HomeComponent implements OnInit, OnDestroy {

  heroQuery = '';
  typedPlaceholder = '';

  private medicines = ['Panadol', 'Ibuprofen', 'Cravit', 'Daflon'];
  private medicineIndex = 0;
  private charIndex = 0;
  private isDeleting = false;
  private typeInterval: any;

  faqs = [
    {
      question: 'What is Nuvia?',
      answer: 'Nuvia is an AI-powered medicine information platform that uses Google Gemini to explain medicine data in simple, clear language anyone can understand.',
      open: false
    },
    {
      question: 'Where does Nuvia get its medicine information?',
      answer: 'Nuvia uses Google Gemini AI which is trained on vast medical knowledge bases to provide accurate and relevant medicine information.',
      open: false
    },
    {
      question: 'Is Nuvia a replacement for a doctor?',
      answer: 'No. Nuvia is an information and education tool only. Always consult a qualified healthcare professional for medical advice.',
      open: false
    },
    {
      question: 'Do I need to create an account?',
      answer: 'No. Nuvia works without any account or login. Your recent searches are saved privately on your device.',
      open: false
    },
    {
      question: 'How does the AI explanation work?',
      answer: 'Nuvia uses Google Gemini to explain medicine information in simple language. The AI only explains — it never invents medical facts.',
      open: false
    },
    {
      question: 'Is Nuvia free to use?',
      answer: 'Yes. Nuvia is completely free and open source. No subscription, no account, no hidden fees.',
      open: false
    }
  ];

  features = [
    {
      icon: 'search',
      title: 'Instant Medicine Search',
      desc: 'Search any medicine by brand name or generic name and get results in seconds.'
    },
    {
      icon: 'auto_awesome',
      title: 'AI-Powered Explanations',
      desc: 'Google Gemini explains complex medical information in simple everyday language.'
    },
    {
      icon: 'warning',
      title: 'Warnings & Side Effects',
      desc: 'Clearly understand what to watch out for before taking any medicine.'
    },
    {
      icon: 'history',
      title: 'Recent Searches',
      desc: 'Your search history is saved locally on your device for quick access.'
    },
    {
      icon: 'devices',
      title: 'Works on Any Device',
      desc: 'Nuvia is a Progressive Web App. Use it on mobile, tablet, or desktop.'
    },
    {
      icon: 'lock',
      title: 'No Account Needed',
      desc: 'No login, no registration, no personal data collected. Just search and learn.'
    }
  ];

  constructor(
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.startTypewriter();
  }

  ngOnDestroy() {
    this.stopTypewriter();
  }

  startTypewriter() {
    this.typeInterval = setInterval(() => {
      const current = this.medicines[this.medicineIndex];

      if (!this.isDeleting) {
        this.typedPlaceholder = current.substring(0, this.charIndex + 1);
        this.charIndex++;
        if (this.charIndex === current.length) {
          this.isDeleting = true;
        }
      } else {
        this.typedPlaceholder = current.substring(0, this.charIndex - 1);
        this.charIndex--;
        if (this.charIndex === 0) {
          this.isDeleting = false;
          this.medicineIndex = (this.medicineIndex + 1) % this.medicines.length;
        }
      }
      this.cdr.detectChanges();
    }, 120);
  }

  stopTypewriter() {
    if (this.typeInterval) {
      clearInterval(this.typeInterval);
      this.typeInterval = null;
    }
  }

  onInputFocus() {
    this.stopTypewriter();
    this.typedPlaceholder = '';
  }

  onInputBlur() {
    if (!this.heroQuery.trim()) {
      this.charIndex = 0;
      this.isDeleting = false;
      this.startTypewriter();
    }
  }

  searchFromHero() {
    if (!this.heroQuery.trim()) return;
    this.router.navigate(['/search'], { queryParams: { q: this.heroQuery.trim() } });
  }

  goToSearch() {
    this.router.navigate(['/search']);
  }

  toggleFaq(index: number) {
    this.faqs[index].open = !this.faqs[index].open;
  }
}