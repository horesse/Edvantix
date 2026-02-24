"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";

import { ArrowLeft, FilePlus } from "lucide-react";
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
            <FilePlus className="text-primary h-5 w-5" />
          </div>
          <div>
            <h1 className="text-xl font-bold tracking-tight">New Post</h1>
            <p className="text-muted-foreground mt-0.5 text-sm">
              Create a new blog post. It will be saved as a draft.
            </p>
          </div>
        </div>
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
