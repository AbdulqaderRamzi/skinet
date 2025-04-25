import { inject, Injectable } from '@angular/core';
import {
  ConfirmationToken,
  loadStripe,
  Stripe,
  StripeAddressElement,
  StripeAddressElementOptions,
  StripeElements,
  StripePaymentElement,
} from '@stripe/stripe-js';
import { environment } from '../../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { CartService } from './cart.service';
import { Cart } from '../../shared/models/cart';
import { catchError, firstValueFrom, map } from 'rxjs';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root',
})
export class StripeService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  private accountService = inject(AccountService);
  private cartService = inject(CartService);
  private stripePromise: Promise<Stripe | null>;
  private elements?: StripeElements;
  private addressElement?: StripeAddressElement;
  private paymentElement?: StripePaymentElement;

  constructor() {
    this.stripePromise = loadStripe(environment.stripePublicKey);
  }

  getStripeInstance() {
    return this.stripePromise;
  }

  async createPaymentElement() {
    if (!this.paymentElement) {
      const element = await this.initializeElements();
      if (element) {
        this.paymentElement = element.create('payment');
      } else {
        throw new Error('Elements instance has not been loaded');
      }
    }
    return this.paymentElement;
  }

  async createAddressElement() {
    if (!this.addressElement) {
      const elements = await this.initializeElements();
      if (elements) {
        const user = this.accountService.currentUser();
        let defaultValues: StripeAddressElementOptions['defaultValues'] = {};
        if (user) {
          defaultValues.name = user.firstName + ' ' + user.lastName;
        }

        if (user?.address) {
          defaultValues.address = {
            line1: user.address.line1,
            line2: user.address.line2,
            city: user.address.city,
            state: user.address.city,
            postal_code: user.address.postalCode,
            country: user.address.country,
          };
        }

        const options: StripeAddressElementOptions = {
          mode: 'shipping',
          defaultValues,
        };
        this.addressElement = elements.create('address', options);
      } else {
        throw new Error('Elements instance has not been loaded');
      }
    }
    return this.addressElement;
  }

  async initializeElements() {
    if (!this.elements) {
      const stripe = await this.getStripeInstance();
      if (stripe) {
        const cart = await firstValueFrom(this.createOrUpdatePaymentIntent());
        this.elements = stripe.elements({
          clientSecret: cart.clientSecret,
          appearance: { labels: 'floating' },
        });
      } else {
        throw new Error('Stripe has not been loaded');
      }
    }
    return this.elements;
  }

  async createConfirmationToken() {
    const stripe = await this.getStripeInstance();
    const elements = await this.initializeElements();
    const result = await elements.submit();

    if (result.error) throw new Error(result.error.message);

    if (stripe) return await stripe?.createConfirmationToken({ elements });

    throw new Error('Stripe not available');
  }

  async confirmPayment(confirmationToken: ConfirmationToken) {
    const stripe = await this.getStripeInstance();
    const elements = await this.initializeElements();
    const result = await elements.submit();

    if (result.error) throw new Error(result.error.message);

    const clientSecret = this.cartService.cart()?.clientSecret;

    if (stripe && clientSecret) {
      return await stripe.confirmPayment({
        clientSecret: clientSecret,
        confirmParams: {
          confirmation_token: confirmationToken.id,
        },
        redirect: 'if_required',
      });
    }
    throw new Error('Unable to load strip');
  }

  createOrUpdatePaymentIntent() {
    const cart = this.cartService.cart();
    if (!cart) throw new Error('Problem with cart');
    return this.http.post<Cart>(this.baseUrl + `payments/${cart.id}`, {}).pipe(
      map(cart => {
        this.cartService.setCart(cart);
        return cart;
      })
    );
  }

  disposeElements() {
    this.elements = undefined;
    this.addressElement = undefined;
    this.paymentElement = undefined;
  }
}
