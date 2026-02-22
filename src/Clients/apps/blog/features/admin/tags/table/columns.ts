import type { TagModel } from "@workspace/types/blog";

export const TAG_COLUMNS: ReadonlyArray<{
  id: string;
  label: string;
  className?: string;
}> = [
  { id: "name", label: "Name", className: "font-semibold" },
  { id: "slug", label: "Slug", className: "font-semibold" },
  { id: "actions", label: "", className: "w-24" },
] as const;

export type TagRow = TagModel;
