import { Component, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';

declare const bootstrap: any;

@Component({
  selector: 'app-root',
  templateUrl: './app.html',
  standalone: false,
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('Frontend');

  selectedFileTextContent: string | null = null;

  constructor(private http: HttpClient) {}

  openTextFile(fileId: number) {
    this.selectedFileTextContent = null;

    this.http.get(`/api/files/${fileId}/text`, {
      responseType: 'text'
    }).subscribe({
      next: (text) => {
        this.selectedFileTextContent = text;
        const modal = new bootstrap.Modal(document.getElementById('textFileModal'));
        modal.show();
      },
      error: (error) => {
        console.error('Failed to load text file:', error);
        this.selectedFileTextContent = 'Failed to load file.';
      }
    });
  }

  closeModal() {
    this.selectedFileTextContent = null;
  }
}
