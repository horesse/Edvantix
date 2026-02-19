"use client";

import Link from "next/link";

import { ArrowRight, Hash, Sparkles } from "lucide-react";

import { Skeleton } from "@workspace/ui/components/skeleton";
import useGetCategories from "@workspace/api-hooks/blog/useGetCategories";

export default function CategoriesPage() {
  const { data: categories, isLoading } = useGetCategories();

  return (
    <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8 py-12">
      {/* ── Hero ── */}
      <div className="mb-10 relative">
        <div className="pointer-events-none absolute top-0 left-0 w-64 h-64 rounded-full bg-chart-2/10 blur-3xl opacity-50" />

        <div className="relative">
          <div className="inline-flex items-center gap-2 rounded-full bg-primary/10 border border-primary/20 px-3 py-1 text-xs font-medium text-primary mb-4 animate-in fade-in duration-500">
            <Sparkles className="h-3 w-3" />
            Browse by topic
          </div>
          <h1 className="text-4xl sm:text-5xl font-bold tracking-tight text-foreground mb-3 animate-in fade-in slide-in-from-bottom-2 duration-500 delay-75">
            Categories
          </h1>
          <p className="text-lg text-muted-foreground max-w-xl animate-in fade-in slide-in-from-bottom-2 duration-500 delay-150">
            Explore posts organized by topic.
          </p>
        </div>
      </div>

      {isLoading ? (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
          {Array.from({ length: 6 }).map((_, i) => (
            <Skeleton key={i} className="h-36 rounded-2xl" />
          ))}
        </div>
      ) : categories?.length === 0 ? (
        <div className="py-24 text-center">
          <div className="mb-3 text-5xl">📂</div>
          <p className="text-muted-foreground text-lg font-medium">
            No categories yet.
          </p>
        </div>
      ) : (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
          {categories?.map((cat, index) => (
            <div
              key={cat.id}
              className="animate-in fade-in slide-in-from-bottom-4"
              style={{
                animationDelay: `${index * 60}ms`,
                animationFillMode: "both",
                animationDuration: "400ms",
              }}
            >
              <Link
                href={`/?category=${cat.id}`}
                className="group flex flex-col rounded-2xl border border-border bg-card p-6 transition-all duration-200 hover:-translate-y-0.5 hover:border-primary/30 hover:shadow-md hover:shadow-primary/5"
              >
                <div className="flex items-start justify-between mb-3">
                  <div className="flex h-10 w-10 items-center justify-center rounded-xl bg-primary/10 transition-colors group-hover:bg-primary/15">
                    <Hash className="h-5 w-5 text-primary" />
                  </div>
                  <ArrowRight className="h-4 w-4 text-muted-foreground/40 transition-all duration-200 group-hover:text-primary group-hover:translate-x-0.5" />
                </div>
                <h2 className="font-semibold text-card-foreground transition-colors group-hover:text-primary mb-1">
                  {cat.name}
                </h2>
                {cat.description && (
                  <p className="text-sm text-muted-foreground line-clamp-2 leading-relaxed">
                    {cat.description}
                  </p>
                )}
              </Link>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
