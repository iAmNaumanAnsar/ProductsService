import { ProductColor } from './product-color';

export interface Product {
  id: string;
  name: string;
  sku: string;
  color: ProductColor;
  price: number;
  stockQuantity: number;
  createdAtUtc: string;
}

export interface CreateProductRequest {
  name: string;
  sku: string;
  color: ProductColor;
  price: number;
  stockQuantity: number;
}
