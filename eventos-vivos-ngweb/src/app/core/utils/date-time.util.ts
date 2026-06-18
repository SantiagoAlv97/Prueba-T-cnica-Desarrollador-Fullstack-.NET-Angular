function pad(value: number): string {
  return String(value).padStart(2, '0');
}

function formatLocalDateTime(date: Date, withSeconds: boolean): string {
  const year = date.getFullYear();
  const month = pad(date.getMonth() + 1);
  const day = pad(date.getDate());
  const hours = pad(date.getHours());
  const minutes = pad(date.getMinutes());
  const seconds = pad(date.getSeconds());

  return withSeconds
    ? `${year}-${month}-${day}T${hours}:${minutes}:${seconds}`
    : `${year}-${month}-${day}T${hours}:${minutes}`;
}

function normalizeIsoLikeString(value: string, withSeconds: boolean): string | null {
  const trimmed = value.trim();

  if (!trimmed) {
    return null;
  }

  const match = trimmed.match(
    /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2})(?::(\d{2}))?(?:\.\d+)?$/,
  );

  if (match) {
    const [, year, month, day, hours, minutes, seconds = '00'] = match;
    return withSeconds
      ? `${year}-${month}-${day}T${hours}:${minutes}:${seconds}`
      : `${year}-${month}-${day}T${hours}:${minutes}`;
  }

  return null;
}

export function toBackendLocalDateTime(value: string | Date | null | undefined): string | null {
  if (!value) {
    return null;
  }

  if (typeof value === 'string') {
    const normalized = normalizeIsoLikeString(value, true);

    if (normalized) {
      return normalized;
    }

    const date = new Date(value);
    return Number.isNaN(date.getTime()) ? null : formatLocalDateTime(date, true);
  }

  return Number.isNaN(value.getTime()) ? null : formatLocalDateTime(value, true);
}

export function toDateTimeLocalInputValue(value: string | Date | null | undefined): string {
  if (!value) {
    return '';
  }

  if (typeof value === 'string') {
    const normalized = normalizeIsoLikeString(value, false);

    if (normalized) {
      return normalized;
    }

    const date = new Date(value);
    return Number.isNaN(date.getTime()) ? '' : formatLocalDateTime(date, false);
  }

  return Number.isNaN(value.getTime()) ? '' : formatLocalDateTime(value, false);
}
