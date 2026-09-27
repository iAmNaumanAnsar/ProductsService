import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  readonly form = this.fb.nonNullable.group({
    username: ['demo', Validators.required],
    password: ['', Validators.required]
  });

  readonly submitting = signal(false);
  readonly errorMessage = signal<string | null>(null);

  constructor(
    private readonly fb: FormBuilder,
    private readonly authService: AuthService,
    private readonly router: Router
  ) {}

  submit(): void {
    if (this.form.invalid || this.submitting()) {
      return;
    }

    this.submitting.set(true);
    this.errorMessage.set(null);

    const { username, password } = this.form.getRawValue();

    this.authService.login(username, password).subscribe({
      next: () => {
        this.submitting.set(false);
        this.router.navigateByUrl('/products');
      },
      error: () => {
        this.submitting.set(false);
        this.errorMessage.set('Invalid username or password.');
      }
    });
  }
}
