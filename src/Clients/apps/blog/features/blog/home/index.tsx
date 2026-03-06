"use client";

import { Suspense } from "react";
import { useEffect, useState } from "react";

import { useSearchParams } from "next/navigation";

import { Search, Sparkles } from "lucide-react";

import useGetPosts from "@workspace/api-hooks/blog/useGetPosts";
import { PostType } from "@workspace/types/blog";
import type { GetPostsQuery } from "@workspace/types/blog";
import { Input } from "@workspace/ui/components/input";
import { Skeleton } from "@workspace/ui/components/skeleton";

import { CategoryTabs } from "@/components/category-tabs";
import { PostCard } from "@/components/post-card";
import { TagFilterPopover } from "@/components/tag-filter-popover";

const SEARCH_DEBOUNCE_MS = 400;

/** Inner component that reads URL search params and drives the post list. */
function PostList() {
  const searchParams = useSearchParams();
  const [search, setSearch] = useState("");
  const [debouncedSearch, setDebouncedSearch] = useState("");

  useEffect(() => {
    const timer = setTimeout(
      () => setDebouncedSearch(search),
      SEARCH_DEBOUNCE_MS,
    );
    return () => clearTimeout(timer);
  }, [search]);

  const query: GetPostsQuery = {
    categoryId: searchParams.get("category") ?? undefined,
    tagId: searchParams.get("tag") ?? undefined,
    type: searchParams.get("type")
      ? (Number(searchParams.get("type")) as PostType)
      : undefined,
    search: debouncedSearch || undefined,
    pageSize: 3,
  };

  const { data, isLoading, isError } = useGetPosts(query);

  const isFiltered = Boolean(query.search || query.categoryId || query.tagId);

  if (isError) {
    return (
      <div className="py-20 text-center">
        <p className="text-muted-foreground">
          Failed to load posts. Please try again.
        </p>
      </div>
    );
  }

  if (isLoading) {
    return (
      <div className="space-y-8">
        <Skeleton className="h-80 w-full rounded-2xl" />
        <div className="grid grid-cols-1 gap-6 sm:grid-cols-2 xl:grid-cols-3">
          {Array.from({ length: 6 }).map((_, i) => (
            <Skeleton key={i} className="h-72 w-full rounded-2xl" />
          ))}
        </div>
      </div>
    );
  }

  const posts = data?.items ?? [];
  const [featuredPost, ...restPosts] = posts;

  return (
    <div className="space-y-8">
      {/* Search + Tags filter row */}
      <div className="flex items-center gap-3">
        <div className="relative max-w-sm flex-1">
          <Search className="text-muted-foreground absolute top-1/2 left-3 h-4 w-4 -translate-y-1/2" />
          <Input
            placeholder="Search posts..."
            className="bg-card rounded-full pl-9"
            value={search}
            onChange={(e) => setSearch(e.target.value)}
          />
        </div>
        <Suspense>
          <TagFilterPopover />
        </Suspense>
      </div>

      {posts.length === 0 ? (
        <div className="py-24 text-center">
          <div className="mb-4 text-5xl">🔍</div>
          <p className="text-muted-foreground text-lg font-medium">
            No posts found.
          </p>
          <p className="text-muted-foreground mt-1 text-sm">
            Try adjusting your filters or search term.
          </p>
        </div>
      ) : (
        <>
          {/* Featured post — only shown when no active filters */}
          {featuredPost && !isFiltered && (
            <div className="animate-in fade-in slide-in-from-bottom-4 duration-500">
              <PostCard post={featuredPost} featured />
            </div>
          )}

          {/* Post grid with staggered entrance animation */}
          <div className="grid grid-cols-1 gap-6 sm:grid-cols-2 xl:grid-cols-3">
            {(featuredPost && !isFiltered ? restPosts : posts).map(
              (post, index) => (
                <div
                  key={post.id}
                  className="animate-in fade-in slide-in-from-bottom-4"
                  style={{
                    animationDelay: `${index * 60}ms`,
                    animationFillMode: "both",
                    animationDuration: "400ms",
                  }}
                >
                  <PostCard post={post} />
                </div>
              ),
            )}
          </div>

          {data && data.totalCount > posts.length && (
            <p className="text-muted-foreground text-center text-sm">
              Showing {posts.length} of {data.totalCount} posts
            </p>
          )}
        </>
      )}
    </div>
  );
}

export function BlogHomeSection() {
  return (
    <div className="mx-auto max-w-7xl px-4 py-12 sm:px-6 lg:px-8">
      {/* ── Hero ── */}
      <div className="relative mb-10">
        {/* Decorative glow blobs */}
        <div className="bg-primary/10 pointer-events-none absolute top-0 left-0 h-80 w-80 rounded-full opacity-60 blur-3xl" />
        <div className="bg-chart-2/10 pointer-events-none absolute top-4 right-0 h-64 w-64 rounded-full opacity-50 blur-3xl" />

        <div className="relative">
          <div className="bg-primary/10 border-primary/20 text-primary animate-in fade-in mb-5 inline-flex items-center gap-2 rounded-full border px-3 py-1 text-xs font-medium duration-500">
            <Sparkles className="h-3 w-3" />
            Latest from our team
          </div>

          <h1 className="text-foreground animate-in fade-in slide-in-from-bottom-2 mb-3 text-4xl font-bold tracking-tight delay-75 duration-500 sm:text-5xl lg:text-6xl">
            The <span className="text-primary">Edvantix</span> Blog
          </h1>
          <p className="text-muted-foreground animate-in fade-in slide-in-from-bottom-2 max-w-xl text-lg delay-150 duration-500">
            News, updates, and insights from our team.
          </p>
        </div>
      </div>

      {/* ── Category tabs ── */}
      <div className="animate-in fade-in slide-in-from-bottom-2 mb-6 delay-200 duration-500">
        <Suspense
          fallback={
            <div className="flex gap-2">
              {Array.from({ length: 5 }).map((_, i) => (
                <Skeleton key={i} className="h-9 w-24 rounded-full" />
              ))}
            </div>
          }
        >
          <CategoryTabs />
        </Suspense>
      </div>

      {/* ── Post list ── */}
      <Suspense
        fallback={
          <div className="space-y-8">
            <Skeleton className="h-80 w-full rounded-2xl" />
            <div className="grid grid-cols-1 gap-6 sm:grid-cols-2 xl:grid-cols-3">
              {Array.from({ length: 6 }).map((_, i) => (
                <Skeleton key={i} className="h-72 w-full rounded-2xl" />
              ))}
            </div>
          </div>
        }
      >
        <PostList />
      </Suspense>
    </div>
  );
}
