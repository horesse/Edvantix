"use client";

import { useState } from "react";

import { Edit, Hash, Loader2, Plus, Trash2 } from "lucide-react";
import { toast } from "sonner";

import useCreateCategory from "@workspace/api-hooks/blog/useCreateCategory";
import useDeleteCategory from "@workspace/api-hooks/blog/useDeleteCategory";
import useGetCategories from "@workspace/api-hooks/blog/useGetCategories";
import useUpdateCategory from "@workspace/api-hooks/blog/useUpdateCategory";
import type { CategoryModel } from "@workspace/types/blog";
import { Button } from "@workspace/ui/components/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
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
  const [deleteTarget, setDeleteTarget] = useState<CategoryModel | null>(null);
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
    setForm({
      name: cat.name,
      slug: cat.slug,
      description: cat.description ?? "",
    });
    setEditTarget(cat);
  };

  const handleCreate = () => {
    createCategory(
      {
        name: form.name,
        slug: form.slug,
        description: form.description || undefined,
      },
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
        request: {
          name: form.name,
          slug: form.slug,
          description: form.description || undefined,
        },
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

  const handleDeleteConfirm = () => {
    if (!deleteTarget) return;
    deleteCategory(deleteTarget.id, {
      onSuccess: () => {
        toast.success("Category deleted");
        setDeleteTarget(null);
      },
      onError: () => toast.error("Failed to delete category"),
    });
  };

  return (
    <div>
      {/* ── Page header ── */}
      <div className="mb-6 flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold tracking-tight">Categories</h1>
          <p className="text-muted-foreground mt-1 text-sm">
            Organize posts into categories.
            {!isLoading && (
              <span className="bg-muted ml-2 rounded-full px-2 py-0.5 font-mono text-xs">
                {categories.length}
              </span>
            )}
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
            <CategoryForm
              form={form}
              onNameChange={handleNameChange}
              onChange={setForm}
            />
            <DialogFooter>
              <Button
                onClick={handleCreate}
                disabled={isCreating || !form.name || !form.slug}
              >
                {isCreating && (
                  <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                )}
                Create
              </Button>
            </DialogFooter>
          </DialogContent>
        </Dialog>
      </div>

      {/* ── Table ── */}
      {isLoading ? (
        <div className="space-y-2">
          {Array.from({ length: 5 }).map((_, i) => (
            <Skeleton key={i} className="h-14 w-full rounded-lg" />
          ))}
        </div>
      ) : (
        <div className="border-border overflow-hidden rounded-xl border">
          <Table>
            <TableHeader>
              <TableRow className="bg-muted/40">
                <TableHead className="font-semibold">Name</TableHead>
                <TableHead className="hidden font-semibold sm:table-cell">
                  Slug
                </TableHead>
                <TableHead className="hidden font-semibold md:table-cell">
                  Description
                </TableHead>
                <TableHead className="w-24" />
              </TableRow>
            </TableHeader>
            <TableBody>
              {categories.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={4} className="py-16 text-center">
                    <div className="flex flex-col items-center gap-2">
                      <div className="bg-muted flex h-12 w-12 items-center justify-center rounded-full">
                        <Hash className="text-muted-foreground h-5 w-5" />
                      </div>
                      <p className="text-muted-foreground text-sm">
                        No categories yet. Create one to get started.
                      </p>
                    </div>
                  </TableCell>
                </TableRow>
              ) : (
                categories.map((cat) => (
                  <TableRow
                    key={cat.id}
                    className="hover:bg-muted/30 transition-colors"
                  >
                    <TableCell>
                      <div className="flex items-center gap-2.5">
                        <div className="bg-primary/10 flex h-7 w-7 shrink-0 items-center justify-center rounded-lg">
                          <Hash className="text-primary h-3.5 w-3.5" />
                        </div>
                        <span className="font-medium">{cat.name}</span>
                      </div>
                    </TableCell>
                    <TableCell className="hidden sm:table-cell">
                      <code className="bg-muted text-muted-foreground rounded px-1.5 py-0.5 font-mono text-xs">
                        /{cat.slug}
                      </code>
                    </TableCell>
                    <TableCell className="text-muted-foreground hidden text-sm md:table-cell">
                      <span className="line-clamp-1">
                        {cat.description ?? "—"}
                      </span>
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center justify-end gap-1">
                        <Dialog
                          open={editTarget?.id === cat.id}
                          onOpenChange={(open) => !open && setEditTarget(null)}
                        >
                          <DialogTrigger asChild>
                            <Button
                              variant="ghost"
                              size="icon"
                              className="h-8 w-8 rounded-lg"
                              onClick={() => openEdit(cat)}
                            >
                              <Edit className="h-3.5 w-3.5" />
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
                                disabled={
                                  isUpdating || !form.name || !form.slug
                                }
                              >
                                {isUpdating && (
                                  <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                                )}
                                Save changes
                              </Button>
                            </DialogFooter>
                          </DialogContent>
                        </Dialog>

                        <Button
                          variant="ghost"
                          size="icon"
                          className="text-destructive/60 hover:text-destructive hover:bg-destructive/10 h-8 w-8 rounded-lg"
                          onClick={() => setDeleteTarget(cat)}
                          disabled={isDeleting}
                        >
                          <Trash2 className="h-3.5 w-3.5" />
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

      {/* Delete confirmation */}
      <Dialog
        open={deleteTarget !== null}
        onOpenChange={(open) => !open && setDeleteTarget(null)}
      >
        <DialogContent className="sm:max-w-sm">
          <DialogHeader>
            <DialogTitle>Delete category</DialogTitle>
            <DialogDescription>
              Delete <span className="font-medium">"{deleteTarget?.name}"</span>
              ? This cannot be undone.
            </DialogDescription>
          </DialogHeader>
          <DialogFooter>
            <Button variant="outline" onClick={() => setDeleteTarget(null)}>
              Cancel
            </Button>
            <Button
              variant="destructive"
              onClick={handleDeleteConfirm}
              disabled={isDeleting}
              className="gap-2"
            >
              {isDeleting && <Loader2 className="h-4 w-4 animate-spin" />}
              Delete
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
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
        <div className="flex items-center gap-2">
          <span className="text-muted-foreground text-sm">/</span>
          <Input
            id="cat-slug"
            placeholder="engineering"
            className="font-mono text-sm"
            value={form.slug}
            onChange={(e) => onChange({ ...form, slug: e.target.value })}
          />
        </div>
      </div>
      <div className="space-y-1.5">
        <Label htmlFor="cat-desc">Description</Label>
        <Textarea
          id="cat-desc"
          placeholder="Brief description…"
          rows={3}
          value={form.description}
          onChange={(e) => onChange({ ...form, description: e.target.value })}
        />
      </div>
    </div>
  );
}
