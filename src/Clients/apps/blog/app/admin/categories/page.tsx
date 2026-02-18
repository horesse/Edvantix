"use client";

import { useState } from "react";

import { Edit, Loader2, Plus, Trash2 } from "lucide-react";
import { toast } from "sonner";

import { Button } from "@workspace/ui/components/button";
import {
  Dialog,
  DialogContent,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@workspace/ui/components/dialog";
import { Input } from "@workspace/ui/components/input";
import { Label } from "@workspace/ui/components/label";
import { Skeleton } from "@workspace/ui/components/skeleton";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@workspace/ui/components/table";
import { Textarea } from "@workspace/ui/components/textarea";
import useGetCategories from "@workspace/api-hooks/blog/useGetCategories";
import useCreateCategory from "@workspace/api-hooks/blog/useCreateCategory";
import useUpdateCategory from "@workspace/api-hooks/blog/useUpdateCategory";
import useDeleteCategory from "@workspace/api-hooks/blog/useDeleteCategory";
import type { CategoryModel } from "@workspace/types/blog";

import { slugify } from "@/lib/utils";

type CategoryFormState = {
  name: string;
  slug: string;
  description: string;
};

const defaultForm: CategoryFormState = { name: "", slug: "", description: "" };

export default function AdminCategoriesPage() {
  const { data: categories = [], isLoading } = useGetCategories();
  const { mutate: createCategory, isPending: isCreating } = useCreateCategory();
  const { mutate: updateCategory, isPending: isUpdating } = useUpdateCategory();
  const { mutate: deleteCategory, isPending: isDeleting } = useDeleteCategory();

  const [createOpen, setCreateOpen] = useState(false);
  const [editTarget, setEditTarget] = useState<CategoryModel | null>(null);
  const [form, setForm] = useState<CategoryFormState>(defaultForm);

  const handleNameChange = (name: string) => {
    setForm((prev) => ({
      ...prev,
      name,
      slug: prev.slug === slugify(prev.name) ? slugify(name) : prev.slug,
    }));
  };

  const openCreate = () => {
    setForm(defaultForm);
    setCreateOpen(true);
  };

  const openEdit = (cat: CategoryModel) => {
    setForm({ name: cat.name, slug: cat.slug, description: cat.description ?? "" });
    setEditTarget(cat);
  };

  const handleCreate = () => {
    createCategory(
      { name: form.name, slug: form.slug, description: form.description || undefined },
      {
        onSuccess: () => {
          toast.success("Category created");
          setCreateOpen(false);
          setForm(defaultForm);
        },
        onError: () => toast.error("Failed to create category"),
      },
    );
  };

  const handleUpdate = () => {
    if (!editTarget) return;
    updateCategory(
      {
        id: editTarget.id,
        request: { name: form.name, slug: form.slug, description: form.description || undefined },
      },
      {
        onSuccess: () => {
          toast.success("Category updated");
          setEditTarget(null);
        },
        onError: () => toast.error("Failed to update category"),
      },
    );
  };

  const handleDelete = (cat: CategoryModel) => {
    if (!confirm(`Delete category "${cat.name}"? This cannot be undone.`)) return;
    deleteCategory(cat.id, {
      onSuccess: () => toast.success("Category deleted"),
      onError: () => toast.error("Failed to delete category"),
    });
  };

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <div>
          <h1 className="text-2xl font-bold tracking-tight">Categories</h1>
          <p className="text-muted-foreground mt-1">
            Organize posts into categories.
          </p>
        </div>

        <Dialog open={createOpen} onOpenChange={setCreateOpen}>
          <DialogTrigger asChild>
            <Button size="sm" className="gap-2" onClick={openCreate}>
              <Plus className="h-4 w-4" />
              New Category
            </Button>
          </DialogTrigger>
          <DialogContent>
            <DialogHeader>
              <DialogTitle>New Category</DialogTitle>
            </DialogHeader>
            <CategoryForm form={form} onNameChange={handleNameChange} onChange={setForm} />
            <DialogFooter>
              <Button
                onClick={handleCreate}
                disabled={isCreating || !form.name || !form.slug}
              >
                {isCreating && <Loader2 className="h-4 w-4 animate-spin mr-2" />}
                Create
              </Button>
            </DialogFooter>
          </DialogContent>
        </Dialog>
      </div>

      {isLoading ? (
        <div className="space-y-3">
          {Array.from({ length: 5 }).map((_, i) => (
            <Skeleton key={i} className="h-14 w-full rounded-md" />
          ))}
        </div>
      ) : (
        <div className="rounded-xl border border-border overflow-hidden">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Name</TableHead>
                <TableHead className="hidden sm:table-cell">Slug</TableHead>
                <TableHead className="hidden md:table-cell">Description</TableHead>
                <TableHead className="w-24" />
              </TableRow>
            </TableHeader>
            <TableBody>
              {categories.length === 0 ? (
                <TableRow>
                  <TableCell
                    colSpan={4}
                    className="text-center text-muted-foreground py-10"
                  >
                    No categories yet. Create one to get started.
                  </TableCell>
                </TableRow>
              ) : (
                categories.map((cat) => (
                  <TableRow key={cat.id}>
                    <TableCell className="font-medium">{cat.name}</TableCell>
                    <TableCell className="hidden sm:table-cell font-mono text-xs text-muted-foreground">
                      /{cat.slug}
                    </TableCell>
                    <TableCell className="hidden md:table-cell text-sm text-muted-foreground line-clamp-1">
                      {cat.description ?? "—"}
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center gap-1 justify-end">
                        <Dialog
                          open={editTarget?.id === cat.id}
                          onOpenChange={(open) => !open && setEditTarget(null)}
                        >
                          <DialogTrigger asChild>
                            <Button
                              variant="ghost"
                              size="icon"
                              className="h-8 w-8"
                              onClick={() => openEdit(cat)}
                            >
                              <Edit className="h-4 w-4" />
                            </Button>
                          </DialogTrigger>
                          <DialogContent>
                            <DialogHeader>
                              <DialogTitle>Edit Category</DialogTitle>
                            </DialogHeader>
                            <CategoryForm
                              form={form}
                              onNameChange={handleNameChange}
                              onChange={setForm}
                            />
                            <DialogFooter>
                              <Button
                                onClick={handleUpdate}
                                disabled={isUpdating || !form.name || !form.slug}
                              >
                                {isUpdating && (
                                  <Loader2 className="h-4 w-4 animate-spin mr-2" />
                                )}
                                Save changes
                              </Button>
                            </DialogFooter>
                          </DialogContent>
                        </Dialog>

                        <Button
                          variant="ghost"
                          size="icon"
                          className="h-8 w-8 text-destructive hover:text-destructive"
                          onClick={() => handleDelete(cat)}
                          disabled={isDeleting}
                        >
                          <Trash2 className="h-4 w-4" />
                        </Button>
                      </div>
                    </TableCell>
                  </TableRow>
                ))
              )}
            </TableBody>
          </Table>
        </div>
      )}
    </div>
  );
}

function CategoryForm({
  form,
  onNameChange,
  onChange,
}: {
  form: CategoryFormState;
  onNameChange: (name: string) => void;
  onChange: (form: CategoryFormState) => void;
}) {
  return (
    <div className="space-y-4 py-2">
      <div className="space-y-1.5">
        <Label htmlFor="cat-name">Name *</Label>
        <Input
          id="cat-name"
          placeholder="Engineering"
          value={form.name}
          onChange={(e) => onNameChange(e.target.value)}
        />
      </div>
      <div className="space-y-1.5">
        <Label htmlFor="cat-slug">Slug *</Label>
        <Input
          id="cat-slug"
          placeholder="engineering"
          value={form.slug}
          onChange={(e) =>
            onChange({ ...form, slug: e.target.value })
          }
        />
      </div>
      <div className="space-y-1.5">
        <Label htmlFor="cat-desc">Description</Label>
        <Textarea
          id="cat-desc"
          placeholder="Brief description…"
          rows={3}
          value={form.description}
          onChange={(e) =>
            onChange({ ...form, description: e.target.value })
          }
        />
      </div>
    </div>
  );
}
