import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CommentApiService } from '../../services/comment-api';
import { CommentTree } from '../../models/comment.model';
import { CommentForm } from '../comment-form/comment-form';

@Component({
  selector: 'app-comment-replies',
  standalone: true,
  templateUrl: './comment-replies.html',
  styleUrls: ['./comment-replies.css'],
  imports: [
    CommonModule,
    CommentForm]
})
export class CommentReplies implements OnInit {
  @Input() commentId!: number;

  replies: CommentTree[] = [];
  isLoading = true;

  constructor(private api: CommentApiService) {}

  ngOnInit(): void {
    this.api.getReplies(this.commentId).subscribe(data => {
      this.replies = data;
      this.isLoading = false;
    });
  }
}
