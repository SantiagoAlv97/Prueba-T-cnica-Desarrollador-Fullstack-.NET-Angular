import { TestBed } from '@angular/core/testing';
import { provideRouter, Router, UrlTree } from '@angular/router';

import { adminGuard } from './admin.guard';
import { AuthService } from '../services/auth.service';

describe('adminGuard', () => {
  let router: Router;
  let authService: {
    isAuthenticated: ReturnType<typeof vi.fn>;
    isAdmin: ReturnType<typeof vi.fn>;
  };

  beforeEach(() => {
    authService = {
      isAuthenticated: vi.fn(),
      isAdmin: vi.fn(),
    };

    TestBed.configureTestingModule({
      providers: [provideRouter([]), { provide: AuthService, useValue: authService }],
    });

    router = TestBed.inject(Router);
  });

  it('should redirect unauthenticated users to /login', () => {
    authService.isAuthenticated.mockReturnValue(false);

    const result = TestBed.runInInjectionContext(() => adminGuard({} as never, {} as never));

    expect(router.serializeUrl(result as UrlTree)).toBe('/login');
  });

  it('should redirect authenticated non-admin users to /eventos', () => {
    authService.isAuthenticated.mockReturnValue(true);
    authService.isAdmin.mockReturnValue(false);

    const result = TestBed.runInInjectionContext(() => adminGuard({} as never, {} as never));

    expect(router.serializeUrl(result as UrlTree)).toBe('/eventos');
  });

  it('should allow access for admin users', () => {
    authService.isAuthenticated.mockReturnValue(true);
    authService.isAdmin.mockReturnValue(true);

    const result = TestBed.runInInjectionContext(() => adminGuard({} as never, {} as never));

    expect(result).toBe(true);
  });
});
