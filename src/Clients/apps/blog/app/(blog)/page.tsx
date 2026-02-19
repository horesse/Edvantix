"use client";

import { Suspense } from "react";
import { useSearchParams } from "next/navigation";

import { Search } from "lucide-react";

import { Input } from "@workspace/ui/components/input";
import { Skeleton } from "@workspace/ui/components/skeleton";
import useGetPosts from "@workspace/api-hooks/blog/useGetPosts";
import { PostType } from "@workspace/types/blog";
import type { GetPostsQuery } from "@workspace/types/blog";

import { PostCard } from "@/components/post-card";
import { SidebarFilters } from "@/components/sidebar-filters";
import { useState, useEffect } from "react";

const SEARCH_DEBOUNCE_MS = 400;

function PostList() {
  const searchParams = useSearchParams();
  const [search, setSearch] = useState("");
  const [debouncedSearch, setDebouncedSearch] = useState("");

  // Откладываем обновление запроса до паузы в наборе, чтобы не делать запрос на каждый символ
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

  if (isError) {
    return (
      <div className="text-center py-20">
        <p className="text-muted-foreground">
          Failed to load posts. Please try again.
        </p>
      </div>
    );
  }

  if (isLoading) {
    return (
      <div className="space-y-6">
        <Skeleton className="h-80 w-full rounded-2xl" />
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          {Array.from({ length: 4 }).map((_, i) => (
            <Skeleton key={i} className="h-72 w-full rounded-xl" />
          ))}
        </div>
      </div>
    );
  }

  const posts = data?.items ?? [];
  const [featuredPost, ...restPosts] = posts;

  return (
    <div className="space-y-8">
      {/* Search */}
      <div className="relative max-w-md">
        <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
        <Input
          placeholder="Search posts..."
          className="pl-9"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />
      </div>

      {posts.length === 0 ? (
        <div className="text-center py-20">
          <p className="text-muted-foreground text-lg">No posts found.</p>
          <p className="text-sm text-muted-foreground mt-1">
            Try adjusting your filters or search term.
          </p>
        </div>
      ) : (
        <>
          {/* Featured post */}
          {featuredPost && !query.search && !query.categoryId && !query.tagId && (
            <PostCard post={featuredPost} featured />
          )}

          {/* Post grid */}
          <div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-3 gap-6">
            {(featuredPost && !query.search && !query.categoryId && !query.tagId
              ? restPosts
              : posts
            ).map((post) => (
              <PostCard key={post.id} post={post} />
            ))}
          </div>

          {/* Pagination info */}
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
    <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8 py-10">
      <div className="flex flex-col lg:flex-row gap-12">
        {/* Main content */}
        <div className="flex-1 min-w-0">
          <div className="mb-8">
            <h1 className="text-3xl font-bold tracking-tight text-foreground">
              The Edvantix Blog
            </h1>
            <p className="text-muted-foreground mt-2">
              News, updates, and insights from our team.
            </p>
          </div>
          <Suspense
            fallback={
              <div className="space-y-6">
                <Skeleton className="h-80 w-full rounded-2xl" />
                <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                  {Array.from({ length: 4 }).map((_, i) => (
                    <Skeleton key={i} className="h-72 w-full rounded-xl" />
                  ))}
                </div>
              </div>
            }
          >
            <PostList />
          </Suspense>
        </div>

        {/* Sidebar */}
        <div className="w-full lg:w-64 xl:w-72 shrink-0">
          <Suspense
            fallback={
              <div className="space-y-4">
                {Array.from({ length: 6 }).map((_, i) => (
                  <Skeleton key={i} className="h-8 w-full rounded-md" />
                ))}
              </div>
            }
          >
            <SidebarFilters />
          </Suspense>
        </div>
      </div>
    </div>
  );
}
