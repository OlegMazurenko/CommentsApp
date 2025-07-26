import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CommentApiService } from '../../services/comment-api';
import { CommentListItem } from '../../models/comment.model';
import { CommentReplies } from '../comment-replies/comment-replies';
import { CommentForm } from '../comment-form/comment-form';

@Component({
  selector: 'app-comment-list',
  standalone: true,
  templateUrl: './comment-list.html',
  styleUrl: './comment-list.css',
  imports: [
    CommonModule,
    CommentReplies,
    CommentForm
  ]
})
export class CommentList implements OnInit {
  comments: CommentListItem[] = [];
  currentPage = 1;
  totalPages = 1;
  sortField = 'created';
  sortDir: 'asc' | 'desc' = 'desc';

  selectedCommentId: number | null = null;

  constructor(private api: CommentApiService) {}

  ngOnInit(): void {
    this.loadComments();
  }

  loadComments(): void {
    const sortParam = `${this.sortField}_${this.sortDir}`;
    this.api.getComments(this.currentPage, sortParam).subscribe(data => {
      this.comments = data;
    });
  }

  changeSort(field: string): void {
    if (this.sortField === field) {
      this.sortDir = this.sortDir === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortField = field;
      this.sortDir = 'asc';
    }
    this.loadComments();
  }

  prevPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.loadComments();
    }
  }

  nextPage(): void {
    this.currentPage++;
    this.loadComments();
  }

  toggleReplies(commentId: number): void {
    this.selectedCommentId = this.selectedCommentId === commentId ? null : commentId;
  }
}
