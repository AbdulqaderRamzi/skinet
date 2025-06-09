import { Routes } from '@angular/router';
import { HomeComponent } from './features/home/home.component';
import { ShopComponent } from './features/shop/shop.component';
import { ProductDetailsComponent } from './features/shop/product-details/product-details.component';
import { ServerErrorComponent } from './shared/components/server-error/server-error.component';
import { NotFoundComponent } from './shared/components/not-found/not-found.component';
import { CartComponent } from './features/cart/cart.component';
import { CheckoutComponent } from './features/checkout/checkout.component';
import { RegisterComponent } from './features/account/register/register.component';
import { LoginComponent } from './features/account/login/login.component';
import { authGuard } from './core/guards/auth.guard';
import { OrderComponent } from './features/orders/order.component';
import { OrderDetailedComponent } from './features/orders/order-detailed/order-detailed.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'shop', component: ShopComponent },
  { path: 'shop/:id', component: ProductDetailsComponent },
  { path: 'cart', component: CartComponent },
  {
    path: 'checkout',
    loadChildren: () => import('./features/checkout/routes').then(r => r.checkoutRoutes),
  },
  {
    path: 'orders',
    loadChildren: () => import('./features/orders/routes').then(r => r.orderRoutes),
  },
  {
    path: 'account',
    loadChildren: () => import('./features/account/routes').then(r => r.accountRoutes),
  },
  { path: 'server-error', component: ServerErrorComponent },
  { path: 'not-found', component: NotFoundComponent },
  { path: '**', redirectTo: '', pathMatch: 'full' },
];
