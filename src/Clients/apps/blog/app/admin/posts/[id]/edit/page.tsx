"use client";

import { use } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";

import { ArrowLeft, FileEdit } from "lucide-react";
import { toast } from "sonner";

import { Skeleton } from "@workspace/ui/components/skeleton";
import useGetAdminPost from "@workspace/api-hooks/blog/useGetAdminPost";
import useUpdatePost from "@workspace/api-hooks/blog/useUpdatePost";
import useGetCategories from "@workspace/api-hooks/blog/useGetCategories";
import useGetTags from "@workspace/api-hooks/blog/useGetTags";

import { PostForm, type PostFormValues } from "@/components/admin/post-form";

function EditPostSkeleton() {
  return (
    <div className="max-w-3xl space-y-6">
      <Skeleton className="h-4 w-28" />
      <div className="rounded-xl border border-border p-5 space-y-3">
        <div className="flex items-center gap-4">
          <Skeleton className="h-10 w-10 rounded-xl shrink-0" />
          <div className="space-y-1.5">
            <Skeleton className="h-5 w-36" />
            <Skeleton className="h-4 w-52" />
          </div>
        </div>
      </div>
      <div className="rounded-xl border border-border p-6 space-y-5">
        <Skeleton className="h-3.5 w-20" />
        <Skeleton className="h-10 w-full" />
        <Skeleton className="h-10 w-full" />
        <Skeleton className="h-24 w-full" />
      </div>
      <div className="rounded-xl border border-border p-6 space-y-4">
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
  const postId = Number(id);

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
      <div className="py-12 text-center text-muted-foreground">
        Post not found.
      </div>
    );
  }

  return (
    <div className="max-w-3xl">
      {/* Back link */}
      <Link
        href="/admin/posts"
        className="inline-flex items-center gap-2 text-sm text-muted-foreground hover:text-foreground transition-colors mb-6 group"
      >
        <ArrowLeft className="h-4 w-4 transition-transform group-hover:-translate-x-1" />
        Back to posts
      </Link>

      {/* Page header */}
      <div className="relative rounded-xl overflow-hidden border border-border bg-gradient-to-br from-primary/5 to-primary/0 px-6 py-5 mb-8">
        <div className="pointer-events-none absolute top-0 right-0 w-40 h-40 rounded-full bg-primary/10 blur-2xl opacity-60" />
        <div className="relative flex items-center gap-4">
          <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-xl bg-primary/10">
            <FileEdit className="h-5 w-5 text-primary" />
          </div>
          <div className="min-w-0">
            <h1 className="text-xl font-bold tracking-tight">Edit Post</h1>
            <p className="text-sm text-muted-foreground mt-0.5 truncate">
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
