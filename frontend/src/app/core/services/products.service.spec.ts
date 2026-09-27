import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { environment } from '../../../environments/environment';
import { ProductsService } from './products.service';

describe('ProductsService', () => {
  let service: ProductsService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()]
    });
    service = TestBed.inject(ProductsService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('getAll without a colour omits the query parameter', () => {
    service.getAll().subscribe();

    const req = httpMock.expectOne(`${environment.apiBaseUrl}/products`);
    expect(req.request.params.has('color')).toBe(false);
    req.flush([]);
  });

  it('getAll with a colour appends it as a query parameter', () => {
    service.getAll('Red').subscribe();

    const req = httpMock.expectOne((r) => r.url === `${environment.apiBaseUrl}/products` && r.params.get('color') === 'Red');
    expect(req.request.params.get('color')).toBe('Red');
    req.flush([]);
  });

  it('create posts the request body to /products', () => {
    const request = { name: 'Mouse', sku: 'SKU-1', color: 'Black' as const, price: 9.99, stockQuantity: 5 };

    service.create(request).subscribe();

    const req = httpMock.expectOne(`${environment.apiBaseUrl}/products`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(request);
    req.flush({ id: '1', ...request, createdAtUtc: new Date().toISOString() });
  });
});
