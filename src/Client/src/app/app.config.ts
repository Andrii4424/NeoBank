import { ApplicationConfig, importProvidersFrom, provideBrowserGlobalErrorListeners, provideZonelessChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { HttpClient, provideHttpClient, withInterceptors } from '@angular/common/http';
import { authTokenInterceptor } from './auth/auth.interceptor';
import { cookieInterceptor } from './auth/cookie.interceptor';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { provideTranslateHttpLoader, TranslateHttpLoader } from '@ngx-translate/http-loader';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZonelessChangeDetection(),
    provideRouter(routes),
    provideHttpClient(withInterceptors([cookieInterceptor, authTokenInterceptor])),
    
    importProvidersFrom(TranslateModule.forRoot({
          defaultLanguage: 'en'
        })),

    provideTranslateHttpLoader({
      prefix: './assets/i18n/',
          suffix: '.json'
    }),
  ]
};
