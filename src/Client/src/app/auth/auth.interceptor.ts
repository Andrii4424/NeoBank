import { catchError, debounceTime, switchMap } from 'rxjs/operators';
import { throwError } from 'rxjs';
import { AuthService } from '../data/services/auth/auth-service';
import { inject } from '@angular/core';
import { HttpHandlerFn, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { SKIP_LOGIN_REDIRECT } from './skip-login-redirect.token';

export const authTokenInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const isRefresh = req.url.toLowerCase().includes('/refresh');
  const skipLoginRedirect = req.context.get(SKIP_LOGIN_REDIRECT);

  const reqWithAuth = isRefresh ? req : setTokenHeaders(req, authService.getAccessToken(), authService);

  return next(reqWithAuth).pipe(
    catchError(err => {
      if (err.status === 401 && !isRefresh) {
        return authService.refresh(skipLoginRedirect).pipe(
          switchMap(token => {
            const retried = setTokenHeaders(req, token, authService);
            return next(retried);
          })
        );
      }
      return throwError(() => err);
    })
  );
};

const setTokenHeaders = (req: HttpRequest<any>, token: string | null, authService:AuthService) => {
  if (!token){
    return req;
  }
  return req.clone({
    withCredentials: true,
    setHeaders: { 
        Authorization: `Bearer ${token}`, 
    }
  });
};

const refreshAndProceed = (req: HttpRequest<any>, authService: AuthService, next: HttpHandlerFn, skipRedirect : boolean) => {
  return authService.refresh(skipRedirect).pipe(
    switchMap(() => {
      const retried = setTokenHeaders(req, authService.accessToken, authService);
      return next(retried);
    })
  );
};
