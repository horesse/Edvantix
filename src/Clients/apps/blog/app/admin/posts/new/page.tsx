"use client";

import { useRouter } from "next/navigation";

import { toast } from "sonner";

import useCreatePost from "@workspace/api-hooks/blog/useCreatePost";
import useGetCategories from "@workspace/api-hooks/blog/useGetCategories";
import useGetTags from "@workspace/api-hooks/blog/useGetTags";

import { PostForm, type PostFormValues } from "@/components/admin/post-form";

export default function NewPostPage() {
  const router = useRouter();
  const { data: categories = [] } = useGetCategories();
  const { data: tags = [] } = useGetTags();
  const { mutate: createPost, isPending } = useCreatePost();

  const handleSubmit = (values: PostFormValues) => {
    createPost(
      {
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
      {
        onSuccess: () => {
          toast.success("Post saved as draft");
          router.push("/admin/posts");
        },
        onError: () => toast.error("Failed to create post"),
      },
    );
  };

  return (
    <div className="max-w-3xl">
      <div className="mb-6">
        <h1 className="text-2xl font-bold tracking-tight">New Post</h1>
        <p className="text-muted-foreground mt-1">
          Create a new blog post. It will be saved as a draft.
        </p>
      </div>

      <PostForm
        categories={categories}
        tags={tags}
        onSubmit={handleSubmit}
        isSubmitting={isPending}
        submitLabel="Save draft"
      />
    </div>
  );
}
