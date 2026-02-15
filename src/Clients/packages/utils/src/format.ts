/**
 * Extracts up to two initials from a full name string.
 *
 * @example
 * ```ts
 * getInitials("Иванов Иван Иванович"); // "ИИ"
 * ```
 */
export function getInitials(name: string): string {
  return name
    .split(" ")
    .map((part) => part[0])
    .filter(Boolean)
    .slice(0, 2)
    .join("")
    .toUpperCase();
}

/**
 * Formats a date range using Russian locale short month/year format.
 * If `end` is falsy, displays "настоящее время" (present time).
 *
 * @example
 * ```ts
 * formatDateRange("2020-09-01", "2024-06-15"); // "сент. 2020 — июн. 2024"
 * formatDateRange("2020-09-01");               // "сент. 2020 — настоящее время"
 * ```
 */
export function formatDateRange(start: string, end?: string | null): string {
  const options: Intl.DateTimeFormatOptions = {
    month: "short",
    year: "numeric",
  };
  const startStr = new Date(start).toLocaleDateString("ru-RU", options);
  const endStr = end
    ? new Date(end).toLocaleDateString("ru-RU", options)
    : "настоящее время";
  return `${startStr} — ${endStr}`;
}
