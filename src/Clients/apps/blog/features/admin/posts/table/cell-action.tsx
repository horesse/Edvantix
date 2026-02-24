"use client";

import Link from "next/link";

import { Archive, Calendar, Edit, Eye, Loader2, Send } from "lucide-react";

import type { PostSummaryModel } from "@workspace/types/blog";
import { PostStatus } from "@workspace/types/blog";
import { Button } from "@workspace/ui/components/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@workspace/ui/components/dropdown-menu";

type PostCellActionProps = Readonly<{
  post: PostSummaryModel;
  isPublishing: boolean;
  isDeleting: boolean;
  onPublish: (postId: string) => void;
  onSchedule: (postId: string) => void;
  onArchive: (postId: string) => void;
}>;

export function PostCellAction({
  post,
  isPublishing,
  isDeleting,
  onPublish,
  onSchedule,
  onArchive,
}: PostCellActionProps) {
  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <Button variant="ghost" size="icon" className="h-8 w-8">
          <span className="sr-only">Open menu</span>
          <span aria-hidden="true">⋯</span>
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end">
        <DropdownMenuItem asChild>
          <Link href={`/${post.slug}`} target="_blank" className="flex items-center gap-2">
            <Eye className="h-4 w-4" />
            View
          </Link>
        </DropdownMenuItem>
        <DropdownMenuItem asChild>
          <Link href={`/admin/posts/${post.id}/edit`} className="flex items-center gap-2">
            <Edit className="h-4 w-4" />
            Edit
          </Link>
        </DropdownMenuItem>
        <DropdownMenuSeparator />
        <DropdownMenuItem
          onClick={() => onPublish(post.id)}
          disabled={isPublishing || post.status === PostStatus.Published}
          className="flex items-center gap-2"
        >
          {isPublishing ? <Loader2 className="h-4 w-4 animate-spin" /> : <Send className="h-4 w-4" />}
          Publish now
        </DropdownMenuItem>
        <DropdownMenuItem
          onClick={() => onSchedule(post.id)}
          disabled={post.status === PostStatus.Published}
          className="flex items-center gap-2"
        >
          <Calendar className="h-4 w-4" />
          Schedule...
        </DropdownMenuItem>
        <DropdownMenuSeparator />
        <DropdownMenuItem
          onClick={() => onArchive(post.id)}
          disabled={isDeleting || post.status === PostStatus.Archived}
          className="text-destructive focus:text-destructive flex items-center gap-2"
        >
          <Archive className="h-4 w-4" />
          Archive
        </DropdownMenuItem>
      </DropdownMenuContent>
    </DropdownMenu>
  );
}
