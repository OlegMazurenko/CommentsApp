export interface CommentListItem {
  id: number;
  text: string;
  createdAt: string;
  userName: string;
  email: string;
  homePage?: string;
  replyCount: number;
}

export interface FileMeta {
  id: number;
  fileName: string;
  contentType: string;
}

export interface CommentTree {
  id: number;
  text: string;
  createdAt: string;
  userName: string;
  email: string;
  homePage?: string;
  files: FileMeta[];
  replies: CommentTree[];
}

export interface UploadedFileDto {
  fileName: string;
  contentType: string;
  base64Content: string;
}

export interface CommentDto {
  userName: string;
  email: string;
  homePage?: string;
  text: string;
  parentCommentId?: number;
  captchaId: string;
  captchaCode: string;
  files: UploadedFileDto[];
}

export interface CommentTree {
  id: number;
  userName: string;
  email: string;
  text: string;
  createdAt: string;
  children: CommentTree[];
}