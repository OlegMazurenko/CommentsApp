import { Component, EventEmitter, Output, signal } from '@angular/core';
import { CommonModule, DatePipe, NgIf, NgFor } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import * as signalR from '@microsoft/signalr';

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

  sortField: 'user' | 'email' | 'date' = 'date';
  sortDirection: 'asc' | 'desc' = 'desc';

  selectedCommentId: number | null = null;
  selectedReplyFormId: number | null = null;
  replyFormsVisible: { [commentId: number]: boolean } = {};
  showMainForm = signal(false);
  isLastPage = false;

  @Output() textFileRequested = new EventEmitter<number>();

  newCommentReceived = false;

  constructor(private http: HttpClient) {
    this.loadComments();
    this.setupSignalR();
  }

  loadComments() {
    const sort = `${this.sortField}_${this.sortDirection}`;
    const url = `https://localhost:5001/api/comments?page=${this.currentPage}&sort=${sort}`;
    this.http.get<{ comments: CommentListItem[]; isLastPage: boolean }>(url)
      .subscribe(result => {
        this.comments = result.comments;
        this.isLastPage = result.isLastPage;
      });
  }

  setupSignalR(): void {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:5001/hubs/comments')
      .withAutomaticReconnect()
      .build();

    connection.start()
      .then(() => console.log('SignalR connected'))
      .catch(err => console.error('SignalR connection error:', err));

    connection.on('NewComment', (commentId: number) => {
      console.log('New comment received via SignalR:', commentId);
      this.newCommentReceived = true;
    });
  }

  reloadNow(): void {
    this.newCommentReceived = false;
    this.loadComments();
  }

  changeSort(field: 'user' | 'email' | 'date') {
    if (this.sortField === field) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortField = field;
      this.sortDirection = 'asc';
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