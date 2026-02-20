"use client";

import { Suspense } from "react";
import { useSearchParams } from "next/navigation";
import { useState, useEffect } from "react";

import { Search, Sparkles } from "lucide-react";

import { Input } from "@workspace/ui/components/input";
import { Skeleton } from "@workspace/ui/components/skeleton";
import useGetPosts from "@workspace/api-hooks/blog/useGetPosts";
import { PostType } from "@workspace/types/blog";
import type { GetPostsQuery } from "@workspace/types/blog";

import { PostCard } from "@/components/post-card";
import { CategoryTabs } from "@/components/category-tabs";
import { TagFilterPopover } from "@/components/tag-filter-popover";

const SEARCH_DEBOUNCE_MS = 400;

/** Inner component that reads URL search params and drives the post list. */
function PostList() {
  const searchParams = useSearchParams();
  const [search, setSearch] = useState("");
  const [debouncedSearch, setDebouncedSearch] = useState("");

  // Debounce the search input to avoid a request per keystroke
  useEffect(() => {
    const timer = setTimeout(() => setDebouncedSearch(search), SEARCH_DEBOUNCE_MS);
    return () => clearTimeout(timer);
  }, [search]);

  const query: GetPostsQuery = {
    categoryId: searchParams.get("category")
      ? Number(searchParams.get("category"))
      : undefined,
    tagId: searchParams.get("tag")
      ? Number(searchParams.get("tag"))
      : undefined,
    type: searchParams.get("type")
      ? (Number(searchParams.get("type")) as PostType)
      : undefined,
    search: debouncedSearch || undefined,
    pageSize: 12,
  };

  const { data, isLoading, isError } = useGetPosts(query);

  const isFiltered = Boolean(query.search || query.categoryId || query.tagId);

  if (isError) {
    return (
      <div className="text-center py-20">
        <p className="text-muted-foreground">Failed to load posts. Please try again.</p>
      </div>
    );
  }

  if (isLoading) {
    return (
      <div className="space-y-8">
        <Skeleton className="h-80 w-full rounded-2xl" />
        <div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-3 gap-6">
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
      <div className="flex gap-3 items-center">
        <div className="relative flex-1 max-w-sm">
          <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
          <Input
            placeholder="Search posts..."
            className="pl-9 rounded-full bg-card"
            value={search}
            onChange={(e) => setSearch(e.target.value)}
          />
        </div>
        <Suspense>
          <TagFilterPopover />
        </Suspense>
      </div>

      {posts.length === 0 ? (
        <div className="text-center py-24">
          <div className="text-5xl mb-4">🔍</div>
          <p className="text-muted-foreground text-lg font-medium">No posts found.</p>
          <p className="text-sm text-muted-foreground mt-1">
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
          <div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-3 gap-6">
            {(featuredPost && !isFiltered ? restPosts : posts).map((post, index) => (
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
            ))}
          </div>

          {data && data.totalPages > 1 && (
            <p className="text-center text-sm text-muted-foreground">
              Showing {posts.length} of {data.totalItems} posts
            </p>
          )}
        </>
      )}
    </div>
  );
}

export default function BlogHomePage() {
  return (
    <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8 py-12">
      {/* ── Hero ── */}
      <div className="mb-10 relative">
        {/* Decorative glow blobs */}
        <div className="pointer-events-none absolute top-0 left-0 w-80 h-80 rounded-full bg-primary/10 blur-3xl opacity-60" />
        <div className="pointer-events-none absolute top-4 right-0 w-64 h-64 rounded-full bg-chart-2/10 blur-3xl opacity-50" />

        <div className="relative">
          <div className="inline-flex items-center gap-2 rounded-full bg-primary/10 border border-primary/20 px-3 py-1 text-xs font-medium text-primary mb-5 animate-in fade-in duration-500">
            <Sparkles className="h-3 w-3" />
            Latest from our team
          </div>

          <h1 className="text-4xl sm:text-5xl lg:text-6xl font-bold tracking-tight text-foreground mb-3 animate-in fade-in slide-in-from-bottom-2 duration-500 delay-75">
            The{" "}
            <span className="text-primary">Edvantix</span>{" "}
            Blog
          </h1>
          <p className="text-lg text-muted-foreground max-w-xl animate-in fade-in slide-in-from-bottom-2 duration-500 delay-150">
            News, updates, and insights from our team.
          </p>
        </div>
      </div>

      {/* ── Category tabs ── */}
      <div className="mb-6 animate-in fade-in slide-in-from-bottom-2 duration-500 delay-200">
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
            <div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-3 gap-6">
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
