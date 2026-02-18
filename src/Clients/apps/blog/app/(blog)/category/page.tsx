"use client";

import Link from "next/link";

import { Hash } from "lucide-react";

import { Skeleton } from "@workspace/ui/components/skeleton";
import useGetCategories from "@workspace/api-hooks/blog/useGetCategories";

export default function CategoriesPage() {
  const { data: categories, isLoading } = useGetCategories();

  return (
    <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8 py-10">
      <div className="mb-8">
        <h1 className="text-3xl font-bold tracking-tight">Categories</h1>
        <p className="text-muted-foreground mt-2">
          Browse posts by category.
        </p>
      </div>

      {isLoading ? (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
          {Array.from({ length: 6 }).map((_, i) => (
            <Skeleton key={i} className="h-28 rounded-xl" />
          ))}
        </div>
      ) : (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
          {categories?.map((cat) => (
            <Link
              key={cat.id}
              href={`/?category=${cat.id}`}
              className="group flex flex-col rounded-xl border border-border bg-card p-6 hover:border-primary/40 hover:shadow-sm transition-all"
            >
              <div className="flex items-center gap-3 mb-3">
                <div className="flex h-9 w-9 items-center justify-center rounded-lg bg-primary/10">
                  <Hash className="h-4 w-4 text-primary" />
                </div>
                <h2 className="font-semibold text-card-foreground group-hover:text-primary transition-colors">
                  {cat.name}
                </h2>
              </div>
              {cat.description && (
                <p className="text-sm text-muted-foreground line-clamp-2">
                  {cat.description}
                </p>
              )}
            </Link>
          ))}
        </div>
      )}
    </div>
  );
}
