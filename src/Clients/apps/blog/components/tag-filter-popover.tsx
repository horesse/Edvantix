"use client";

import { usePathname, useRouter, useSearchParams } from "next/navigation";

import { Tag, X } from "lucide-react";

import useGetTags from "@workspace/api-hooks/blog/useGetTags";
import { Button } from "@workspace/ui/components/button";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@workspace/ui/components/popover";
import { Skeleton } from "@workspace/ui/components/skeleton";

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

  const toggleTag = (tagId: string) => {
    const params = new URLSearchParams(searchParams.toString());
    if (activeTagId === tagId) {
      params.delete("tag");
    } else {
      params.set("tag", tagId);
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
  const activeTagName = tags?.find((t) => t.id === activeTagId)?.name;

  return (
    <Popover>
      <PopoverTrigger asChild>
        <Button
          variant={hasActiveTag ? "default" : "outline"}
          size="sm"
          className="shrink-0 gap-2 rounded-full"
        >
          <Tag className="h-4 w-4" />
          {hasActiveTag && activeTagName ? `#${activeTagName}` : "Tags"}
          {hasActiveTag && (
            <span className="bg-primary-foreground/25 ml-0.5 flex h-4 w-4 items-center justify-center rounded-full text-[10px] font-bold">
              1
            </span>
          )}
        </Button>
      </PopoverTrigger>

      <PopoverContent align="start" className="w-80 p-4">
        <div className="mb-3 flex items-center justify-between">
          <h4 className="text-sm font-semibold">Filter by Tag</h4>
          {hasActiveTag && (
            <Button
              variant="ghost"
              size="sm"
              className="text-muted-foreground hover:text-foreground h-6 gap-1 px-2 text-xs"
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
              const isActive = activeTagId === tag.id;
              return (
                <button
                  key={tag.id}
                  type="button"
                  onClick={() => toggleTag(tag.id)}
                  className={`inline-flex cursor-pointer items-center rounded-full px-3 py-1 text-xs font-medium transition-all duration-200 select-none ${
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
          <p className="text-muted-foreground py-4 text-center text-sm">
            No tags yet.
          </p>
        )}
      </PopoverContent>
    </Popover>
  );
}
