"use client";

import Link from "next/link";
import { useSearchParams } from "next/navigation";

import { ScrollArea, ScrollBar } from "@workspace/ui/components/scroll-area";
import { Skeleton } from "@workspace/ui/components/skeleton";
import useGetCategories from "@workspace/api-hooks/blog/useGetCategories";

/** Horizontal scrollable category tab bar for filtering posts on the home page. */
export function CategoryTabs() {
  const searchParams = useSearchParams();
  const activeCategoryId = searchParams.get("category");
  const { data: categories, isLoading } = useGetCategories();

  if (isLoading) {
    return (
      <div className="flex gap-2">
        {Array.from({ length: 5 }).map((_, i) => (
          <Skeleton key={i} className="h-9 w-24 rounded-full" />
        ))}
      </div>
    );
  }

  return (
    <ScrollArea className="w-full">
      <div className="flex gap-2 pb-1">
        {/* "All" tab */}
        <Link
          href="/"
          className={`inline-flex items-center rounded-full px-4 py-2 text-sm font-medium whitespace-nowrap transition-all duration-200 ${
            !activeCategoryId
              ? "bg-primary text-primary-foreground shadow-sm"
              : "bg-card border border-border text-muted-foreground hover:text-foreground hover:border-primary/40 hover:bg-card"
          }`}
        >
          All Posts
        </Link>

        {categories?.map((cat) => {
          const isActive = activeCategoryId === String(cat.id);
          return (
            <Link
              key={cat.id}
              href={isActive ? "/" : `/?category=${cat.id}`}
              className={`inline-flex items-center rounded-full px-4 py-2 text-sm font-medium whitespace-nowrap transition-all duration-200 ${
                isActive
                  ? "bg-primary text-primary-foreground shadow-sm"
                  : "bg-card border border-border text-muted-foreground hover:text-foreground hover:border-primary/40"
              }`}
            >
              {cat.name}
            </Link>
          );
        })}
      </div>
      <ScrollBar orientation="horizontal" className="h-1.5" />
    </ScrollArea>
  );
}
