import Image from "next/image";
import Link from "next/link";

import { ArrowRight, Calendar, Clock, Heart, Lock } from "lucide-react";

import type { PostSummaryModel } from "@workspace/types/blog";
import { PostType } from "@workspace/types/blog";
import { Badge } from "@workspace/ui/components/badge";

import { estimateReadTime, formatDate } from "@/lib/utils";

type PostCardProps = {
  post: PostSummaryModel;
  featured?: boolean;
};

const TYPE_LABELS: Record<PostType, string> = {
  [PostType.News]: "News",
  [PostType.Changelog]: "Changelog",
};

const TYPE_COLORS: Record<PostType, string> = {
  [PostType.News]: "bg-primary/10 text-primary border-primary/20",
  [PostType.Changelog]:
    "bg-green-500/10 text-green-600 border-green-500/20 dark:text-green-400",
};

export function PostCard({ post, featured = false }: PostCardProps) {
  if (featured) {
    return <FeaturedPostCard post={post} />;
  }

  return (
    <article className="group border-border bg-card hover:shadow-primary/5 hover:border-primary/30 relative flex h-full flex-col overflow-hidden rounded-2xl border transition-all duration-300 hover:-translate-y-1 hover:shadow-lg">
      {/* Gradient accent line that appears on hover */}
      <div className="from-primary/0 via-primary to-primary/0 absolute inset-x-0 top-0 h-0.5 bg-gradient-to-r opacity-0 transition-opacity duration-300 group-hover:opacity-100" />

      {post.coverImageUrl && (
        <Link href={`/${post.slug}`} className="block shrink-0 overflow-hidden">
          <div className="relative aspect-[16/9] overflow-hidden">
            <Image
              src={post.coverImageUrl}
              alt={post.title}
              fill
              className="object-cover transition-transform duration-500 group-hover:scale-105"
              sizes="(max-width: 640px) 100vw, (max-width: 1024px) 50vw, 33vw"
            />
            {/* Subtle overlay on hover */}
            <div className="absolute inset-0 bg-gradient-to-t from-black/20 to-transparent opacity-0 transition-opacity duration-300 group-hover:opacity-100" />
          </div>
        </Link>
      )}

      <div className="flex flex-1 flex-col p-5">
        {/* Type badge + premium */}
        <div className="mb-3 flex items-center gap-2">
          <span
            className={`inline-flex items-center rounded-full border px-2 py-0.5 text-xs font-medium ${TYPE_COLORS[post.type]}`}
          >
            {TYPE_LABELS[post.type]}
          </span>
          {post.isPremium && (
            <span className="inline-flex items-center gap-1 rounded-full border border-amber-500/20 bg-amber-500/10 px-2 py-0.5 text-xs font-medium text-amber-600 dark:text-amber-400">
              <Lock className="h-3 w-3" />
              Premium
            </span>
          )}
        </div>

        {/* Title */}
        <Link href={`/${post.slug}`}>
          <h2 className="text-card-foreground group-hover:text-primary mb-2 line-clamp-2 text-base leading-snug font-semibold transition-colors duration-200">
            {post.title}
          </h2>
        </Link>

        {/* Summary */}
        {post.summary && (
          <p className="text-muted-foreground mb-4 line-clamp-2 flex-1 text-sm">
            {post.summary}
          </p>
        )}

        {/* Category pills */}
        {post.categories.length > 0 && (
          <div className="mb-4 flex flex-wrap gap-1">
            {post.categories.slice(0, 3).map((cat) => (
              <Link key={cat.id} href={`/category/${cat.slug}`}>
                <Badge
                  variant="secondary"
                  className="hover:bg-accent cursor-pointer text-xs transition-colors"
                >
                  {cat.name}
                </Badge>
              </Link>
            ))}
          </div>
        )}

        {/* Meta row */}
        <div className="text-muted-foreground border-border mt-auto flex items-center gap-3 border-t pt-3 text-xs">
          {post.publishedAt && (
            <span className="flex items-center gap-1">
              <Calendar className="h-3 w-3" />
              {formatDate(post.publishedAt)}
            </span>
          )}
          <span className="flex items-center gap-1">
            <Clock className="h-3 w-3" />
            {estimateReadTime(post.summary ?? post.title)} min read
          </span>
          <span className="ml-auto flex items-center gap-1">
            <Heart className="h-3 w-3" />
            {post.likesCount}
          </span>
        </div>
      </div>
    </article>
  );
}

function FeaturedPostCard({ post }: { post: PostSummaryModel }) {
  return (
    <article className="group border-border bg-card hover:shadow-primary/10 hover:border-primary/30 relative overflow-hidden rounded-2xl border transition-all duration-300 hover:shadow-xl">
      {/* Animated gradient background */}
      <div className="from-primary/5 to-chart-2/5 pointer-events-none absolute inset-0 bg-gradient-to-br via-transparent opacity-0 transition-opacity duration-500 group-hover:opacity-100" />

      <div className="flex flex-col lg:flex-row">
        {/* Cover image */}
        {post.coverImageUrl && (
          <Link
            href={`/${post.slug}`}
            className="relative block aspect-[16/9] shrink-0 overflow-hidden lg:aspect-auto lg:w-1/2"
          >
            <Image
              src={post.coverImageUrl}
              alt={post.title}
              fill
              className="object-cover transition-transform duration-700 group-hover:scale-105"
              sizes="(max-width: 1024px) 100vw, 50vw"
              priority
            />
            <div className="to-card/20 absolute inset-0 bg-gradient-to-r from-transparent via-transparent" />
          </Link>
        )}

        {/* Content */}
        <div className="relative flex flex-col justify-center p-6 lg:w-1/2 lg:p-10">
          <div className="mb-4 flex items-center gap-2">
            <span
              className={`inline-flex items-center rounded-full border px-2.5 py-1 text-xs font-semibold ${TYPE_COLORS[post.type]}`}
            >
              {TYPE_LABELS[post.type]}
            </span>
            <span className="text-muted-foreground inline-flex items-center gap-1.5 text-xs font-medium tracking-wider uppercase">
              <span className="bg-primary inline-block h-1.5 w-1.5 animate-pulse rounded-full" />
              Featured
            </span>
          </div>

          <Link href={`/${post.slug}`}>
            <h1 className="text-card-foreground group-hover:text-primary mb-4 text-2xl leading-tight font-bold transition-colors duration-200 lg:text-3xl">
              {post.title}
            </h1>
          </Link>

          {post.summary && (
            <p className="text-muted-foreground mb-6 line-clamp-3 leading-relaxed">
              {post.summary}
            </p>
          )}

          {post.categories.length > 0 && (
            <div className="mb-6 flex flex-wrap gap-2">
              {post.categories.map((cat) => (
                <Link key={cat.id} href={`/category/${cat.slug}`}>
                  <Badge
                    variant="outline"
                    className="hover:bg-accent cursor-pointer transition-colors"
                  >
                    {cat.name}
                  </Badge>
                </Link>
              ))}
            </div>
          )}

          <div className="text-muted-foreground flex items-center gap-4 text-sm">
            {post.author && (
              <span className="text-foreground font-medium">
                {post.author.fullName}
              </span>
            )}
            {post.publishedAt && (
              <span className="flex items-center gap-1">
                <Calendar className="h-3.5 w-3.5" />
                {formatDate(post.publishedAt)}
              </span>
            )}
            <Link
              href={`/${post.slug}`}
              className="text-primary group/link ml-auto inline-flex items-center gap-1.5 font-medium transition-all duration-200 hover:gap-2.5"
            >
              Read more
              <ArrowRight className="h-4 w-4 transition-transform group-hover/link:translate-x-0.5" />
            </Link>
          </div>
        </div>
      </div>
    </article>
  );
}
