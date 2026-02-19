"use client";

import { use } from "react";
import Image from "next/image";
import Link from "next/link";
import { notFound } from "next/navigation";

import { ArrowLeft, Calendar, Clock, Lock, User } from "lucide-react";
import ReactMarkdown from "react-markdown";
import remarkGfm from "remark-gfm";

import { Badge } from "@workspace/ui/components/badge";
import { Separator } from "@workspace/ui/components/separator";
import { Skeleton } from "@workspace/ui/components/skeleton";
import useGetPost from "@workspace/api-hooks/blog/useGetPost";
import { PostType } from "@workspace/types/blog";

import { PostLikeButton } from "@/components/post-like-button";
import { formatDate, estimateReadTime } from "@/lib/utils";

const TYPE_LABELS: Record<PostType, string> = {
  [PostType.News]: "News",
  [PostType.Changelog]: "Changelog",
};

type PostPageParams = {
  params: Promise<{ slug: string }>;
};

function PostContent({ slug }: { slug: string }) {
  const { data: post, isLoading, isError } = useGetPost(slug);

  if (isLoading) {
    return (
      <div className="space-y-6 animate-pulse">
        <Skeleton className="h-8 w-24 rounded-full" />
        <Skeleton className="h-12 w-3/4" />
        <Skeleton className="h-5 w-full" />
        <Skeleton className="h-64 w-full rounded-xl" />
        <div className="space-y-3">
          {Array.from({ length: 8 }).map((_, i) => (
            <Skeleton key={i} className="h-4 w-full" />
          ))}
        </div>
      </div>
    );
  }

  if (isError || !post) {
    notFound();
  }

  const readTime = estimateReadTime(post.content);

  return (
    <article className="max-w-3xl mx-auto">
      {/* Back link */}
      <Link
        href="/"
        className="inline-flex items-center gap-2 text-sm text-muted-foreground hover:text-foreground transition-colors mb-8 group"
      >
        <ArrowLeft className="h-4 w-4 transition-transform group-hover:-translate-x-1" />
        Back to blog
      </Link>

      {/* Header */}
      <header className="mb-8">
        {/* Type + Premium badge */}
        <div className="flex items-center gap-2 mb-4">
          <Badge variant="outline" className="text-primary border-primary/30">
            {TYPE_LABELS[post.type]}
          </Badge>
          {post.isPremium && (
            <Badge
              variant="outline"
              className="text-amber-600 border-amber-500/30 dark:text-amber-400"
            >
              <Lock className="h-3 w-3 mr-1" />
              Premium
            </Badge>
          )}
        </div>

        {/* Title */}
        <h1 className="text-3xl sm:text-4xl lg:text-5xl font-bold leading-tight tracking-tight text-foreground mb-4">
          {post.title}
        </h1>

        {/* Summary */}
        {post.summary && (
          <p className="text-lg text-muted-foreground leading-relaxed mb-6">
            {post.summary}
          </p>
        )}

        {/* Meta */}
        <div className="flex flex-wrap items-center gap-4 text-sm text-muted-foreground mb-6">
          {post.author && (
            <span className="flex items-center gap-1.5 font-medium text-foreground">
              <User className="h-4 w-4" />
              {post.author.fullName}
            </span>
          )}
          {post.publishedAt && (
            <span className="flex items-center gap-1.5">
              <Calendar className="h-4 w-4" />
              {formatDate(post.publishedAt)}
            </span>
          )}
          <span className="flex items-center gap-1.5">
            <Clock className="h-4 w-4" />
            {readTime} min read
          </span>
        </div>

        {/* Categories & Tags */}
        {(post.categories.length > 0 || post.tags.length > 0) && (
          <div className="flex flex-wrap gap-2 mb-6">
            {post.categories.map((cat) => (
              <Link key={cat.id} href={`/category/${cat.slug}`}>
                <Badge className="cursor-pointer" variant="secondary">
                  {cat.name}
                </Badge>
              </Link>
            ))}
            {post.tags.map((tag) => (
              <Link key={tag.id} href={`/?tag=${tag.id}`}>
                <Badge className="cursor-pointer" variant="outline">
                  #{tag.name}
                </Badge>
              </Link>
            ))}
          </div>
        )}

        <Separator />
      </header>

      {/* Cover image */}
      {post.coverImageUrl && (
        <div className="relative aspect-[16/9] w-full overflow-hidden rounded-xl mb-10">
          <Image
            src={post.coverImageUrl}
            alt={post.title}
            fill
            className="object-cover"
            priority
            sizes="(max-width: 768px) 100vw, 768px"
          />
        </div>
      )}

      {/* Markdown content */}
      <div className="prose prose-neutral dark:prose-invert max-w-none prose-headings:font-bold prose-headings:tracking-tight prose-a:text-primary prose-a:no-underline hover:prose-a:underline prose-code:text-primary prose-pre:bg-muted prose-pre:border prose-pre:border-border">
        <ReactMarkdown remarkPlugins={[remarkGfm]}>
          {post.content}
        </ReactMarkdown>
      </div>

      {/* Footer */}
      <div className="mt-12 pt-8 border-t border-border flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4">
        <PostLikeButton
          postId={post.id}
          initialLikesCount={post.likesCount}
          initialLiked={post.isLikedByMe}
        />
        <Link
          href="/"
          className="inline-flex items-center gap-2 text-sm text-muted-foreground hover:text-foreground transition-colors group"
        >
          <ArrowLeft className="h-4 w-4 transition-transform group-hover:-translate-x-1" />
          Back to blog
        </Link>
      </div>
    </article>
  );
}

export default function PostPage({ params }: PostPageParams) {
  const { slug } = use(params);

  return (
    <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8 py-10">
      <PostContent slug={slug} />
    </div>
  );
}
