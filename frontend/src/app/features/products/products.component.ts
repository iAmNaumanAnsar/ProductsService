import { CommonModule } from '@angular/common';
import { Component, OnInit, signal } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { PRODUCT_COLORS, ProductColor } from '../../core/models/product-color';
import { Product } from '../../core/models/product';
import { ProductsService } from '../../core/services/products.service';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './products.component.html',
  styleUrl: './products.component.scss'
})
export class ProductsComponent implements OnInit {
  readonly colors = PRODUCT_COLORS;

  readonly products = signal<Product[]>([]);
  readonly loading = signal(false);
  readonly colorFilter = signal<ProductColor | ''>('');
  readonly formError = signal<string | null>(null);
  readonly submitting = signal(false);

  readonly form = this.fb.nonNullable.group({
    name: ['', Validators.required],
    sku: ['', [Validators.required, Validators.pattern(/^[A-Za-z0-9-]+$/)]],
    color: this.fb.nonNullable.control<ProductColor>('Black', Validators.required),
    price: [0, [Validators.required, Validators.min(0)]],
    stockQuantity: [0, [Validators.required, Validators.min(0)]]
  });

  constructor(
    private readonly fb: FormBuilder,
    private readonly productsService: ProductsService,
    private readonly authService: AuthService,
    private readonly router: Router
  ) {}

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.loading.set(true);
    this.productsService.getAll(this.colorFilter() || undefined).subscribe({
      next: (products) => {
        this.products.set(products);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  onColorFilterChange(color: string): void {
    this.colorFilter.set(color as ProductColor | '');
    this.loadProducts();
  }

  submit(): void {
    if (this.form.invalid || this.submitting()) {
      return;
    }

    this.submitting.set(true);
    this.formError.set(null);

    this.productsService.create(this.form.getRawValue()).subscribe({
      next: () => {
        this.submitting.set(false);
        this.form.reset({ name: '', sku: '', color: 'Black', price: 0, stockQuantity: 0 });
        this.loadProducts();
      },
      error: (err) => {
        this.submitting.set(false);
        this.formError.set(
          err.status === 409
            ? 'A product with that SKU already exists.'
            : 'Could not create the product. Check the form values.'
        );
      }
    });
  }

  logout(): void {
    this.authService.logout();
    this.router.navigateByUrl('/login');
  }
}
