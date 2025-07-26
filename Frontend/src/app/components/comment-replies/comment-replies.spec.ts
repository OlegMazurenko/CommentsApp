import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CommentReplies } from './comment-replies';

describe('CommentReplies', () => {
  let component: CommentReplies;
  let fixture: ComponentFixture<CommentReplies>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CommentReplies]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CommentReplies);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
