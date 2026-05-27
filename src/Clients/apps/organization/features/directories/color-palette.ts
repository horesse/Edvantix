/** Маппинг названия цвета → HEX для цветовых меток справочников. */
export const COLOR_DOTS: Record<string, string> = {
  sky: "#0ea5e9",
  teal: "#14b8a6",
  indigo: "#6366f1",
  violet: "#8b5cf6",
  amber: "#f59e0b",
  rose: "#f43f5e",
  pink: "#ec4899",
  slate: "#94a3b8",
  emerald: "#10b981",
  blue: "#3b82f6",
};

export const COLOR_DOT_NAMES = Object.keys(
  COLOR_DOTS,
) as (keyof typeof COLOR_DOTS)[];
