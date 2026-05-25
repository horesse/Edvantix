/** Возвращает форму слова по числу.
 *  forms = [форма_1, форма_2_4, форма_5+] — русское склонение. */
export function declension(
  n: number,
  forms: readonly [string, string, string],
): string {
  const abs = Math.abs(n);
  const m10 = abs % 10;
  const m100 = abs % 100;

  if (m10 === 1 && m100 !== 11) return forms[0];
  if (m10 >= 2 && m10 <= 4 && (m100 < 10 || m100 >= 20)) return forms[1];

  return forms[2];
}

const RECORD_FORMS = ["запись", "записи", "записей"] as const;

/** Склонение слова «запись» по числу: 1 запись, 2 записи, 5 записей. */
export function declRecords(n: number): string {
  return declension(n, RECORD_FORMS);
}

/** Относительная дата из ISO-строки: «сегодня», «вчера», «N дней назад» и т.д. */
export function relativeDate(iso: string | null | undefined): string | null {
  if (!iso) return null;

  const date = new Date(iso);
  if (isNaN(date.getTime())) return null;

  const now = new Date();
  const diffMs = now.getTime() - date.getTime();
  const diffDays = Math.floor(diffMs / (1000 * 60 * 60 * 24));

  if (diffDays < 0) return null;
  if (diffDays === 0) return "сегодня";
  if (diffDays === 1) return "вчера";
  if (diffDays < 7)
    return `${diffDays} ${declension(diffDays, ["день", "дня", "дней"])} назад`;
  if (diffDays < 14) return "неделю назад";

  const diffWeeks = Math.floor(diffDays / 7);
  if (diffDays < 30) {
    return `${diffWeeks} ${declension(diffWeeks, ["неделю", "недели", "недель"])} назад`;
  }

  const diffMonths = Math.floor(diffDays / 30);
  if (diffDays < 365) {
    return `${diffMonths} ${declension(diffMonths, ["месяц", "месяца", "месяцев"])} назад`;
  }

  const diffYears = Math.floor(diffDays / 365);

  return `${diffYears} ${declension(diffYears, ["год", "года", "лет"])} назад`;
}
