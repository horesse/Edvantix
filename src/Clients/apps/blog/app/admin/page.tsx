"use client";

import Link from "next/link";

import {
  Archive,
  Edit3,
  FileText,
  Hash,
  Plus,
  Send,
  Tag,
  TrendingUp,
} from "lucide-react";

import { Button } from "@workspace/ui/components/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@workspace/ui/components/card";
import { Badge } from "@workspace/ui/components/badge";
import { Skeleton } from "@workspace/ui/components/skeleton";
import useGetAdminPosts from "@workspace/api-hooks/blog/useGetAdminPosts";
import useGetCategories from "@workspace/api-hooks/blog/useGetCategories";
import useGetTags from "@workspace/api-hooks/blog/useGetTags";
import { PostStatus } from "@workspace/types/blog";
import { formatDate } from "@/lib/utils";

/** Fetches a single page (size=1) of admin posts filtered by status to get totalItems efficiently. */
function useStatusCount(status: PostStatus) {
  const { data } = useGetAdminPosts({ status, pageSize: 1 });
  return data?.totalItems ?? null;
}

const STATUS_CONFIG = {
  [PostStatus.Draft]: {
    label: "Draft",
    color: "bg-muted-foreground",
    bg: "bg-muted-foreground/10",
    text: "text-muted-foreground",
  },
  [PostStatus.Scheduled]: {
    label: "Scheduled",
    color: "bg-amber-500",
    bg: "bg-amber-500/10",
    text: "text-amber-600 dark:text-amber-400",
  },
  [PostStatus.Published]: {
    label: "Published",
    color: "bg-green-500",
    bg: "bg-green-500/10",
    text: "text-green-600 dark:text-green-400",
  },
  [PostStatus.Archived]: {
    label: "Archived",
    color: "bg-destructive",
    bg: "bg-destructive/10",
    text: "text-destructive",
  },
} as const;

/** Horizontal bar chart entry showing a status distribution row. */
function StatusBar({
  status,
  count,
  total,
}: {
  status: PostStatus;
  count: number | null;
  total: number;
}) {
  const cfg = STATUS_CONFIG[status];
  const pct = total > 0 && count !== null ? Math.round((count / total) * 100) : 0;

  return (
    <div className="flex items-center gap-3">
      <span className={`text-xs font-medium w-20 shrink-0 ${cfg.text}`}>
        {cfg.label}
      </span>
      <div className="flex-1 h-2 rounded-full bg-muted overflow-hidden">
        <div
          className={`h-full rounded-full ${cfg.color} transition-all duration-700 ease-out`}
          style={{ width: `${pct}%` }}
        />
      </div>
      <span className="text-xs font-mono text-muted-foreground w-8 text-right">
        {count ?? "—"}
      </span>
    </div>
  );
}

export default function AdminDashboardPage() {
  const draftCount = useStatusCount(PostStatus.Draft);
  const publishedCount = useStatusCount(PostStatus.Published);
  const scheduledCount = useStatusCount(PostStatus.Scheduled);
  const archivedCount = useStatusCount(PostStatus.Archived);

  const { data: categories } = useGetCategories();
  const { data: tags } = useGetTags();

  // Fetch last 5 posts for recent activity
  const { data: recentPosts, isLoading: postsLoading } = useGetAdminPosts({
    pageSize: 5,
    pageIndex: 1,
  });

  const totalPosts =
    (draftCount ?? 0) +
    (publishedCount ?? 0) +
    (scheduledCount ?? 0) +
    (archivedCount ?? 0);

  const statCards = [
    {
      label: "Total Posts",
      value: totalPosts || "—",
      icon: FileText,
      href: "/admin/posts",
      gradient: "from-primary/20 to-primary/5",
      iconColor: "text-primary",
      iconBg: "bg-primary/10",
    },
    {
      label: "Published",
      value: publishedCount ?? "—",
      icon: Send,
      href: "/admin/posts",
      gradient: "from-green-500/20 to-green-500/5",
      iconColor: "text-green-600 dark:text-green-400",
      iconBg: "bg-green-500/10",
    },
    {
      label: "Categories",
      value: categories?.length ?? "—",
      icon: Hash,
      href: "/admin/categories",
      gradient: "from-chart-2/20 to-chart-2/5",
      iconColor: "text-chart-2",
      iconBg: "bg-chart-2/10",
    },
    {
      label: "Tags",
      value: tags?.length ?? "—",
      icon: Tag,
      href: "/admin/tags",
      gradient: "from-amber-500/20 to-amber-500/5",
      iconColor: "text-amber-600 dark:text-amber-400",
      iconBg: "bg-amber-500/10",
    },
  ];

  return (
    <div className="space-y-8">
      {/* ── Header ── */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold tracking-tight">Dashboard</h1>
          <p className="text-muted-foreground mt-1">
            Overview of your blog content.
          </p>
        </div>
        <Button asChild className="gap-2">
          <Link href="/admin/posts/new">
            <Plus className="h-4 w-4" />
            New post
          </Link>
        </Button>
      </div>

      {/* ── Stats Grid ── */}
      <div className="grid grid-cols-2 lg:grid-cols-4 gap-4">
        {statCards.map((card) => {
          const Icon = card.icon;
          return (
            <Link key={card.label} href={card.href}>
              <Card
                className={`relative overflow-hidden border-border transition-all duration-200 hover:-translate-y-0.5 hover:shadow-md bg-gradient-to-br ${card.gradient}`}
              >
                <CardContent className="pt-5 pb-5 px-5">
                  <div className="flex items-start justify-between mb-3">
                    <div className={`rounded-xl p-2.5 ${card.iconBg}`}>
                      <Icon className={`h-5 w-5 ${card.iconColor}`} />
                    </div>
                    <TrendingUp className="h-4 w-4 text-muted-foreground/40" />
                  </div>
                  <div className="text-3xl font-bold text-foreground mb-0.5">
                    {card.value}
                  </div>
                  <div className="text-sm text-muted-foreground">{card.label}</div>
                </CardContent>
              </Card>
            </Link>
          );
        })}
      </div>

      {/* ── Middle Row: Distribution + Recent Activity ── */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Post status distribution */}
        <Card>
          <CardHeader className="pb-3">
            <CardTitle className="text-base">Post Distribution</CardTitle>
            <CardDescription>Breakdown of posts by status</CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            {(
              [
                PostStatus.Published,
                PostStatus.Scheduled,
                PostStatus.Draft,
                PostStatus.Archived,
              ] as const
            ).map((status) => {
              const countMap: Record<PostStatus, number | null> = {
                [PostStatus.Draft]: draftCount,
                [PostStatus.Scheduled]: scheduledCount,
                [PostStatus.Published]: publishedCount,
                [PostStatus.Archived]: archivedCount,
              };
              return (
                <StatusBar
                  key={status}
                  status={status}
                  count={countMap[status]}
                  total={totalPosts}
                />
              );
            })}

            {/* Mini donut using conic-gradient */}
            {totalPosts > 0 && (
              <div className="flex items-center gap-4 mt-2 pt-4 border-t border-border">
                <div
                  className="w-14 h-14 rounded-full shrink-0"
                  style={{
                    background: `conic-gradient(
                      oklch(0.56 0.13 42.95) 0% ${Math.round(((publishedCount ?? 0) / totalPosts) * 100)}%,
                      oklch(0.88 0.08 91) ${Math.round(((publishedCount ?? 0) / totalPosts) * 100)}% ${Math.round((((publishedCount ?? 0) + (scheduledCount ?? 0)) / totalPosts) * 100)}%,
                      oklch(0.61 0.01 91.49) ${Math.round((((publishedCount ?? 0) + (scheduledCount ?? 0)) / totalPosts) * 100)}% ${Math.round((((publishedCount ?? 0) + (scheduledCount ?? 0) + (draftCount ?? 0)) / totalPosts) * 100)}%,
                      oklch(0.64 0.21 25.39) ${Math.round((((publishedCount ?? 0) + (scheduledCount ?? 0) + (draftCount ?? 0)) / totalPosts) * 100)}% 100%
                    )`,
                  }}
                />
                <div className="space-y-1 text-xs text-muted-foreground">
                  <div className="flex items-center gap-2">
                    <span className="h-2 w-2 rounded-full bg-primary inline-block" />
                    Published ({publishedCount ?? 0})
                  </div>
                  <div className="flex items-center gap-2">
                    <span className="h-2 w-2 rounded-full bg-amber-400 inline-block" />
                    Scheduled ({scheduledCount ?? 0})
                  </div>
                  <div className="flex items-center gap-2">
                    <span className="h-2 w-2 rounded-full bg-muted-foreground inline-block" />
                    Draft ({draftCount ?? 0})
                  </div>
                  <div className="flex items-center gap-2">
                    <span className="h-2 w-2 rounded-full bg-destructive inline-block" />
                    Archived ({archivedCount ?? 0})
                  </div>
                </div>
              </div>
            )}
          </CardContent>
        </Card>

        {/* Recent posts */}
        <Card>
          <CardHeader className="pb-3">
            <div className="flex items-center justify-between">
              <div>
                <CardTitle className="text-base">Recent Posts</CardTitle>
                <CardDescription>Latest content activity</CardDescription>
              </div>
              <Button asChild variant="ghost" size="sm" className="text-xs">
                <Link href="/admin/posts">View all</Link>
              </Button>
            </div>
          </CardHeader>
          <CardContent className="space-y-1">
            {postsLoading
              ? Array.from({ length: 5 }).map((_, i) => (
                  <Skeleton key={i} className="h-12 w-full rounded-lg" />
                ))
              : recentPosts?.items.map((post) => (
                  <Link
                    key={post.id}
                    href={`/admin/posts/${post.id}/edit`}
                    className="flex items-start gap-3 rounded-lg p-2.5 hover:bg-accent transition-colors group"
                  >
                    <div className="shrink-0 mt-0.5">
                      {post.status === PostStatus.Published ? (
                        <div className="h-2 w-2 rounded-full bg-green-500 mt-1.5" />
                      ) : post.status === PostStatus.Scheduled ? (
                        <div className="h-2 w-2 rounded-full bg-amber-500 mt-1.5" />
                      ) : post.status === PostStatus.Archived ? (
                        <div className="h-2 w-2 rounded-full bg-destructive mt-1.5" />
                      ) : (
                        <div className="h-2 w-2 rounded-full bg-muted-foreground mt-1.5" />
                      )}
                    </div>
                    <div className="flex-1 min-w-0">
                      <p className="text-sm font-medium line-clamp-1 group-hover:text-primary transition-colors">
                        {post.title}
                      </p>
                      <p className="text-xs text-muted-foreground">
                        {post.publishedAt
                          ? formatDate(post.publishedAt)
                          : "Not published"}
                      </p>
                    </div>
                    <Edit3 className="h-3.5 w-3.5 text-muted-foreground shrink-0 opacity-0 group-hover:opacity-100 transition-opacity mt-0.5" />
                  </Link>
                ))}
          </CardContent>
        </Card>
      </div>

      {/* ── Quick Actions ── */}
      <Card>
        <CardHeader className="pb-3">
          <CardTitle className="text-base">Quick Actions</CardTitle>
        </CardHeader>
        <CardContent className="flex flex-wrap gap-3">
          <Button asChild className="gap-2">
            <Link href="/admin/posts/new">
              <Plus className="h-4 w-4" />
              New Post
            </Link>
          </Button>
          <Button asChild variant="outline" size="sm">
            <Link href="/admin/categories" className="gap-2">
              <Hash className="h-4 w-4" />
              Add Category
            </Link>
          </Button>
          <Button asChild variant="outline" size="sm">
            <Link href="/admin/tags" className="gap-2">
              <Tag className="h-4 w-4" />
              Add Tag
            </Link>
          </Button>
          <Button asChild variant="outline" size="sm">
            <Link href="/admin/posts" className="gap-2">
              <Archive className="h-4 w-4" />
              Manage Posts
            </Link>
          </Button>
        </CardContent>
      </Card>
    </div>
  );
}
