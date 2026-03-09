"use client";

import { use } from "react";

import Link from "next/link";

import { ArrowLeft, Hash } from "lucide-react";

import useGetCategories from "@workspace/api-hooks/blog/useGetCategories";
import useGetPosts from "@workspace/api-hooks/blog/useGetPosts";
import { Skeleton } from "@workspace/ui/components/skeleton";

import { PostCard } from "@/components/post-card";

type Props = { params: Promise<{ slug: string }> };

export function CategoryPostsSection({ params }: Props) {
  const { slug } = use(params);
  const { data: categories } = useGetCategories();
  const category = categories?.find((c) => c.slug === slug);

  const { data, isLoading } = useGetPosts(
    category ? { categoryId: category.id } : undefined,
    { enabled: Boolean(category) },
  );

  return (
    <div className="mx-auto max-w-7xl px-4 py-10 sm:px-6 lg:px-8">
      {/* Back link */}
      <Link
        href="/category"
        className="text-muted-foreground hover:text-foreground group mb-8 inline-flex items-center gap-2 text-sm transition-colors"
      >
        <ArrowLeft className="h-4 w-4 transition-transform group-hover:-translate-x-1" />
        All categories
      </Link>

      {/* ── Category header card ── */}
      <div className="border-border from-chart-2/5 via-background to-primary/5 animate-in fade-in slide-in-from-bottom-4 relative mb-10 overflow-hidden rounded-2xl border bg-gradient-to-br px-7 py-8 duration-500 sm:px-10">
        <div className="bg-chart-2/10 pointer-events-none absolute top-0 right-0 h-56 w-56 rounded-full opacity-50 blur-3xl" />
        <div className="bg-primary/10 pointer-events-none absolute bottom-0 left-0 h-40 w-40 rounded-full opacity-40 blur-3xl" />

        <div className="relative flex items-start gap-4">
          <div className="bg-chart-2/15 border-chart-2/20 flex h-12 w-12 shrink-0 items-center justify-center rounded-2xl border">
            <Hash className="text-chart-2 h-6 w-6" />
          </div>
          <div>
            <h1 className="text-foreground text-3xl font-bold tracking-tight">
              {category?.name ?? slug}
            </h1>
            {category?.description && (
              <p className="text-muted-foreground mt-1.5 max-w-xl text-base leading-relaxed">
                {category.description}
              </p>
            )}
            {data && (
              <p className="text-muted-foreground mt-3 text-sm">
                {data.totalCount} post{data.totalCount !== 1 ? "s" : ""} in this
                category
              </p>
            )}
          </div>
        </div>
      </div>

      {isLoading ? (
        <div className="grid grid-cols-1 gap-6 sm:grid-cols-2 xl:grid-cols-3">
          {Array.from({ length: 6 }).map((_, i) => (
            <Skeleton key={i} className="h-72 rounded-2xl" />
          ))}
        </div>
      ) : data?.items.length === 0 ? (
        <div className="py-24 text-center">
          <div className="mb-3 text-5xl">📭</div>
          <p className="text-muted-foreground text-lg font-medium">
            No posts in this category yet.
          </p>
          <Link
            href="/"
            className="text-primary mt-3 inline-flex items-center gap-2 text-sm hover:underline"
          >
            <ArrowLeft className="h-3.5 w-3.5" />
            Back to all posts
          </Link>
        </div>
      ) : (
        <div className="grid grid-cols-1 gap-6 sm:grid-cols-2 xl:grid-cols-3">
          {data?.items.map((post, index) => (
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
      )}
    </div>
  );
}
