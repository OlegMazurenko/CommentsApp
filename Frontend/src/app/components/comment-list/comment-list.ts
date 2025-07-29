import { Component, EventEmitter, Output, signal } from '@angular/core';
import { CommonModule, DatePipe, NgIf, NgFor } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { CommentForm } from '../comment-form/comment-form';
import { CommentReplies } from '../comment-replies/comment-replies';
import { CommentListItem } from '../../models/comment.model';

@Component({
  selector: 'app-comment-list',
  standalone: true,
  imports: [CommonModule, NgIf, NgFor, DatePipe, CommentForm, CommentReplies],
  templateUrl: './comment-list.html'
})
export class CommentList {
  comments: CommentListItem[] = [];
  currentPage = 1;
  sortField = 'created';
  sortOrder: 'asc' | 'desc' = 'desc';
  selectedCommentId: number | null = null;
  selectedReplyFormId: number | null = null;
  replyFormsVisible: { [commentId: number]: boolean } = {};
  showMainForm = signal(false);
  isLastPage = false;

  @Output() textFileRequested = new EventEmitter<number>();

  constructor(private http: HttpClient) {
    this.loadComments();
  }

  loadComments() {
    const url = `https://localhost:5001/api/comments?page=${this.currentPage}&sortField=${this.sortField}&sortOrder=${this.sortOrder}`;
    this.http.get<{ comments: CommentListItem[]; isLastPage: boolean }>(url)
      .subscribe(result => {
        this.comments = result.comments;
        this.isLastPage = result.isLastPage;
      });
  }

  changeSort(field: string) {
    if (this.sortField === field) {
      this.sortOrder = this.sortOrder === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortField = field;
      this.sortOrder = 'asc';
    }
    this.loadComments();
  }

  toggleReplies(commentId: number) {
    this.selectedCommentId = this.selectedCommentId === commentId ? null : commentId;
  }

  toggleReplyForm(commentId: number) {
    this.replyFormsVisible[commentId] = !this.replyFormsVisible[commentId];
    this.selectedReplyFormId = this.replyFormsVisible[commentId] ? commentId : null;
  }

  toggleMainForm() {
    this.showMainForm.set(!this.showMainForm());
  }

  prevPage() {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.loadComments();
    }
  }

  nextPage() {
    if (!this.isLastPage) {
      this.currentPage++;
      this.loadComments();
    }
  }

  onTextFileClick(fileId: number) {
    this.textFileRequested.emit(fileId);
  }
}
