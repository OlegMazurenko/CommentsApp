import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { CommentForm } from '../comment-form/comment-form';

interface Reply {
  id: number;
  userName: string;
  email: string;
  homePage?: string;
  text: string;
  createdAt: string;
  repliesCount: number; // 👈 добавим это поле в API
}

@Component({
  selector: 'app-comment-replies',
  standalone: true,
  imports: [CommonModule, CommentForm],
  templateUrl: './comment-replies.html',
  styleUrl: './comment-replies.css'
})
export class CommentReplies implements OnInit {
  @Input() commentId!: number;

  replies: Reply[] = [];
  loadedReplies: { [id: number]: Reply[] } = {};
  expandedReplies: { [id: number]: boolean } = {};
  replyFormVisible: { [id: number]: boolean } = {};

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.loadReplies(this.commentId);
  }

  loadReplies(parentId: number): void {
    this.http.get<Reply[]>(`https://localhost:5001/api/comments/${parentId}/replies`)
      .subscribe({
        next: res => {
          if (parentId === this.commentId) {
            this.replies = res;
          } else {
            this.loadedReplies[parentId] = res;
          }
          this.expandedReplies[parentId] = true;
        },
        error: err => console.error(err)
      });
  }

  toggleReplies(id: number): void {
    if (this.expandedReplies[id]) {
      this.expandedReplies[id] = false;
    } else if (this.loadedReplies[id]) {
      this.expandedReplies[id] = true;
    } else {
      this.loadReplies(id);
    }
  }

  showReplies(id: number): boolean {
    return this.expandedReplies[id] ?? false;
  }

  toggleReplyForm(id: number): void {
    this.replyFormVisible[id] = !this.replyFormVisible[id];
  }

  showReplyForm(id: number): boolean {
    return this.replyFormVisible[id] ?? false;
  }
}
