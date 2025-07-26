import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';

import { App } from './app';
import { AppRoutingModule } from './app-routing-module';

import { CommentList } from './components/comment-list/comment-list';
import { CommentForm } from './components/comment-form/comment-form';
import { CommentReplies } from './components/comment-replies/comment-replies';

@NgModule({
  declarations: [
    App
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    CommentList,
    CommentForm,
    CommentReplies
  ],
  providers: [
    provideHttpClient(withInterceptorsFromDi())
  ],
  bootstrap: [App]
})
export class AppModule {}
