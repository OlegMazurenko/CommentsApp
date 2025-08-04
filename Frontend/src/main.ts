import 'zone.js';
import './styles.css';
//import './assets/lightbox2/custom-lightbox.min.css';

//import 'lightbox2/dist/css/lightbox.min.css';
import lightbox from 'lightbox2';

import * as bootstrap from 'bootstrap';

(globalThis as any).bootstrap = bootstrap;
(globalThis as any).lightbox = lightbox;

import { platformBrowser } from '@angular/platform-browser';
import { AppModule } from './app/app-module';

platformBrowser().bootstrapModule(AppModule, {
  ngZoneEventCoalescing: true,
})
  .catch(err => console.error(err));
