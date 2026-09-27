import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { TestBed } from '@angular/core/testing';
import { environment } from '../../../environments/environment';
import { AuthService } from './auth.service';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    localStorage.clear();
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()]
    });
    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('starts unauthenticated when no token is stored', () => {
    expect(service.isAuthenticated()).toBe(false);
  });

  it('login stores the access token and flips isAuthenticated to true', () => {
    service.login('demo', 'Passw0rd!').subscribe();

    const req = httpMock.expectOne(`${environment.apiBaseUrl}/auth/token`);
    expect(req.request.method).toBe('POST');
    req.flush({ accessToken: 'test-token', expiresAtUtc: new Date().toISOString() });

    expect(service.isAuthenticated()).toBe(true);
    expect(service.token).toBe('test-token');
  });

  it('logout clears the token', () => {
    service.login('demo', 'Passw0rd!').subscribe();
    httpMock.expectOne(`${environment.apiBaseUrl}/auth/token`).flush({
      accessToken: 'test-token',
      expiresAtUtc: new Date().toISOString()
    });

    service.logout();

    expect(service.isAuthenticated()).toBe(false);
    expect(service.token).toBeNull();
  });
});
