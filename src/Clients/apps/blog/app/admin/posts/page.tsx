"use client";

import { useState } from "react";

import Link from "next/link";

import { format } from "date-fns";
import {
  Archive,
  Calendar,
  Edit,
  Eye,
  Loader2,
  MoreHorizontal,
  Plus,
  Send,
} from "lucide-react";
import { toast } from "sonner";

import useDeletePost from "@workspace/api-hooks/blog/useDeletePost";
import useGetAdminPosts from "@workspace/api-hooks/blog/useGetAdminPosts";
import usePublishPost from "@workspace/api-hooks/blog/usePublishPost";
import { PostStatus, PostType } from "@workspace/types/blog";
import { Badge } from "@workspace/ui/components/badge";
import { Button } from "@workspace/ui/components/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@workspace/ui/components/dialog";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@workspace/ui/components/dropdown-menu";
import { Input } from "@workspace/ui/components/input";
import { Label } from "@workspace/ui/components/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@workspace/ui/components/select";
import { Skeleton } from "@workspace/ui/components/skeleton";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@workspace/ui/components/table";

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

/** Минимальное значение для datetime-local: текущий момент в формате "YYYY-MM-DDThh:mm" */
function nowDatetimeLocal(): string {
  const now = new Date();
  now.setSeconds(0, 0);
  return now.toISOString().slice(0, 16);
}

export default function AdminPostsPage() {
  const [page, setPage] = useState(1);
  const [statusFilter, setStatusFilter] = useState<PostStatus | undefined>(
    undefined,
  );
  // Состояние диалога планирования: id поста и выбранная дата/время
  const [scheduleDialog, setScheduleDialog] = useState<{
    postId: string;
    scheduledAt: string;
  } | null>(null);
  const [archivePostId, setArchivePostId] = useState<string | null>(null);

  const { data, isLoading } = useGetAdminPosts({
    pageIndex: page,
    pageSize: 20,
    status: statusFilter,
  });
  const { mutate: publish, isPending: isPublishing } = usePublishPost();
  const { mutate: deletePost, isPending: isDeleting } = useDeletePost();

  const handlePublish = (postId: string) => {
    publish(
      { postId },
      {
        onSuccess: () => toast.success("Post published successfully"),
        onError: () => toast.error("Failed to publish post"),
      },
    );
  };

  const handleScheduleConfirm = () => {
    if (!scheduleDialog) return;

    publish(
      {
        postId: scheduleDialog.postId,
        request: {
          scheduledAt: new Date(scheduleDialog.scheduledAt).toISOString(),
        },
      },
      {
        onSuccess: () => {
          toast.success("Post scheduled successfully");
          setScheduleDialog(null);
        },
        onError: () => toast.error("Failed to schedule post"),
      },
    );
  };

  const handleArchiveConfirm = () => {
    if (archivePostId === null) return;
    deletePost(archivePostId, {
      onSuccess: () => {
        toast.success("Post archived");
        setArchivePostId(null);
      },
      onError: () => toast.error("Failed to archive post"),
    });
  };

  const handleStatusFilter = (value: string) => {
    setPage(1);
    setStatusFilter(
      value === "all" ? undefined : (Number(value) as PostStatus),
    );
  };

  return (
    <div>
      <div className="mb-6 flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold tracking-tight">Posts</h1>
          <p className="text-muted-foreground mt-1">Manage all blog posts.</p>
        </div>
        <Button asChild size="sm" className="gap-2">
          <Link href="/admin/posts/new">
            <Plus className="h-4 w-4" />
            New Post
          </Link>
        </Button>
      </div>

      <div className="mb-4 flex gap-3">
        <Select defaultValue="all" onValueChange={handleStatusFilter}>
          <SelectTrigger className="w-40">
            <SelectValue placeholder="All statuses" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="all">All statuses</SelectItem>
            <SelectItem value={String(PostStatus.Draft)}>Draft</SelectItem>
            <SelectItem value={String(PostStatus.Scheduled)}>
              Scheduled
            </SelectItem>
            <SelectItem value={String(PostStatus.Published)}>
              Published
            </SelectItem>
            <SelectItem value={String(PostStatus.Archived)}>
              Archived
            </SelectItem>
          </SelectContent>
        </Select>
      </div>

      {isLoading ? (
        <div className="space-y-3">
          {Array.from({ length: 8 }).map((_, i) => (
            <Skeleton key={i} className="h-14 w-full rounded-md" />
          ))}
        </div>
      ) : (
        <>
          <div className="border-border overflow-hidden rounded-xl border">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Title</TableHead>
                  <TableHead className="hidden sm:table-cell">Status</TableHead>
                  <TableHead className="hidden sm:table-cell">Type</TableHead>
                  <TableHead className="hidden md:table-cell">Date</TableHead>
                  <TableHead className="hidden lg:table-cell">Likes</TableHead>
                  <TableHead className="w-12" />
                </TableRow>
              </TableHeader>
              <TableBody>
                {data?.items.map((post) => (
                  <TableRow key={post.id}>
                    <TableCell>
                      <div className="flex flex-col">
                        <span className="line-clamp-1 font-medium">
                          {post.title}
                        </span>
                        <span className="text-muted-foreground font-mono text-xs">
                          /{post.slug}
                        </span>
                      </div>
                    </TableCell>
                    <TableCell className="hidden sm:table-cell">
                      <Badge
                        variant={STATUS_VARIANTS[post.status]}
                        className="text-xs"
                      >
                        {STATUS_LABELS[post.status]}
                      </Badge>
                    </TableCell>
                    <TableCell className="hidden sm:table-cell">
                      <Badge variant="outline" className="text-xs">
                        {TYPE_LABELS[post.type]}
                      </Badge>
                    </TableCell>
                    <TableCell className="text-muted-foreground hidden text-sm md:table-cell">
                      {post.status === PostStatus.Scheduled &&
                      post.scheduledAt ? (
                        <span className="flex items-center gap-1 text-amber-600 dark:text-amber-400">
                          <Calendar className="h-3 w-3" />
                          {format(
                            new Date(post.scheduledAt),
                            "MMM d, yyyy HH:mm",
                          )}
                        </span>
                      ) : post.publishedAt ? (
                        format(new Date(post.publishedAt), "MMM d, yyyy")
                      ) : (
                        "—"
                      )}
                    </TableCell>
                    <TableCell className="text-muted-foreground hidden text-sm lg:table-cell">
                      {post.likesCount}
                    </TableCell>
                    <TableCell>
                      <DropdownMenu>
                        <DropdownMenuTrigger asChild>
                          <Button
                            variant="ghost"
                            size="icon"
                            className="h-8 w-8"
                          >
                            <MoreHorizontal className="h-4 w-4" />
                          </Button>
                        </DropdownMenuTrigger>
                        <DropdownMenuContent align="end">
                          <DropdownMenuItem asChild>
                            <Link
                              href={`/${post.slug}`}
                              target="_blank"
                              className="flex items-center gap-2"
                            >
                              <Eye className="h-4 w-4" />
                              View
                            </Link>
                          </DropdownMenuItem>
                          <DropdownMenuItem asChild>
                            <Link
                              href={`/admin/posts/${post.id}/edit`}
                              className="flex items-center gap-2"
                            >
                              <Edit className="h-4 w-4" />
                              Edit
                            </Link>
                          </DropdownMenuItem>
                          <DropdownMenuSeparator />
                          <DropdownMenuItem
                            onClick={() => handlePublish(post.id)}
                            disabled={
                              isPublishing ||
                              post.status === PostStatus.Published
                            }
                            className="flex items-center gap-2"
                          >
                            {isPublishing ? (
                              <Loader2 className="h-4 w-4 animate-spin" />
                            ) : (
                              <Send className="h-4 w-4" />
                            )}
                            Publish now
                          </DropdownMenuItem>
                          <DropdownMenuItem
                            onClick={() =>
                              setScheduleDialog({
                                postId: post.id,
                                scheduledAt: nowDatetimeLocal(),
                              })
                            }
                            disabled={post.status === PostStatus.Published}
                            className="flex items-center gap-2"
                          >
                            <Calendar className="h-4 w-4" />
                            Schedule...
                          </DropdownMenuItem>
                          <DropdownMenuSeparator />
                          <DropdownMenuItem
                            onClick={() => setArchivePostId(post.id)}
                            disabled={
                              isDeleting || post.status === PostStatus.Archived
                            }
                            className="text-destructive focus:text-destructive flex items-center gap-2"
                          >
                            <Archive className="h-4 w-4" />
                            Archive
                          </DropdownMenuItem>
                        </DropdownMenuContent>
                      </DropdownMenu>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>

          {/* Pagination */}
          {data && data.totalPages > 1 && (
            <div className="text-muted-foreground mt-4 flex items-center justify-between text-sm">
              <span>{data.totalItems} total posts</span>
              <div className="flex gap-2">
                <Button
                  variant="outline"
                  size="sm"
                  disabled={!data.hasPreviousPage}
                  onClick={() => setPage((p) => p - 1)}
                >
                  Previous
                </Button>
                <span className="flex items-center px-3">
                  {page} / {data.totalPages}
                </span>
                <Button
                  variant="outline"
                  size="sm"
                  disabled={!data.hasNextPage}
                  onClick={() => setPage((p) => p + 1)}
                >
                  Next
                </Button>
              </div>
            </div>
          )}
        </>
      )}

      {/* Archive confirmation dialog */}
      <Dialog
        open={archivePostId !== null}
        onOpenChange={(open) => !open && setArchivePostId(null)}
      >
        <DialogContent className="sm:max-w-sm">
          <DialogHeader>
            <DialogTitle>Archive post</DialogTitle>
            <DialogDescription>
              Archive this post? It will no longer be visible publicly.
            </DialogDescription>
          </DialogHeader>
          <DialogFooter>
            <Button variant="outline" onClick={() => setArchivePostId(null)}>
              Cancel
            </Button>
            <Button
              variant="destructive"
              onClick={handleArchiveConfirm}
              disabled={isDeleting}
              className="gap-2"
            >
              {isDeleting && <Loader2 className="h-4 w-4 animate-spin" />}
              Archive
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* Schedule dialog */}
      <Dialog
        open={scheduleDialog !== null}
        onOpenChange={(open) => !open && setScheduleDialog(null)}
      >
        <DialogContent className="sm:max-w-sm">
          <DialogHeader>
            <DialogTitle>Schedule post</DialogTitle>
          </DialogHeader>
          <div className="space-y-2 py-2">
            <Label htmlFor="scheduled-at">Publish at</Label>
            <Input
              id="scheduled-at"
              type="datetime-local"
              min={nowDatetimeLocal()}
              value={scheduleDialog?.scheduledAt ?? ""}
              onChange={(e) =>
                scheduleDialog &&
                setScheduleDialog({
                  ...scheduleDialog,
                  scheduledAt: e.target.value,
                })
              }
            />
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setScheduleDialog(null)}>
              Cancel
            </Button>
            <Button
              onClick={handleScheduleConfirm}
              disabled={isPublishing || !scheduleDialog?.scheduledAt}
              className="gap-2"
            >
              {isPublishing && <Loader2 className="h-4 w-4 animate-spin" />}
              Schedule
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
