"use client";

import { use, useEffect, useState } from "react";

import Image from "next/image";
import Link from "next/link";
import { notFound } from "next/navigation";

import { ArrowLeft, Calendar, Clock, Link2, Lock, Twitter } from "lucide-react";
import ReactMarkdown from "react-markdown";
import remarkGfm from "remark-gfm";

import useGetPost from "@workspace/api-hooks/blog/useGetPost";
import { PostType } from "@workspace/types/blog";
import { Badge } from "@workspace/ui/components/badge";
import { Button } from "@workspace/ui/components/button";
import { Skeleton } from "@workspace/ui/components/skeleton";

import { PostLikeButton } from "@/components/post-like-button";
import { estimateReadTime, formatDate } from "@/lib/utils";

const TYPE_LABELS: Record<PostType, string> = {
  [PostType.News]: "News",
  [PostType.Changelog]: "Changelog",
};

// ── Heading utilities ──────────────────────────────────────────────────────────

type Heading = { level: 2 | 3; text: string; id: string };

function toHeadingId(text: string): string {
  return text
    .toLowerCase()
    .replace(/[^a-z0-9\s-]/g, "")
    .trim()
    .replace(/\s+/g, "-");
}

function childrenToText(node: React.ReactNode): string {
  if (typeof node === "string" || typeof node === "number") return String(node);
  if (Array.isArray(node)) return node.map(childrenToText).join("");
  if (node && typeof node === "object" && "props" in (node as object)) {
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    return childrenToText((node as any).props?.children as React.ReactNode);
  }
  return "";
}

function extractHeadings(content: string): Heading[] {
  const regex = /^(#{2,3})\s+(.+)$/gm;
  const results: Heading[] = [];
  let match: RegExpExecArray | null;
  while ((match = regex.exec(content)) !== null) {
    const levelStr = match[1];
    const textStr = match[2];
    if (!levelStr || !textStr) continue;
    const level = levelStr.length as 2 | 3;
    const text = textStr.trim();
    results.push({ level, text, id: toHeadingId(text) });
  }
  return results;
}

// Custom heading renderers that add anchor IDs for ToC linking
const markdownComponents = {
  h2: ({ children }: { children?: React.ReactNode }) => (
    <h2 id={toHeadingId(childrenToText(children))}>{children}</h2>
  ),
  h3: ({ children }: { children?: React.ReactNode }) => (
    <h3 id={toHeadingId(childrenToText(children))}>{children}</h3>
  ),
};

// ── Reading progress bar ───────────────────────────────────────────────────────

function ReadingProgressBar() {
  const [progress, setProgress] = useState(0);

  useEffect(() => {
    const update = () => {
      const el = document.documentElement;
      const docHeight = el.scrollHeight - el.clientHeight;
      setProgress(docHeight > 0 ? (window.scrollY / docHeight) * 100 : 0);
    };
    window.addEventListener("scroll", update, { passive: true });
    return () => window.removeEventListener("scroll", update);
  }, []);

  return (
    <div className="pointer-events-none fixed top-0 right-0 left-0 z-[60] h-0.5">
      <div
        className="from-primary to-chart-2 h-full bg-gradient-to-r"
        style={{ width: `${progress}%` }}
      />
    </div>
  );
}

// ── Table of Contents ──────────────────────────────────────────────────────────

function TableOfContents({ headings }: { headings: Heading[] }) {
  const [activeId, setActiveId] = useState("");

  useEffect(() => {
    if (!headings.length) return;

    const observer = new IntersectionObserver(
      (entries) => {
        for (const entry of entries) {
          if (entry.isIntersecting) {
            setActiveId(entry.target.id);
            break;
          }
        }
      },
      { rootMargin: "-10% 0% -80% 0%" },
    );

    headings.forEach(({ id }) => {
      const el = document.getElementById(id);
      if (el) observer.observe(el);
    });

    return () => observer.disconnect();
  }, [headings]);

  if (!headings.length) return null;

  return (
    <nav>
      <p className="text-muted-foreground mb-3 flex items-center gap-2 text-xs font-semibold tracking-wider uppercase">
        <span className="bg-primary inline-block h-3 w-0.5 rounded-full" />
        On this page
      </p>
      <ul className="space-y-0.5">
        {headings.map((h) => (
          <li key={h.id}>
            <a
              href={`#${h.id}`}
              className={`block border-l-2 py-1 text-sm transition-colors duration-150 ${
                h.level === 3 ? "pl-5" : "pl-3"
              } ${
                activeId === h.id
                  ? "border-primary text-primary font-medium"
                  : "text-muted-foreground hover:text-foreground border-transparent"
              }`}
              onClick={(e) => {
                e.preventDefault();
                document
                  .getElementById(h.id)
                  ?.scrollIntoView({ behavior: "smooth", block: "start" });
                setActiveId(h.id);
              }}
            >
              {h.text}
            </a>
          </li>
        ))}
      </ul>
    </nav>
  );
}

// ── Share buttons ──────────────────────────────────────────────────────────────

function ShareSection() {
  const [copied, setCopied] = useState(false);
  const [pageUrl, setPageUrl] = useState("");

  useEffect(() => {
    setPageUrl(window.location.href);
  }, []);

  const copyLink = async () => {
    try {
      await navigator.clipboard.writeText(window.location.href);
      setCopied(true);
      setTimeout(() => setCopied(false), 2000);
    } catch {
      // clipboard API unavailable in some contexts
    }
  };

  return (
    <div className="flex items-center gap-2">
      <span className="text-muted-foreground mr-1 text-xs font-medium tracking-wider uppercase">
        Share
      </span>
      <Button
        variant="outline"
        size="icon"
        className="h-8 w-8 rounded-full"
        onClick={copyLink}
        title="Copy link"
      >
        <Link2 className="h-3.5 w-3.5" />
      </Button>
      <a
        href={`https://twitter.com/intent/tweet?url=${encodeURIComponent(pageUrl)}`}
        target="_blank"
        rel="noopener noreferrer"
        className="border-input bg-background text-foreground hover:bg-accent hover:text-accent-foreground inline-flex h-8 w-8 items-center justify-center rounded-full border transition-colors"
        title="Share on X"
      >
        <Twitter className="h-3.5 w-3.5" />
      </a>
      {copied && (
        <span className="animate-in fade-in text-primary text-xs duration-200">
          Copied!
        </span>
      )}
    </div>
  );
}

// ── Loading skeleton ───────────────────────────────────────────────────────────

function PostSkeleton() {
  return (
    <div className="space-y-6">
      <Skeleton className="h-5 w-28" />
      <div className="border-border space-y-5 rounded-2xl border p-8">
        <div className="flex gap-2">
          <Skeleton className="h-6 w-20 rounded-full" />
        </div>
        <Skeleton className="h-11 w-3/4" />
        <Skeleton className="h-5 w-full" />
        <Skeleton className="h-5 w-2/3" />
        <div className="flex flex-wrap gap-3 pt-2">
          <Skeleton className="h-8 w-32 rounded-full" />
          <Skeleton className="h-8 w-36 rounded-full" />
          <Skeleton className="h-8 w-28 rounded-full" />
        </div>
      </div>
      <Skeleton className="h-72 w-full rounded-2xl" />
      <div className="space-y-3 pt-2">
        {Array.from({ length: 10 }).map((_, i) => (
          <Skeleton
            key={i}
            className="h-4 w-full"
            style={{ opacity: Math.max(0.2, 1 - i * 0.08) }}
          />
        ))}
      </div>
    </div>
  );
}

// ── Main post content ──────────────────────────────────────────────────────────

function PostContent({ slug }: { slug: string }) {
  const { data: post, isLoading, isError } = useGetPost(slug);

  if (isLoading) return <PostSkeleton />;
  if (isError || !post) notFound();

  const readTime = estimateReadTime(post.content);
  const headings = extractHeadings(post.content);

  return (
    <>
      <ReadingProgressBar />

      {/* Back link */}
      <Link
        href="/"
        className="text-muted-foreground hover:text-foreground group mb-8 inline-flex items-center gap-2 text-sm transition-colors"
      >
        <ArrowLeft className="h-4 w-4 transition-transform group-hover:-translate-x-1" />
        Back to blog
      </Link>

      {/* ── Post header card ── */}
      <header className="border-border from-primary/5 via-background to-chart-2/5 animate-in fade-in slide-in-from-bottom-4 relative mb-8 overflow-hidden rounded-2xl border bg-gradient-to-br p-7 duration-500 sm:p-10">
        {/* Decorative glow blobs */}
        <div className="bg-primary/10 pointer-events-none absolute top-0 right-0 h-72 w-72 rounded-full opacity-50 blur-3xl" />
        <div className="bg-chart-2/10 pointer-events-none absolute bottom-0 left-0 h-52 w-52 rounded-full opacity-40 blur-3xl" />

        <div className="relative">
          {/* Type badge + premium */}
          <div className="mb-4 flex items-center gap-2">
            <Badge
              variant="outline"
              className="text-primary border-primary/30 bg-primary/5 text-xs font-semibold tracking-wide uppercase"
            >
              {TYPE_LABELS[post.type]}
            </Badge>
            {post.isPremium && (
              <Badge
                variant="outline"
                className="border-amber-500/30 bg-amber-500/5 text-xs font-semibold text-amber-600 dark:text-amber-400"
              >
                <Lock className="mr-1 h-3 w-3" />
                Premium
              </Badge>
            )}
          </div>

          {/* Title */}
          <h1 className="text-foreground mb-4 text-3xl leading-tight font-bold tracking-tight sm:text-4xl lg:text-[2.75rem]">
            {post.title}
          </h1>

          {/* Summary */}
          {post.summary && (
            <p className="text-muted-foreground mb-6 max-w-2xl text-base leading-relaxed sm:text-lg">
              {post.summary}
            </p>
          )}

          {/* Meta chips */}
          <div className="flex flex-wrap items-center gap-2.5">
            {post.author && (
              <div className="bg-muted/50 flex items-center gap-2 rounded-full px-3 py-1.5">
                <div className="bg-primary/20 text-primary flex h-5 w-5 items-center justify-center rounded-full text-[10px] font-bold">
                  {post.author.fullName.charAt(0).toUpperCase()}
                </div>
                <span className="text-foreground text-sm font-medium">
                  {post.author.fullName}
                </span>
              </div>
            )}
            {post.publishedAt && (
              <div className="bg-muted/50 text-muted-foreground flex items-center gap-1.5 rounded-full px-3 py-1.5 text-sm">
                <Calendar className="h-3.5 w-3.5" />
                {formatDate(post.publishedAt)}
              </div>
            )}
            <div className="bg-muted/50 text-muted-foreground flex items-center gap-1.5 rounded-full px-3 py-1.5 text-sm">
              <Clock className="h-3.5 w-3.5" />
              {readTime} min read
            </div>
          </div>

          {/* Categories & Tags */}
          {(post.categories.length > 0 || post.tags.length > 0) && (
            <div className="border-border/60 mt-5 flex flex-wrap gap-2 border-t pt-5">
              {post.categories.map((cat) => (
                <Link key={cat.id} href={`/category/${cat.slug}`}>
                  <Badge
                    variant="secondary"
                    className="hover:bg-primary/10 hover:text-primary cursor-pointer transition-colors"
                  >
                    {cat.name}
                  </Badge>
                </Link>
              ))}
              {post.tags.map((tag) => (
                <Link key={tag.id} href={`/?tag=${tag.id}`}>
                  <Badge
                    variant="outline"
                    className="hover:border-primary/40 hover:bg-primary/5 hover:text-primary cursor-pointer transition-colors"
                  >
                    #{tag.name}
                  </Badge>
                </Link>
              ))}
            </div>
          )}
        </div>
      </header>

      {/* ── Cover image ── */}
      {post.coverImageUrl && (
        <div className="animate-in fade-in relative mb-10 aspect-[16/9] w-full overflow-hidden rounded-2xl shadow-lg delay-150 duration-700">
          <Image
            src={post.coverImageUrl}
            alt={post.title}
            fill
            className="object-cover"
            priority
            sizes="(max-width: 768px) 100vw, 900px"
          />
          <div className="absolute inset-0 bg-gradient-to-t from-black/10 to-transparent" />
        </div>
      )}

      {/* ── Content + sticky ToC ── */}
      <div className="animate-in fade-in delay-200 duration-700 lg:grid lg:grid-cols-[1fr_220px] lg:gap-12">
        {/* Prose content */}
        <div className="prose prose-neutral dark:prose-invert prose-headings:font-bold prose-headings:tracking-tight prose-h2:text-2xl prose-h3:text-xl prose-a:text-primary prose-a:no-underline hover:prose-a:underline prose-code:before:content-none prose-code:after:content-none prose-code:rounded prose-code:bg-muted prose-code:px-1.5 prose-code:py-0.5 prose-code:text-[0.85em] prose-code:text-primary prose-pre:border prose-pre:border-border prose-pre:bg-muted prose-img:rounded-xl prose-blockquote:border-l-primary prose-blockquote:text-muted-foreground max-w-none">
          <ReactMarkdown
            remarkPlugins={[remarkGfm]}
            components={markdownComponents}
          >
            {post.content}
          </ReactMarkdown>
        </div>

        {/* Sticky table of contents (desktop only) */}
        {headings.length > 0 && (
          <aside className="hidden lg:block">
            <div className="sticky top-24">
              <TableOfContents headings={headings} />
            </div>
          </aside>
        )}
      </div>

      {/* ── Post footer ── */}
      <div className="animate-in fade-in border-border mt-14 space-y-6 border-t pt-8 duration-500">
        <div className="flex flex-col items-center justify-between gap-4 sm:flex-row">
          <PostLikeButton
            postId={post.id}
            initialLikesCount={post.likesCount}
            initialLiked={post.isLikedByMe}
          />
          <ShareSection />
        </div>

        <div>
          <Link
            href="/"
            className="text-muted-foreground hover:text-foreground group inline-flex items-center gap-2 text-sm transition-colors"
          >
            <ArrowLeft className="h-4 w-4 transition-transform group-hover:-translate-x-1" />
            Back to blog
          </Link>
        </div>
      </div>
    </>
  );
}

// ── Page entry point ───────────────────────────────────────────────────────────

type PostPageParams = { params: Promise<{ slug: string }> };

export function PostDetailsSection({ params }: PostPageParams) {
  const { slug } = use(params);

  return (
    <div className="mx-auto max-w-5xl px-4 py-10 sm:px-6 lg:px-8">
      <PostContent slug={slug} />
    </div>
  );
}
