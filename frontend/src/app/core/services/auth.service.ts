import { HttpClient } from '@angular/common/http';
import { Injectable, computed, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';

interface LoginRequest {
  username: string;
  password: string;
}

interface LoginResponse {
  accessToken: string;
  expiresAtUtc: string;
}

const TOKEN_STORAGE_KEY = 'products-service.access-token';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly tokenSignal = signal<string | null>(localStorage.getItem(TOKEN_STORAGE_KEY));

  readonly isAuthenticated = computed(() => !!this.tokenSignal());

  constructor(private readonly http: HttpClient) {}

  get token(): string | null {
    return this.tokenSignal();
  }

  login(username: string, password: string): Observable<LoginResponse> {
    const request: LoginRequest = { username, password };

    return this.http.post<LoginResponse>(`${environment.apiBaseUrl}/auth/token`, request).pipe(
      tap((response) => {
        localStorage.setItem(TOKEN_STORAGE_KEY, response.accessToken);
        this.tokenSignal.set(response.accessToken);
      })
    );
  }

  logout(): void {
    localStorage.removeItem(TOKEN_STORAGE_KEY);
    this.tokenSignal.set(null);
  }
}
