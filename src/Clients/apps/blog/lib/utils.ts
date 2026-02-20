import { format, formatDistanceToNow } from "date-fns";

/** Estimates reading time in minutes based on word count */
export function estimateReadTime(content: string): number {
  const wordsPerMinute = 200;
  const words = content.trim().split(/\s+/).length;
  return Math.max(1, Math.ceil(words / wordsPerMinute));
}

/** Formats a date string to human-readable form */
export function formatDate(date: string | Date): string {
  return format(new Date(date), "MMMM d, yyyy");
}

/** Returns relative time like "2 days ago" */
export function formatRelativeDate(date: string | Date): string {
  return formatDistanceToNow(new Date(date), { addSuffix: true });
}

/** Converts a title to a URL-friendly slug */
export function slugify(text: string): string {
  return text
    .toLowerCase()
    .replace(/[^\w\s-]/g, "")
    .replace(/[\s_]+/g, "-")
    .replace(/^-+|-+$/g, "");
}
