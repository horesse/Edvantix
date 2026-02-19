import Image from "next/image";
import Link from "next/link";

import { ArrowRight, Calendar, Clock, Heart, Lock } from "lucide-react";

import { Badge } from "@workspace/ui/components/badge";
import type { PostSummaryModel } from "@workspace/types/blog";
import { PostType } from "@workspace/types/blog";

import { formatDate, estimateReadTime } from "@/lib/utils";

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
    <article className="group relative flex flex-col h-full rounded-2xl border border-border bg-card overflow-hidden transition-all duration-300 hover:-translate-y-1 hover:shadow-lg hover:shadow-primary/5 hover:border-primary/30">
      {/* Gradient accent line that appears on hover */}
      <div className="absolute inset-x-0 top-0 h-0.5 bg-gradient-to-r from-primary/0 via-primary to-primary/0 opacity-0 group-hover:opacity-100 transition-opacity duration-300" />

      {post.coverImageUrl && (
        <Link href={`/${post.slug}`} className="block overflow-hidden shrink-0">
          <div className="relative aspect-[16/9] overflow-hidden">
            <Image
              src={post.coverImageUrl}
              alt={post.title}
              fill
              className="object-cover transition-transform duration-500 group-hover:scale-105"
              sizes="(max-width: 640px) 100vw, (max-width: 1024px) 50vw, 33vw"
            />
            {/* Subtle overlay on hover */}
            <div className="absolute inset-0 bg-gradient-to-t from-black/20 to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-300" />
          </div>
        </Link>
      )}

      <div className="flex flex-col flex-1 p-5">
        {/* Type badge + premium */}
        <div className="flex items-center gap-2 mb-3">
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
          <h2 className="text-base font-semibold leading-snug text-card-foreground group-hover:text-primary transition-colors duration-200 line-clamp-2 mb-2">
            {post.title}
          </h2>
        </Link>

        {/* Summary */}
        {post.summary && (
          <p className="text-sm text-muted-foreground line-clamp-2 mb-4 flex-1">
            {post.summary}
          </p>
        )}

        {/* Category pills */}
        {post.categories.length > 0 && (
          <div className="flex flex-wrap gap-1 mb-4">
            {post.categories.slice(0, 3).map((cat) => (
              <Link key={cat.id} href={`/category/${cat.slug}`}>
                <Badge
                  variant="secondary"
                  className="text-xs cursor-pointer hover:bg-accent transition-colors"
                >
                  {cat.name}
                </Badge>
              </Link>
            ))}
          </div>
        )}

        {/* Meta row */}
        <div className="flex items-center gap-3 text-xs text-muted-foreground mt-auto pt-3 border-t border-border">
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
          <span className="flex items-center gap-1 ml-auto">
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
    <article className="group relative overflow-hidden rounded-2xl border border-border bg-card transition-all duration-300 hover:shadow-xl hover:shadow-primary/10 hover:border-primary/30">
      {/* Animated gradient background */}
      <div className="pointer-events-none absolute inset-0 bg-gradient-to-br from-primary/5 via-transparent to-chart-2/5 opacity-0 group-hover:opacity-100 transition-opacity duration-500" />

      <div className="flex flex-col lg:flex-row">
        {/* Cover image */}
        {post.coverImageUrl && (
          <Link
            href={`/${post.slug}`}
            className="block relative lg:w-1/2 aspect-[16/9] lg:aspect-auto overflow-hidden shrink-0"
          >
            <Image
              src={post.coverImageUrl}
              alt={post.title}
              fill
              className="object-cover transition-transform duration-700 group-hover:scale-105"
              sizes="(max-width: 1024px) 100vw, 50vw"
              priority
            />
            <div className="absolute inset-0 bg-gradient-to-r from-transparent via-transparent to-card/20" />
          </Link>
        )}

        {/* Content */}
        <div className="relative flex flex-col justify-center p-6 lg:p-10 lg:w-1/2">
          <div className="flex items-center gap-2 mb-4">
            <span
              className={`inline-flex items-center rounded-full border px-2.5 py-1 text-xs font-semibold ${TYPE_COLORS[post.type]}`}
            >
              {TYPE_LABELS[post.type]}
            </span>
            <span className="inline-flex items-center gap-1.5 text-xs text-muted-foreground font-medium uppercase tracking-wider">
              <span className="h-1.5 w-1.5 rounded-full bg-primary inline-block animate-pulse" />
              Featured
            </span>
          </div>

          <Link href={`/${post.slug}`}>
            <h1 className="text-2xl lg:text-3xl font-bold leading-tight text-card-foreground group-hover:text-primary transition-colors duration-200 mb-4">
              {post.title}
            </h1>
          </Link>

          {post.summary && (
            <p className="text-muted-foreground leading-relaxed mb-6 line-clamp-3">
              {post.summary}
            </p>
          )}

          {post.categories.length > 0 && (
            <div className="flex flex-wrap gap-2 mb-6">
              {post.categories.map((cat) => (
                <Link key={cat.id} href={`/category/${cat.slug}`}>
                  <Badge
                    variant="outline"
                    className="cursor-pointer hover:bg-accent transition-colors"
                  >
                    {cat.name}
                  </Badge>
                </Link>
              ))}
            </div>
          )}

          <div className="flex items-center gap-4 text-sm text-muted-foreground">
            {post.author && (
              <span className="font-medium text-foreground">{post.author.fullName}</span>
            )}
            {post.publishedAt && (
              <span className="flex items-center gap-1">
                <Calendar className="h-3.5 w-3.5" />
                {formatDate(post.publishedAt)}
              </span>
            )}
            <Link
              href={`/${post.slug}`}
              className="ml-auto inline-flex items-center gap-1.5 text-primary font-medium hover:gap-2.5 transition-all duration-200 group/link"
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
