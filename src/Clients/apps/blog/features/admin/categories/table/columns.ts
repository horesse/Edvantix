import type { CategoryModel } from "@workspace/types/blog";

export const CATEGORY_COLUMNS: ReadonlyArray<{
  id: string;
  label: string;
  className?: string;
}> = [
  { id: "name", label: "Name", className: "font-semibold" },
  { id: "slug", label: "Slug", className: "hidden sm:table-cell font-semibold" },
  { id: "description", label: "Description", className: "hidden md:table-cell font-semibold" },
  { id: "actions", label: "", className: "w-24" },
] as const;

export type CategoryRow = CategoryModel;
