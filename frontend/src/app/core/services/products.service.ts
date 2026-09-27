import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CreateProductRequest, Product } from '../models/product';
import { ProductColor } from '../models/product-color';

@Injectable({ providedIn: 'root' })
export class ProductsService {
  private readonly baseUrl = `${environment.apiBaseUrl}/products`;

  constructor(private readonly http: HttpClient) {}

  getAll(color?: ProductColor | ''): Observable<Product[]> {
    let params = new HttpParams();
    if (color) {
      params = params.set('color', color);
    }

    return this.http.get<Product[]>(this.baseUrl, { params });
  }

  create(request: CreateProductRequest): Observable<Product> {
    return this.http.post<Product>(this.baseUrl, request);
  }
}
