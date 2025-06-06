import { nanoid } from 'nanoid';

export type CartType = {
  id: string;
  items: CartItem[];
  deliveryMethodId?: string;
  clientSecret?: string;
  paymentIntentId?: string;
};

export type CartItem = {
  productId: string;
  productName: string;
  price: number;
  quantity: number;
  pictureUrl: string;
  brand: string;
  type: string;
};

export class Cart implements CartType {
  id = nanoid();
  items: CartItem[] = [];
  deliveryMethodId?: string;
  clientSecret?: string;
  paymentIntentId?: string;
}
