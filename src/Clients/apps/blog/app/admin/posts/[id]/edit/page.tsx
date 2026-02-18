"use client";

import { use } from "react";
import { useRouter, useSearchParams } from "next/navigation";

import { Loader2 } from "lucide-react";
import { toast } from "sonner";

import useGetPost from "@workspace/api-hooks/blog/useGetPost";
import useUpdatePost from "@workspace/api-hooks/blog/useUpdatePost";
import useGetCategories from "@workspace/api-hooks/blog/useGetCategories";
import useGetTags from "@workspace/api-hooks/blog/useGetTags";

import { PostForm, type PostFormValues } from "@/components/admin/post-form";

export default function EditPostPage({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = use(params);
  const postId = Number(id);
  const searchParams = useSearchParams();
  const slug = searchParams.get("slug") ?? "";

  const router = useRouter();
  const { data: post, isLoading } = useGetPost(slug);
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

  if (isLoading) {
    return (
      <div className="flex items-center justify-center py-24">
        <Loader2 className="h-6 w-6 animate-spin text-muted-foreground" />
      </div>
    );
  }

  if (!post) {
    return (
      <div className="py-12 text-center text-muted-foreground">
        Post not found.
      </div>
    );
  }

  return (
    <div className="max-w-3xl">
      <div className="mb-6">
        <h1 className="text-2xl font-bold tracking-tight">Edit Post</h1>
        <p className="text-muted-foreground mt-1">
          Update the post content and settings.
        </p>
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
