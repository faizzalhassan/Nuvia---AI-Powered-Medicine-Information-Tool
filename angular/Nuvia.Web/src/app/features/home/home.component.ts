import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {

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

  stats = [
    { number: '10,000+', label: 'Medicines Searchable' },
    { number: 'AI', label: 'Powered by Gemini' },
    { number: '100%', label: 'Free to Use' },
    { number: '0', label: 'Account Required' }
  ];

  constructor(private router: Router) {}

  goToSearch() {
    this.router.navigate(['/search']);
  }

  toggleFaq(index: number) {
    this.faqs[index].open = !this.faqs[index].open;
  }
}