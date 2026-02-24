"use client";

import { useState } from "react";

import Link from "next/link";

import { Loader2, Plus } from "lucide-react";
import { toast } from "sonner";

import useDeletePost from "@workspace/api-hooks/blog/useDeletePost";
import usePublishPost from "@workspace/api-hooks/blog/usePublishPost";
import { PostStatus } from "@workspace/types/blog";
import { Button } from "@workspace/ui/components/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@workspace/ui/components/dialog";
import { Input } from "@workspace/ui/components/input";
import { Label } from "@workspace/ui/components/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@workspace/ui/components/select";

import { PostsTable } from "./table/table";

function nowDatetimeLocal(): string {
  const now = new Date();
  now.setSeconds(0, 0);
  return now.toISOString().slice(0, 16);
}

export function AdminPostsSection() {
  const [statusFilter, setStatusFilter] = useState<PostStatus | undefined>(
    undefined,
  );
  const [scheduleDialog, setScheduleDialog] = useState<{
    postId: string;
    scheduledAt: string;
  } | null>(null);
  const [archivePostId, setArchivePostId] = useState<string | null>(null);

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
        <Select
          defaultValue="all"
          onValueChange={(value) =>
            setStatusFilter(
              value === "all" ? undefined : (Number(value) as PostStatus),
            )
          }
        >
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

      <PostsTable
        status={statusFilter}
        isPublishing={isPublishing}
        isDeleting={isDeleting}
        onPublish={handlePublish}
        onSchedule={(postId) =>
          setScheduleDialog({
            postId,
            scheduledAt: nowDatetimeLocal(),
          })
        }
        onArchive={setArchivePostId}
      />

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
