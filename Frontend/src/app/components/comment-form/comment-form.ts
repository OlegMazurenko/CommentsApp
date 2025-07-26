import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  ReactiveFormsModule,
  FormBuilder,
  FormGroup,
  Validators
} from '@angular/forms';
import { HttpClient } from '@angular/common/http';


@Component({
  selector: 'app-comment-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './comment-form.html',
  styleUrls: ['./comment-form.css']
})
export class CommentForm {
  form: FormGroup;
  @Input() parentCommentId: number | null = null;

  captchaId: string = '';
  captchaUrl: string = '';
  errorMessage: string = '';
  selectedFiles: File[] = [];

  constructor(private fb: FormBuilder, private http: HttpClient) {
    this.form = this.fb.group({
      userName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      homePage: [''],
      text: ['', Validators.required],
      captchaCode: ['', Validators.required]
    });
  }

  ngOnInit() {
    this.loadCaptcha();
  }

  loadCaptcha() {
    this.http.get('https://localhost:5001/api/captcha', {
      responseType: 'blob',
      observe: 'response'
    }).subscribe(response => {
      const contentDisposition = response.headers.get('Content-Disposition') || '';
      const match = contentDisposition.match(/filename="?([a-f0-9\-]{36})\.png"?/i);

      if (match && match[1]) {
        this.captchaId = match[1];
      } else {
        console.error('Captcha ID not found in Content-Disposition:', contentDisposition);
        this.captchaId = '';
      }

      const reader = new FileReader();
      reader.onloadend = () => {
        this.captchaUrl = reader.result as string;
      };
      reader.readAsDataURL(response.body!);
    });
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (!input.files) return;

    this.selectedFiles = [];

    Array.from(input.files).forEach(file => {
      const isText = file.type === 'text/plain' && file.name.endsWith('.txt');
      const isImage = ['image/jpeg', 'image/png', 'image/gif'].includes(file.type);

      if (!isText && !isImage) {
        alert(`Unsupported file: ${file.name}`);
        return;
      }

      if (isText && file.size > 100_000) {
        alert(`Text file too large: ${file.name}`);
        return;
      }

      this.selectedFiles.push(file);
    });
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
      return new Promise<void>((resolve) => {
        const reader = new FileReader();
        reader.onload = () => {
          const base64 = (reader.result as string).split(',')[1];
          formData.append(`Files[${index}].FileName`, file.name);
          formData.append(`Files[${index}].ContentType`, file.type);
          formData.append(`Files[${index}].Base64Content`, base64);
          resolve();
        };
        reader.readAsDataURL(file);
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
