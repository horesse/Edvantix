"use client";

import { use } from "react";
import Link from "next/link";

import { ArrowLeft } from "lucide-react";

import { Skeleton } from "@workspace/ui/components/skeleton";
import useGetPosts from "@workspace/api-hooks/blog/useGetPosts";
import useGetCategories from "@workspace/api-hooks/blog/useGetCategories";

import { PostCard } from "@/components/post-card";

type Props = { params: Promise<{ slug: string }> };

export default function CategoryPage({ params }: Props) {
  const { slug } = use(params);
  const { data: categories } = useGetCategories();
  const category = categories?.find((c) => c.slug === slug);

  const { data, isLoading } = useGetPosts(
    category ? { categoryId: category.id } : undefined,
    { enabled: Boolean(category) },
  );

  return (
    <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8 py-10">
      <Link
        href="/category"
        className="inline-flex items-center gap-2 text-sm text-muted-foreground hover:text-foreground transition-colors mb-8 group"
      >
        <ArrowLeft className="h-4 w-4 group-hover:-translate-x-1 transition-transform" />
        All categories
      </Link>

      <div className="mb-8">
        <h1 className="text-3xl font-bold tracking-tight">
          {category?.name ?? slug}
        </h1>
        {category?.description && (
          <p className="text-muted-foreground mt-2">{category.description}</p>
        )}
      </div>

      {isLoading ? (
        <div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-3 gap-6">
          {Array.from({ length: 6 }).map((_, i) => (
            <Skeleton key={i} className="h-72 rounded-xl" />
          ))}
        </div>
      ) : (
        <div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-3 gap-6">
          {data?.items.map((post) => (
            <PostCard key={post.id} post={post} />
          ))}
        </div>
      )}

      {data?.items.length === 0 && (
        <p className="text-center text-muted-foreground py-20">
          No posts in this category yet.
        </p>
      )}
    </div>
  );
}
