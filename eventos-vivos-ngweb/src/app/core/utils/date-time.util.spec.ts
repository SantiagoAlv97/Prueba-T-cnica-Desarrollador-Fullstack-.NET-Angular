import { toBackendLocalDateTime, toDateTimeLocalInputValue } from './date-time.util';

describe('date-time.util', () => {
  it('should normalize datetime-local strings for backend without UTC suffix', () => {
    const normalized = toBackendLocalDateTime('2026-08-15T18:00');

    expect(normalized).toBe('2026-08-15T18:00:00');
    expect(toBackendLocalDateTime('2026-08-15T18:00:45')).toBe('2026-08-15T18:00:45');
    expect(normalized?.includes('Z')).toBe(false);
  });

  it('should format Date instances without UTC suffix', () => {
    const value = new Date(2026, 7, 15, 18, 0, 0);

    expect(toBackendLocalDateTime(value)).toBe('2026-08-15T18:00:00');
  });

  it('should normalize values for datetime-local inputs', () => {
    expect(toDateTimeLocalInputValue('2026-08-15T18:00:00')).toBe('2026-08-15T18:00');
  });

  it('should return null or empty string for invalid values', () => {
    expect(toBackendLocalDateTime('')).toBeNull();
    expect(toBackendLocalDateTime('not-a-date')).toBeNull();
    expect(toDateTimeLocalInputValue('not-a-date')).toBe('');
  });
});
