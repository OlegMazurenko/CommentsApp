export interface CommentListItem {
  id: number;
  userName: string;
  email: string;
  homePage?: string;
  text: string;
  createdAt: string;
  replyCount: number;
  files?: UploadedFileDto[];
}

export interface FileMeta {
  id: number;
  fileName: string;
  contentType: string;
}

export interface UploadedFileDto {
  id: number;
  fileName: string;
  contentType: string;
  base64Content: string;
}

export interface Reply {
  id: number;
  userName: string;
  email: string;
  homePage?: string;
  text: string;
  createdAt: string;
  repliesCount: number;
  files?: UploadedFileDto[];
}
