import { TestBed } from '@angular/core/testing';
import { provideRouter, Router } from '@angular/router';
import { of } from 'rxjs';
import { vi } from 'vitest';

import { LoginComponent } from './login.component';
import { AuthService } from '../../../core/services/auth.service';
import { ToastService } from '../../../core/services/toast.service';

describe('LoginComponent', () => {
  let authService: {
    isAuthenticated: ReturnType<typeof vi.fn>;
    isAdmin: ReturnType<typeof vi.fn>;
    loginWithGoogle: ReturnType<typeof vi.fn>;
  };
  let toastService: {
    error: ReturnType<typeof vi.fn>;
    success: ReturnType<typeof vi.fn>;
  };
  let router: Router;

  beforeEach(async () => {
    authService = {
      isAuthenticated: vi.fn().mockReturnValue(false),
      isAdmin: vi.fn().mockReturnValue(false),
      loginWithGoogle: vi.fn().mockReturnValue(
        of({
          token: 'token',
          usuarioId: 1,
          nombre: 'Ada',
          email: 'ada@example.com',
          rol: 'cliente',
        }),
      ),
    };

    toastService = {
      error: vi.fn(),
      success: vi.fn(),
    };

    (globalThis as { google?: unknown }).google = {
      accounts: {
        id: {
          initialize: vi.fn(),
          renderButton: vi.fn(),
        },
      },
    };

    await TestBed.configureTestingModule({
      imports: [LoginComponent],
      providers: [
        provideRouter([]),
        { provide: AuthService, useValue: authService },
        { provide: ToastService, useValue: toastService },
      ],
    }).compileComponents();

    router = TestBed.inject(Router);
  });

  afterEach(() => {
    delete (globalThis as { google?: unknown }).google;
  });

  it('should create the component and initialize Google Sign-In', async () => {
    const fixture = TestBed.createComponent(LoginComponent);
    fixture.detectChanges();
    await fixture.whenStable();
    const googleApi = (globalThis as { google?: { accounts?: { id?: { initialize?: unknown } } } })
      .google;

    expect(fixture.componentInstance).toBeTruthy();
    expect(googleApi?.accounts?.id?.initialize).toHaveBeenCalled();
  });

  it('should redirect authenticated users according to their role', async () => {
    authService.isAuthenticated.mockReturnValue(true);
    authService.isAdmin.mockReturnValue(true);
    const navigateSpy = vi.spyOn(router, 'navigateByUrl').mockResolvedValue(true);
    const fixture = TestBed.createComponent(LoginComponent);

    await fixture.componentInstance.ngAfterViewInit();

    expect(navigateSpy).toHaveBeenCalledWith('/admin/eventos');
  });
});
