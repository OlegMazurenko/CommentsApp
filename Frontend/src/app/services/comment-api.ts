import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  CommentDto,
  CommentListItem,
  CommentTree
} from '../models/comment.model';

@Injectable({
  providedIn: 'root'
})
export class CommentApiService {
  private readonly apiUrl = 'https://localhost:5001/api';

  constructor(private http: HttpClient) {}

  /** Get main comments (table) */
  getComments(page: number, sort: string): Observable<CommentListItem[]> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('sort', sort);

    return this.http.get<CommentListItem[]>(`${this.apiUrl}/comments`, { params });
  }

  /** Get a tree of replies for one comment */
  getReplies(commentId: number): Observable<CommentTree[]> {
    return this.http.get<CommentTree[]>(`${this.apiUrl}/comments/${commentId}/replies`);
  }

  /** Send a new comment */
  postComment(formData: FormData): Observable<number> {
  return this.http.post<number>(`${this.apiUrl}/comments`, formData);
  }

  /** Get CAPTCHA as Blob (for image) */
  getCaptchaBlob(): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/captcha`, { responseType: 'blob' });
  }

  /** Get a link to download file by ID */
  getFileUrl(fileId: number): string {
    return `${this.apiUrl}/files/${fileId}`;
  }
}
