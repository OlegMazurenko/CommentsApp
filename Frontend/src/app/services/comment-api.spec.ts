import { TestBed } from '@angular/core/testing';

import { CommentApiService } from './comment-api';

describe('CommentApi', () => {
  let service: CommentApiService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CommentApiService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
