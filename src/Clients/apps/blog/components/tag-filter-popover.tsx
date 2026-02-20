"use client";

import { useSearchParams, useRouter, usePathname } from "next/navigation";

import { Tag, X } from "lucide-react";

import { Button } from "@workspace/ui/components/button";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@workspace/ui/components/popover";
import { Skeleton } from "@workspace/ui/components/skeleton";
import useGetTags from "@workspace/api-hooks/blog/useGetTags";

/**
 * A button that opens a popover to filter posts by tag.
 * Active tag is stored in the URL as `?tag=<id>`.
 */
export function TagFilterPopover() {
  const router = useRouter();
  const pathname = usePathname();
  const searchParams = useSearchParams();
  const activeTagId = searchParams.get("tag");
  const { data: tags, isLoading } = useGetTags();

  const toggleTag = (tagId: number) => {
    const params = new URLSearchParams(searchParams.toString());
    if (activeTagId === String(tagId)) {
      params.delete("tag");
    } else {
      params.set("tag", String(tagId));
    }
    // Preserve the path but update the query string
    router.push(`${pathname}?${params.toString()}`);
  };

  const clearTag = () => {
    const params = new URLSearchParams(searchParams.toString());
    params.delete("tag");
    router.push(`${pathname}?${params.toString()}`);
  };

  const hasActiveTag = Boolean(activeTagId);
  const activeTagName = tags?.find((t) => String(t.id) === activeTagId)?.name;

  return (
    <Popover>
      <PopoverTrigger asChild>
        <Button
          variant={hasActiveTag ? "default" : "outline"}
          size="sm"
          className="gap-2 rounded-full shrink-0"
        >
          <Tag className="h-4 w-4" />
          {hasActiveTag && activeTagName ? `#${activeTagName}` : "Tags"}
          {hasActiveTag && (
            <span className="ml-0.5 flex h-4 w-4 items-center justify-center rounded-full bg-primary-foreground/25 text-[10px] font-bold">
              1
            </span>
          )}
        </Button>
      </PopoverTrigger>

      <PopoverContent align="start" className="w-80 p-4">
        <div className="flex items-center justify-between mb-3">
          <h4 className="text-sm font-semibold">Filter by Tag</h4>
          {hasActiveTag && (
            <Button
              variant="ghost"
              size="sm"
              className="h-6 px-2 text-xs gap-1 text-muted-foreground hover:text-foreground"
              onClick={clearTag}
            >
              <X className="h-3 w-3" />
              Clear
            </Button>
          )}
        </div>

        {isLoading ? (
          <div className="flex flex-wrap gap-2">
            {Array.from({ length: 10 }).map((_, i) => (
              <Skeleton key={i} className="h-7 w-16 rounded-full" />
            ))}
          </div>
        ) : tags && tags.length > 0 ? (
          <div className="flex flex-wrap gap-2">
            {tags.map((tag) => {
              const isActive = activeTagId === String(tag.id);
              return (
                <button
                  key={tag.id}
                  type="button"
                  onClick={() => toggleTag(tag.id)}
                  className={`inline-flex items-center rounded-full px-3 py-1 text-xs font-medium transition-all duration-200 cursor-pointer select-none ${
                    isActive
                      ? "bg-primary text-primary-foreground shadow-sm"
                      : "bg-muted text-muted-foreground hover:bg-accent hover:text-foreground"
                  }`}
                >
                  #{tag.name}
                </button>
              );
            })}
          </div>
        ) : (
          <p className="text-sm text-muted-foreground text-center py-4">No tags yet.</p>
        )}
      </PopoverContent>
    </Popover>
  );
}
