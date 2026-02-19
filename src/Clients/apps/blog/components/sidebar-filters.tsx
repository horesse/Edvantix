"use client";

import Link from "next/link";
import { useSearchParams } from "next/navigation";

import { Hash, Tag } from "lucide-react";

import { Badge } from "@workspace/ui/components/badge";
import { Separator } from "@workspace/ui/components/separator";
import { Skeleton } from "@workspace/ui/components/skeleton";
import useGetCategories from "@workspace/api-hooks/blog/useGetCategories";
import useGetTags from "@workspace/api-hooks/blog/useGetTags";

export function SidebarFilters() {
  const searchParams = useSearchParams();
  const activeCategoryId = searchParams.get("category");
  const activeTagId = searchParams.get("tag");

  const { data: categories, isLoading: categoriesLoading } = useGetCategories();
  const { data: tags, isLoading: tagsLoading } = useGetTags();

  return (
    <aside className="space-y-6">
      {/* Categories */}
      <div>
        <h3 className="flex items-center gap-2 text-sm font-semibold text-foreground mb-3">
          <Hash className="h-4 w-4 text-primary" />
          Categories
        </h3>
        {categoriesLoading ? (
          <div className="space-y-2">
            {Array.from({ length: 5 }).map((_, i) => (
              <Skeleton key={i} className="h-7 w-full rounded-md" />
            ))}
          </div>
        ) : (
          <div className="space-y-1">
            <Link
              href="/"
              className={`flex items-center justify-between w-full rounded-md px-3 py-1.5 text-sm transition-colors ${
                !activeCategoryId
                  ? "bg-primary/10 text-primary font-medium"
                  : "text-muted-foreground hover:bg-accent hover:text-foreground"
              }`}
            >
              All posts
            </Link>
            {categories?.map((cat) => {
              const isActive = activeCategoryId === String(cat.id);
              return (
                <Link
                  key={cat.id}
                  href={isActive ? "/" : `/?category=${cat.id}`}
                  className={`flex items-center justify-between w-full rounded-md px-3 py-1.5 text-sm transition-colors ${
                    isActive
                      ? "bg-primary/10 text-primary font-medium"
                      : "text-muted-foreground hover:bg-accent hover:text-foreground"
                  }`}
                >
                  <span>{cat.name}</span>
                </Link>
              );
            })}
          </div>
        )}
      </div>

      <Separator />

      {/* Tags */}
      <div>
        <h3 className="flex items-center gap-2 text-sm font-semibold text-foreground mb-3">
          <Tag className="h-4 w-4 text-primary" />
          Tags
        </h3>
        {tagsLoading ? (
          <div className="flex flex-wrap gap-2">
            {Array.from({ length: 8 }).map((_, i) => (
              <Skeleton key={i} className="h-6 w-16 rounded-full" />
            ))}
          </div>
        ) : (
          <div className="flex flex-wrap gap-2">
            {tags?.map((tag) => {
              const isActive = activeTagId === String(tag.id);
              return (
                <Link key={tag.id} href={isActive ? "/" : `/?tag=${tag.id}`}>
                  <Badge
                    variant={isActive ? "default" : "outline"}
                    className="cursor-pointer hover:bg-primary/10 transition-colors"
                  >
                    {tag.name}
                  </Badge>
                </Link>
              );
            })}
          </div>
        )}
      </div>
    </aside>
  );
}
