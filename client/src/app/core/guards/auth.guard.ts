import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AccountService } from '../services/account.service';
import { map, of } from 'rxjs';

export const authGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const router = inject(Router);

  // need to return observable to router beacause eventually it will subscribe to it

  // It’s possible for currentUser() to be falsy (like null or undefined) while auth.isAuthenticated is true,
  // especially in Angular applications with asynchronous authentication.
  // This usually happens due to timing issues or how the application manages local and server states.

  //* Synchronous Check
  if (accountService.currentUser()) return of(true);

  //* Asynchronous Check
  // It’s possible for currentUser() to be falsy (like null or undefined)
  //  while auth.isAuthenticated is true, especially
  // in Angular applications with asynchronous authentication. This usually happens due to timing issues
  // or how the application manages local and server states.
  return accountService.getAuthState().pipe(
    map(auth => {
      if (auth.isAuthenticated) {
        return true;
      }
      router.navigate(['/account/login'], { queryParams: { returnUrl: state.url } });
      return false;
    })
  );
};
