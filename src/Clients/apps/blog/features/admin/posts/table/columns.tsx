import { format } from "date-fns";

import type { PostSummaryModel } from "@workspace/types/blog";
import { PostStatus, PostType } from "@workspace/types/blog";
import { Badge } from "@workspace/ui/components/badge";

const TYPE_LABELS: Record<PostType, string> = {
  [PostType.News]: "News",
  [PostType.Changelog]: "Changelog",
};

const STATUS_LABELS: Record<PostStatus, string> = {
  [PostStatus.Draft]: "Draft",
  [PostStatus.Scheduled]: "Scheduled",
  [PostStatus.Published]: "Published",
  [PostStatus.Archived]: "Archived",
};

const STATUS_VARIANTS: Record<
  PostStatus,
  "default" | "secondary" | "destructive" | "outline"
> = {
  [PostStatus.Draft]: "secondary",
  [PostStatus.Scheduled]: "outline",
  [PostStatus.Published]: "default",
  [PostStatus.Archived]: "destructive",
};

export function renderPostTitle(post: PostSummaryModel) {
  return (
    <div className="flex flex-col">
      <span className="line-clamp-1 font-medium">{post.title}</span>
      <span className="text-muted-foreground font-mono text-xs">/{post.slug}</span>
    </div>
  );
}

export function renderPostStatus(post: PostSummaryModel) {
  return (
    <Badge variant={STATUS_VARIANTS[post.status]} className="text-xs">
      {STATUS_LABELS[post.status]}
    </Badge>
  );
}

export function renderPostType(post: PostSummaryModel) {
  return (
    <Badge variant="outline" className="text-xs">
      {TYPE_LABELS[post.type]}
    </Badge>
  );
}

export function renderPostDate(post: PostSummaryModel) {
  if (post.status === PostStatus.Scheduled && post.scheduledAt) {
    return (
      <span className="text-amber-600 dark:text-amber-400">
        {format(new Date(post.scheduledAt), "MMM d, yyyy HH:mm")}
      </span>
    );
  }

  if (post.publishedAt) {
    return format(new Date(post.publishedAt), "MMM d, yyyy");
  }

  return "—";
}
