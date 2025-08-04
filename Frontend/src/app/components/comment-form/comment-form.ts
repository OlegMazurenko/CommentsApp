import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'app-comment-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './comment-form.html'
})
export class CommentForm implements OnInit {
  @Input() parentCommentId?: number;

  form: FormGroup;
  selectedFiles: File[] = [];
  warnings: string[] = [];
  errorMessage = '';
  xhtmlError = '';
  isSubmitting = false;
  submitErrorDetail = '';
  captchaUrl = '';
  captchaId = '';
  previewText = '';
  showPreview = false;

  constructor(private fb: FormBuilder, private http: HttpClient, private cdRef: ChangeDetectorRef) {
    this.form = this.fb.group({
      userName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      homePage: [''],
      text: ['', Validators.required],
      captchaCode: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.loadCaptcha();

    this.form.get('text')?.valueChanges.subscribe(value => {
      if (this.showPreview) {
        this.updatePreview(value);
      }
    });
  }

  loadCaptcha(): void {
    this.http.get('/api/captcha', {
      responseType: 'blob',
      observe: 'response'
    }).subscribe({
      next: response => {
        const contentDisposition = response.headers.get('Content-Disposition') || '';
        const match = contentDisposition.match(/filename="?(.+?)\.png"?/);
        this.captchaId = match ? match[1] : '';

        const blob = response.body;
        if (blob) {
          this.captchaUrl = URL.createObjectURL(blob);
          this.cdRef.detectChanges();
        }
      },
      error: error => console.error('Failed to load CAPTCHA:', error)
    });
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (!input.files) return;

    this.selectedFiles = [];
    this.warnings = [];

    for (const file of Array.from(input.files)) {
      if (file.type === 'text/plain') {
        if (file.size > 100_000) {
          this.warnings.push(`${file.name} exceeds the limit of 100KB and will be ignored.`);
          continue;
        }
      } else if (file.type.startsWith('image/')) {
        if (!['image/jpeg', 'image/png', 'image/gif'].includes(file.type)) {
          this.warnings.push(`${file.name} is not a supported image format and will be ignored.`);
          continue;
        }
      } else {
        this.warnings.push(`${file.name} is not a supported file type and will be ignored.`);
        continue;
      }

      const reader = new FileReader();
      reader.onload = () => {
        const base64 = (reader.result as string).split(',')[1];
        if (!base64) {
          this.warnings.push(`${file.name} is empty or could not be read.`);
          return;
        }

        this.selectedFiles.push(file);
      };
      reader.readAsDataURL(file);
    }
  }

  insertTag(tag: string): void {
    const textarea = document.getElementById('text') as HTMLTextAreaElement;
    const start = textarea.selectionStart;
    const end = textarea.selectionEnd;
    const original = this.form.value.text || '';
    const open = `<${tag}>`;
    const close = `</${tag}>`;

    this.form.patchValue({
      text: original.slice(0, start) + open + original.slice(start, end) + close + original.slice(end)
    });
  }

  insertLink(): void {
    const textarea = document.getElementById('text') as HTMLTextAreaElement;
    const start = textarea.selectionStart;
    const end = textarea.selectionEnd;
    const original = this.form.value.text || '';
    const selected = original.slice(start, end);
    const template = `<a href="" title="">${selected}</a>`;

    this.form.patchValue({
      text: original.slice(0, start) + template + original.slice(end)
    });
  }

  togglePreview(): void {
    this.showPreview = !this.showPreview;
    if (this.showPreview) {
      this.updatePreview(this.form.value.text);
    }
  }

  updatePreview(value: string): void {
    this.previewText = value;
  }

  isValidXhtml(text: string): boolean {
    const allowedTags = ['a', 'code', 'i', 'strong'];

    const tagRegex = /<\/?([a-zA-Z0-9]+)(\s[^>]*)?>/g;
    let match: RegExpExecArray | null;

    while ((match = tagRegex.exec(text)) !== null) {
      const tag = match[1].toLowerCase();

      if (!allowedTags.includes(tag)) {
        this.xhtmlError = `Tag <${tag}> is not allowed.`;
        return false;
      }

      if (tag === 'a' && match[0][1] !== '/') {
        const attrs = match[2] ?? '';
        if (!attrs.includes('href=') || !attrs.includes('title=')) {
          this.xhtmlError = '<a> tag must include both href and title attributes.';
          return false;
        }
      }
    }

    const stack: string[] = [];
    const tags = [...text.matchAll(/<\/?([a-zA-Z0-9]+)(\s[^>]*)?>/g)];

    for (const tagMatch of tags) {
      const tag = tagMatch[1].toLowerCase();
      const isClosing = tagMatch[0].startsWith('</');

      if (!allowedTags.includes(tag)) {
        this.xhtmlError = `Tag <${tag}> is not allowed.`;
        return false;
      }

      if (!isClosing) {
        stack.push(tag);
      } else {
        const last = stack.pop();
        if (last !== tag) {
          this.xhtmlError = `Unmatched closing tag </${tag}>.`;
          return false;
        }
      }
    }

    if (stack.length > 0) {
      this.xhtmlError = `Unclosed tag <${stack.pop()}>.`;
      return false;
    }

    this.xhtmlError = '';
    return true;
  }

  async submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const text = this.form.value.text;
    if (!this.isValidXhtml(text)) {
      return;
    }

    this.errorMessage = '';
    this.submitErrorDetail = '';
    this.isSubmitting = true;
    this.cdRef.detectChanges();

    const formData = new FormData();
    formData.append('userName', this.form.value.userName);
    formData.append('email', this.form.value.email);
    formData.append('homePage', this.form.value.homePage ?? '');
    formData.append('text', text);
    formData.append('captchaId', this.captchaId);
    formData.append('captchaCode', this.form.value.captchaCode);
    formData.append('parentCommentId', this.parentCommentId?.toString() ?? '');

    const fileReadPromises = this.selectedFiles.map((file, index) => {
      return new Promise<void>((resolve) => {
        const reader = new FileReader();
        reader.onload = () => {
          const base64 = (reader.result as string).split(',')[1] || '';
          formData.append(`Files[${index}].FileName`, file.name);
          formData.append(`Files[${index}].ContentType`, file.type);
          formData.append(`Files[${index}].Base64Content`, base64);
          resolve();
        };
        reader.readAsDataURL(file);
      });
    });

    await Promise.all(fileReadPromises);

    this.http.post('/api/comments', formData).subscribe({
      next: () => {
        window.location.reload();
      },
      error: (error) => {
        this.errorMessage = 'Error submitting comment';
        this.submitErrorDetail = error?.error ?? '';
        this.isSubmitting = false;
        this.loadCaptcha();
        this.cdRef.detectChanges();
      }
    });
  }
}