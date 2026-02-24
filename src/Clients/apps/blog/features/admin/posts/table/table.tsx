"use client";

import { useEffect, useState } from "react";

import useGetAdminPosts from "@workspace/api-hooks/blog/useGetAdminPosts";
import { PostStatus } from "@workspace/types/blog";
import { Button } from "@workspace/ui/components/button";
import { Skeleton } from "@workspace/ui/components/skeleton";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@workspace/ui/components/table";

import { PostCellAction } from "./cell-action";
import {
  renderPostDate,
  renderPostStatus,
  renderPostTitle,
  renderPostType,
} from "./columns";

type PostsTableProps = Readonly<{
  status?: PostStatus;
  isPublishing: boolean;
  isDeleting: boolean;
  onPublish: (postId: string) => void;
  onSchedule: (postId: string) => void;
  onArchive: (postId: string) => void;
}>;

export function PostsTable({
  status,
  isPublishing,
  isDeleting,
  onPublish,
  onSchedule,
  onArchive,
}: PostsTableProps) {
  const pageSize = 20;
  const [page, setPage] = useState(1);

  useEffect(() => {
    setPage(1);
  }, [status]);

  const { data, isLoading } = useGetAdminPosts({
    pageIndex: page,
    pageSize,
    status,
  });

  const totalCount = data?.totalCount ?? 0;
  const totalPages = Math.max(1, Math.ceil(totalCount / pageSize));
  const hasPreviousPage = page > 1;
  const hasNextPage = page < totalPages;

  if (isLoading) {
    return (
      <div className="space-y-3">
        {Array.from({ length: 8 }).map((_, i) => (
          <Skeleton key={i} className="h-14 w-full rounded-md" />
        ))}
      </div>
    );
  }

  return (
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
            {data?.map((post) => (
              <TableRow key={post.id}>
                <TableCell>{renderPostTitle(post)}</TableCell>
                <TableCell className="hidden sm:table-cell">
                  {renderPostStatus(post)}
                </TableCell>
                <TableCell className="hidden sm:table-cell">
                  {renderPostType(post)}
                </TableCell>
                <TableCell className="text-muted-foreground hidden text-sm md:table-cell">
                  {renderPostDate(post)}
                </TableCell>
                <TableCell className="text-muted-foreground hidden text-sm lg:table-cell">
                  {post.likesCount}
                </TableCell>
                <TableCell>
                  <PostCellAction
                    post={post}
                    isPublishing={isPublishing}
                    isDeleting={isDeleting}
                    onPublish={onPublish}
                    onSchedule={onSchedule}
                    onArchive={onArchive}
                  />
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </div>

      {data && totalPages > 1 && (
        <div className="text-muted-foreground mt-4 flex items-center justify-between text-sm">
          <span>{totalCount} total posts</span>
          <div className="flex gap-2">
            <Button
              variant="outline"
              size="sm"
              disabled={!hasPreviousPage}
              onClick={() => setPage((p) => p - 1)}
            >
              Previous
            </Button>
            <span className="flex items-center px-3">
              {page} / {totalPages}
            </span>
            <Button
              variant="outline"
              size="sm"
              disabled={!hasNextPage}
              onClick={() => setPage((p) => p + 1)}
            >
              Next
            </Button>
          </div>
        </div>
      )}
    </>
  );
}
