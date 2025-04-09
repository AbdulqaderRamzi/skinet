import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { NavigationExtras, Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { SnackbarService } from '../services/snackbar.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const snackbar = inject(SnackbarService);

  return next(req).pipe(
    catchError((response: HttpErrorResponse) => {
      if (response.status === 400) {
        if (response.error.errors) {
          const modelStateErrors = [];
          for (const key in response.error.errors) {
            if (response.error.errors[key]) {
              modelStateErrors.push(response.error.errors[key]);
            }
          }
          throw modelStateErrors.flat();
        } else {
          snackbar.error(response.error.title || response.error);
        }
      }
      if (response.status === 401) {
        snackbar.error(response.error.title || response.error);
      }
      if (response.status === 404) {
        router.navigateByUrl('/not-found');
      }
      if (response.status === 500) {
        const navigationExtras: NavigationExtras = { state: { error: response.error } };
        router.navigateByUrl('/server-error', navigationExtras);
      }

      return throwError(() => response);
    })
  );
};
