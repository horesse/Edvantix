"use client";

import Link from "next/link";

import { FileText, Hash, Plus, Tag } from "lucide-react";

import { Button } from "@workspace/ui/components/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@workspace/ui/components/card";
import useGetPosts from "@workspace/api-hooks/blog/useGetPosts";
import useGetCategories from "@workspace/api-hooks/blog/useGetCategories";
import useGetTags from "@workspace/api-hooks/blog/useGetTags";

export default function AdminDashboardPage() {
  const { data: posts } = useGetPosts({ pageSize: 100 });
  const { data: categories } = useGetCategories();
  const { data: tags } = useGetTags();

  return (
    <div className="max-w-4xl">
      <div className="flex items-center justify-between mb-8">
        <div>
          <h1 className="text-2xl font-bold tracking-tight">Dashboard</h1>
          <p className="text-muted-foreground mt-1">
            Manage your blog content.
          </p>
        </div>
        <Button asChild>
          <Link href="/admin/posts/new" className="gap-2">
            <Plus className="h-4 w-4" />
            New post
          </Link>
        </Button>
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-3 gap-4 mb-8">
        <Card>
          <CardHeader className="pb-2">
            <CardDescription className="flex items-center gap-2">
              <FileText className="h-4 w-4" />
              Published Posts
            </CardDescription>
            <CardTitle className="text-3xl font-bold">
              {posts?.totalItems ?? "—"}
            </CardTitle>
          </CardHeader>
          <CardContent>
            <Link
              href="/admin/posts"
              className="text-sm text-primary hover:underline"
            >
              View all →
            </Link>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="pb-2">
            <CardDescription className="flex items-center gap-2">
              <Hash className="h-4 w-4" />
              Categories
            </CardDescription>
            <CardTitle className="text-3xl font-bold">
              {categories?.length ?? "—"}
            </CardTitle>
          </CardHeader>
          <CardContent>
            <Link
              href="/admin/categories"
              className="text-sm text-primary hover:underline"
            >
              Manage →
            </Link>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="pb-2">
            <CardDescription className="flex items-center gap-2">
              <Tag className="h-4 w-4" />
              Tags
            </CardDescription>
            <CardTitle className="text-3xl font-bold">
              {tags?.length ?? "—"}
            </CardTitle>
          </CardHeader>
          <CardContent>
            <Link
              href="/admin/tags"
              className="text-sm text-primary hover:underline"
            >
              Manage →
            </Link>
          </CardContent>
        </Card>
      </div>

      {/* Quick actions */}
      <Card>
        <CardHeader>
          <CardTitle className="text-base">Quick Actions</CardTitle>
        </CardHeader>
        <CardContent className="flex flex-wrap gap-3">
          <Button asChild variant="outline" size="sm">
            <Link href="/admin/posts/new" className="gap-2">
              <Plus className="h-4 w-4" />
              New Post
            </Link>
          </Button>
          <Button asChild variant="outline" size="sm">
            <Link href="/admin/categories" className="gap-2">
              <Hash className="h-4 w-4" />
              Add Category
            </Link>
          </Button>
          <Button asChild variant="outline" size="sm">
            <Link href="/admin/tags" className="gap-2">
              <Tag className="h-4 w-4" />
              Add Tag
            </Link>
          </Button>
        </CardContent>
      </Card>
    </div>
  );
}
