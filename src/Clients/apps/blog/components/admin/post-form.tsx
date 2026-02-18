"use client";

import { useEffect } from "react";
import { useForm } from "react-hook-form";

import { Loader2 } from "lucide-react";

import { Badge } from "@workspace/ui/components/badge";
import { Button } from "@workspace/ui/components/button";
import { Checkbox } from "@workspace/ui/components/checkbox";
import {
  Form,
  FormControl,
  FormDescription,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@workspace/ui/components/form";
import { Input } from "@workspace/ui/components/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@workspace/ui/components/select";
import { Textarea } from "@workspace/ui/components/textarea";
import type { CategoryModel, PostModel, TagModel } from "@workspace/types/blog";
import { PostType } from "@workspace/types/blog";

import { slugify } from "@/lib/utils";

export type PostFormValues = {
  title: string;
  slug: string;
  content: string;
  summary: string;
  type: string;
  isPremium: boolean;
  coverImageUrl: string;
  categoryIds: number[];
  tagIds: number[];
};

type PostFormProps = {
  defaultValues?: Partial<PostModel>;
  categories: CategoryModel[];
  tags: TagModel[];
  onSubmit: (values: PostFormValues) => void;
  isSubmitting: boolean;
  submitLabel?: string;
};

export function PostForm({
  defaultValues,
  categories,
  tags,
  onSubmit,
  isSubmitting,
  submitLabel = "Save draft",
}: PostFormProps) {
  const form = useForm<PostFormValues>({
    defaultValues: {
      title: defaultValues?.title ?? "",
      slug: defaultValues?.slug ?? "",
      content: defaultValues?.content ?? "",
      summary: defaultValues?.summary ?? "",
      type: String(defaultValues?.type ?? PostType.News),
      isPremium: defaultValues?.isPremium ?? false,
      coverImageUrl: defaultValues?.coverImageUrl ?? "",
      categoryIds: defaultValues?.categories?.map((c) => c.id) ?? [],
      tagIds: defaultValues?.tags?.map((t) => t.id) ?? [],
    },
  });

  const title = form.watch("title");

  // Auto-generate slug from title if slug is empty
  useEffect(() => {
    if (!defaultValues?.slug) {
      form.setValue("slug", slugify(title));
    }
  }, [title, defaultValues?.slug, form]);

  const toggleId = (field: "categoryIds" | "tagIds", id: number) => {
    const current = form.getValues(field);
    form.setValue(
      field,
      current.includes(id) ? current.filter((v) => v !== id) : [...current, id],
    );
  };

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
        {/* Title */}
        <FormField
          control={form.control}
          name="title"
          rules={{ required: "Title is required" }}
          render={({ field }) => (
            <FormItem>
              <FormLabel>Title *</FormLabel>
              <FormControl>
                <Input placeholder="Post title" {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />

        {/* Slug */}
        <FormField
          control={form.control}
          name="slug"
          rules={{ required: "Slug is required" }}
          render={({ field }) => (
            <FormItem>
              <FormLabel>Slug *</FormLabel>
              <FormControl>
                <Input placeholder="post-url-slug" {...field} />
              </FormControl>
              <FormDescription>URL-friendly identifier for this post.</FormDescription>
              <FormMessage />
            </FormItem>
          )}
        />

        {/* Summary */}
        <FormField
          control={form.control}
          name="summary"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Summary</FormLabel>
              <FormControl>
                <Textarea
                  placeholder="Brief description of the post…"
                  rows={3}
                  {...field}
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />

        {/* Type */}
        <FormField
          control={form.control}
          name="type"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Type</FormLabel>
              <Select onValueChange={field.onChange} defaultValue={field.value}>
                <FormControl>
                  <SelectTrigger className="w-48">
                    <SelectValue />
                  </SelectTrigger>
                </FormControl>
                <SelectContent>
                  <SelectItem value={String(PostType.News)}>News</SelectItem>
                  <SelectItem value={String(PostType.Changelog)}>
                    Changelog
                  </SelectItem>
                </SelectContent>
              </Select>
              <FormMessage />
            </FormItem>
          )}
        />

        {/* Cover image URL */}
        <FormField
          control={form.control}
          name="coverImageUrl"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Cover Image URL</FormLabel>
              <FormControl>
                <Input placeholder="https://..." {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />

        {/* Premium */}
        <FormField
          control={form.control}
          name="isPremium"
          render={({ field }) => (
            <FormItem className="flex items-center gap-3 space-y-0">
              <FormControl>
                <Checkbox
                  checked={field.value}
                  onCheckedChange={field.onChange}
                />
              </FormControl>
              <div>
                <FormLabel>Premium content</FormLabel>
                <FormDescription className="text-xs">
                  Only visible to subscribers.
                </FormDescription>
              </div>
            </FormItem>
          )}
        />

        {/* Categories */}
        <FormField
          control={form.control}
          name="categoryIds"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Categories</FormLabel>
              <div className="flex flex-wrap gap-2 mt-1">
                {categories.map((cat) => (
                  <Badge
                    key={cat.id}
                    variant={
                      field.value.includes(cat.id) ? "default" : "outline"
                    }
                    className="cursor-pointer select-none transition-colors"
                    onClick={() => toggleId("categoryIds", cat.id)}
                  >
                    {cat.name}
                  </Badge>
                ))}
              </div>
              <FormMessage />
            </FormItem>
          )}
        />

        {/* Tags */}
        <FormField
          control={form.control}
          name="tagIds"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Tags</FormLabel>
              <div className="flex flex-wrap gap-2 mt-1">
                {tags.map((tag) => (
                  <Badge
                    key={tag.id}
                    variant={
                      field.value.includes(tag.id) ? "default" : "outline"
                    }
                    className="cursor-pointer select-none transition-colors"
                    onClick={() => toggleId("tagIds", tag.id)}
                  >
                    #{tag.name}
                  </Badge>
                ))}
              </div>
              <FormMessage />
            </FormItem>
          )}
        />

        {/* Content — Markdown */}
        <FormField
          control={form.control}
          name="content"
          rules={{ required: "Content is required" }}
          render={({ field }) => (
            <FormItem>
              <FormLabel>Content (Markdown) *</FormLabel>
              <FormControl>
                <Textarea
                  placeholder="Write your post in Markdown…"
                  rows={20}
                  className="font-mono text-sm resize-y"
                  {...field}
                />
              </FormControl>
              <FormDescription>
                Supports GitHub Flavored Markdown (GFM).
              </FormDescription>
              <FormMessage />
            </FormItem>
          )}
        />

        <Button type="submit" disabled={isSubmitting} className="gap-2">
          {isSubmitting && <Loader2 className="h-4 w-4 animate-spin" />}
          {submitLabel}
        </Button>
      </form>
    </Form>
  );
}
