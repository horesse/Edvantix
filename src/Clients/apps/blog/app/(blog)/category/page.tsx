"use client";

import Link from "next/link";

import { ArrowRight, Hash, Sparkles } from "lucide-react";

import useGetCategories from "@workspace/api-hooks/blog/useGetCategories";
import { Skeleton } from "@workspace/ui/components/skeleton";

export default function CategoriesPage() {
  const { data: categories, isLoading } = useGetCategories();

  return (
    <div className="mx-auto max-w-7xl px-4 py-12 sm:px-6 lg:px-8">
      {/* ── Hero ── */}
      <div className="relative mb-10">
        <div className="bg-chart-2/10 pointer-events-none absolute top-0 left-0 h-64 w-64 rounded-full opacity-50 blur-3xl" />

        <div className="relative">
          <div className="bg-primary/10 border-primary/20 text-primary animate-in fade-in mb-4 inline-flex items-center gap-2 rounded-full border px-3 py-1 text-xs font-medium duration-500">
            <Sparkles className="h-3 w-3" />
            Browse by topic
          </div>
          <h1 className="text-foreground animate-in fade-in slide-in-from-bottom-2 mb-3 text-4xl font-bold tracking-tight delay-75 duration-500 sm:text-5xl">
            Categories
          </h1>
          <p className="text-muted-foreground animate-in fade-in slide-in-from-bottom-2 max-w-xl text-lg delay-150 duration-500">
            Explore posts organized by topic.
          </p>
        </div>
      </div>

      {isLoading ? (
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
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
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
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
                className="group border-border bg-card hover:border-primary/30 hover:shadow-primary/5 flex flex-col rounded-2xl border p-6 transition-all duration-200 hover:-translate-y-0.5 hover:shadow-md"
              >
                <div className="mb-3 flex items-start justify-between">
                  <div className="bg-primary/10 group-hover:bg-primary/15 flex h-10 w-10 items-center justify-center rounded-xl transition-colors">
                    <Hash className="text-primary h-5 w-5" />
                  </div>
                  <ArrowRight className="text-muted-foreground/40 group-hover:text-primary h-4 w-4 transition-all duration-200 group-hover:translate-x-0.5" />
                </div>
                <h2 className="text-card-foreground group-hover:text-primary mb-1 font-semibold transition-colors">
                  {cat.name}
                </h2>
                {cat.description && (
                  <p className="text-muted-foreground line-clamp-2 text-sm leading-relaxed">
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
