import { Component, Input } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { DomSanitizer } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-comment-form',
  standalone: true,
  templateUrl: './comment-form.html',
  imports: [CommonModule, ReactiveFormsModule]
})
export class CommentForm {
  @Input() parentCommentId?: number;

  form: FormGroup;
  selectedFiles: File[] = [];
  captchaUrl: string = '';
  captchaId: string = '';
  errorMessage: string = '';

  constructor(private fb: FormBuilder, private http: HttpClient, private sanitizer: DomSanitizer) {
    this.form = this.fb.group({
      userName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      homePage: [''],
      text: ['', Validators.required],
      captchaCode: ['', Validators.required]
    });

    this.loadCaptcha();
  }

  loadCaptcha() {
    this.http.get('https://localhost:5001/api/captcha', {
      responseType: 'blob',
      observe: 'response'
    }).subscribe(response => {
      const blob = response.body!;
      const url = URL.createObjectURL(blob);
      this.captchaUrl = this.sanitizer.bypassSecurityTrustUrl(url) as string;

      const contentDisposition = response.headers.get('Content-Disposition') || '';
      const match = contentDisposition.match(/filename="?([a-f0-9-]{36})\.png"?/i);
      this.captchaId = match?.[1] ?? '';
    });
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files) {
      this.selectedFiles = Array.from(input.files);
    }
  }

  async submit() {
    if (this.form.invalid) return;

    const formData = new FormData();
    formData.append('userName', this.form.value.userName);
    formData.append('email', this.form.value.email);
    formData.append('homePage', this.form.value.homePage ?? '');
    formData.append('text', this.form.value.text);
    formData.append('captchaId', this.captchaId);
    formData.append('captchaCode', this.form.value.captchaCode);
    formData.append('parentCommentId', this.parentCommentId?.toString() ?? '');

    const fileReadPromises = this.selectedFiles.map((file, index) => {
      return new Promise<void>((resolve, reject) => {
        const reader = new FileReader();
        reader.onload = () => {
          const result = reader.result as string;

          // Проверка и безопасное извлечение base64
          const base64 = result.includes(',') ? result.split(',')[1] : '';

          if (!base64) {
            console.warn(`Base64 content not found for file: ${file.name}`);
            return reject(`Base64 not extracted for ${file.name}`);
          }

          formData.append(`Files[${index}].FileName`, file.name);
          formData.append(`Files[${index}].ContentType`, file.type);
          formData.append(`Files[${index}].Base64Content`, base64);
          resolve();
        };

        reader.onerror = () => {
          reject(`Error reading file: ${file.name}`);
        };

        reader.readAsDataURL(file); // Поддерживает и изображения, и текст
      });
    });

    await Promise.all(fileReadPromises);

    this.http.post('https://localhost:5001/api/comments', formData).subscribe({
      next: () => {
        this.form.reset();
        this.selectedFiles = [];
        this.errorMessage = '';
        this.loadCaptcha();
      },
      error: (error) => {
        this.errorMessage = 'Error submitting comment.';
        console.error(error);
        this.loadCaptcha();
      }
    });
  }
}
