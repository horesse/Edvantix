"use client";

import { use } from "react";

import Link from "next/link";
import { useRouter } from "next/navigation";

import { ArrowLeft, FileEdit } from "lucide-react";
import { toast } from "sonner";

import useGetAdminPost from "@workspace/api-hooks/blog/useGetAdminPost";
import useGetCategories from "@workspace/api-hooks/blog/useGetCategories";
import useGetTags from "@workspace/api-hooks/blog/useGetTags";
import useUpdatePost from "@workspace/api-hooks/blog/useUpdatePost";
import { Skeleton } from "@workspace/ui/components/skeleton";

import { PostForm, type PostFormValues } from "@/components/admin/post-form";

function EditPostSkeleton() {
  return (
    <div className="max-w-3xl space-y-6">
      <Skeleton className="h-4 w-28" />
      <div className="border-border space-y-3 rounded-xl border p-5">
        <div className="flex items-center gap-4">
          <Skeleton className="h-10 w-10 shrink-0 rounded-xl" />
          <div className="space-y-1.5">
            <Skeleton className="h-5 w-36" />
            <Skeleton className="h-4 w-52" />
          </div>
        </div>
      </div>
      <div className="border-border space-y-5 rounded-xl border p-6">
        <Skeleton className="h-3.5 w-20" />
        <Skeleton className="h-10 w-full" />
        <Skeleton className="h-10 w-full" />
        <Skeleton className="h-24 w-full" />
      </div>
      <div className="border-border space-y-4 rounded-xl border p-6">
        <Skeleton className="h-3.5 w-20" />
        <div className="grid grid-cols-2 gap-5">
          <Skeleton className="h-10 w-full" />
          <Skeleton className="h-10 w-full" />
        </div>
      </div>
      <Skeleton className="h-80 w-full rounded-xl" />
    </div>
  );
}

export default function EditPostPage({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = use(params);
  const postId = id;

  const router = useRouter();
  const { data: post, isLoading } = useGetAdminPost(postId);
  const { data: categories = [] } = useGetCategories();
  const { data: tags = [] } = useGetTags();
  const { mutate: updatePost, isPending } = useUpdatePost();

  const handleSubmit = (values: PostFormValues) => {
    updatePost(
      {
        postId,
        request: {
          title: values.title,
          slug: values.slug,
          content: values.content,
          summary: values.summary,
          type: Number(values.type),
          isPremium: values.isPremium,
          coverImageUrl: values.coverImageUrl || undefined,
          categoryIds: values.categoryIds,
          tagIds: values.tagIds,
        },
      },
      {
        onSuccess: () => {
          toast.success("Post updated");
          router.push("/admin/posts");
        },
        onError: () => toast.error("Failed to update post"),
      },
    );
  };

  if (isLoading) return <EditPostSkeleton />;

  if (!post) {
    return (
      <div className="text-muted-foreground py-12 text-center">
        Post not found.
      </div>
    );
  }

  return (
    <div className="max-w-3xl">
      {/* Back link */}
      <Link
        href="/admin/posts"
        className="text-muted-foreground hover:text-foreground group mb-6 inline-flex items-center gap-2 text-sm transition-colors"
      >
        <ArrowLeft className="h-4 w-4 transition-transform group-hover:-translate-x-1" />
        Back to posts
      </Link>

      {/* Page header */}
      <div className="border-border from-primary/5 to-primary/0 relative mb-8 overflow-hidden rounded-xl border bg-gradient-to-br px-6 py-5">
        <div className="bg-primary/10 pointer-events-none absolute top-0 right-0 h-40 w-40 rounded-full opacity-60 blur-2xl" />
        <div className="relative flex items-center gap-4">
          <div className="bg-primary/10 flex h-10 w-10 shrink-0 items-center justify-center rounded-xl">
            <FileEdit className="text-primary h-5 w-5" />
          </div>
          <div className="min-w-0">
            <h1 className="text-xl font-bold tracking-tight">Edit Post</h1>
            <p className="text-muted-foreground mt-0.5 truncate text-sm">
              {post.title}
            </p>
          </div>
        </div>
      </div>

      <PostForm
        defaultValues={post}
        categories={categories}
        tags={tags}
        onSubmit={handleSubmit}
        isSubmitting={isPending}
        submitLabel="Save changes"
      />
    </div>
  );
}
