import Image from "next/image";
import Link from "next/link";

import { Calendar, Clock, Heart, Lock } from "lucide-react";

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
    <article className="group flex flex-col rounded-xl border border-border bg-card overflow-hidden hover:border-primary/30 transition-all duration-200 hover:shadow-md">
      {post.coverImageUrl && (
        <Link href={`/${post.slug}`} className="block overflow-hidden">
          <div className="relative aspect-[16/9] overflow-hidden">
            <Image
              src={post.coverImageUrl}
              alt={post.title}
              fill
              className="object-cover transition-transform duration-300 group-hover:scale-105"
              sizes="(max-width: 640px) 100vw, (max-width: 1024px) 50vw, 33vw"
            />
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
          <h2 className="text-base font-semibold leading-snug text-card-foreground group-hover:text-primary transition-colors line-clamp-2 mb-2">
            {post.title}
          </h2>
        </Link>

        {/* Summary */}
        {post.summary && (
          <p className="text-sm text-muted-foreground line-clamp-2 mb-4 flex-1">
            {post.summary}
          </p>
        )}

        {/* Categories */}
        {post.categories.length > 0 && (
          <div className="flex flex-wrap gap-1 mb-4">
            {post.categories.slice(0, 3).map((cat) => (
              <Link key={cat.id} href={`/category/${cat.slug}`}>
                <Badge variant="secondary" className="text-xs cursor-pointer hover:bg-accent">
                  {cat.name}
                </Badge>
              </Link>
            ))}
          </div>
        )}

        {/* Meta */}
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
    <article className="group relative overflow-hidden rounded-2xl border border-border bg-card">
      <div className="flex flex-col lg:flex-row">
        {/* Image */}
        {post.coverImageUrl && (
          <Link
            href={`/${post.slug}`}
            className="block relative lg:w-1/2 aspect-[16/9] lg:aspect-auto overflow-hidden"
          >
            <Image
              src={post.coverImageUrl}
              alt={post.title}
              fill
              className="object-cover transition-transform duration-300 group-hover:scale-105"
              sizes="(max-width: 1024px) 100vw, 50vw"
              priority
            />
          </Link>
        )}

        {/* Content */}
        <div className="flex flex-col justify-center p-6 lg:p-10 lg:w-1/2">
          <div className="flex items-center gap-2 mb-4">
            <span
              className={`inline-flex items-center rounded-full border px-2.5 py-1 text-xs font-semibold ${TYPE_COLORS[post.type]}`}
            >
              {TYPE_LABELS[post.type]}
            </span>
            <span className="text-xs text-muted-foreground font-medium uppercase tracking-wider">
              Featured
            </span>
          </div>

          <Link href={`/${post.slug}`}>
            <h1 className="text-2xl lg:text-3xl font-bold leading-tight text-card-foreground group-hover:text-primary transition-colors mb-4">
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
                  <Badge variant="outline" className="cursor-pointer hover:bg-accent">
                    {cat.name}
                  </Badge>
                </Link>
              ))}
            </div>
          )}

          <div className="flex items-center gap-4 text-sm text-muted-foreground">
            {post.author && (
              <span className="font-medium text-foreground">
                {post.author.fullName}
              </span>
            )}
            {post.publishedAt && (
              <span className="flex items-center gap-1">
                <Calendar className="h-3.5 w-3.5" />
                {formatDate(post.publishedAt)}
              </span>
            )}
            <span className="flex items-center gap-1 ml-auto">
              <Heart className="h-3.5 w-3.5" />
              {post.likesCount}
            </span>
          </div>
        </div>
      </div>
    </article>
  );
}
