"use client";

import { useState } from "react";

import { Plus } from "lucide-react";
import { toast } from "sonner";

import useCreateCategory from "@workspace/api-hooks/blog/useCreateCategory";
import useDeleteCategory from "@workspace/api-hooks/blog/useDeleteCategory";
import useGetCategories from "@workspace/api-hooks/blog/useGetCategories";
import useUpdateCategory from "@workspace/api-hooks/blog/useUpdateCategory";
import type {
  CategoryModel,
  CreateCategoryRequest,
  UpdateCategoryRequest,
} from "@workspace/types/blog";
import { Button } from "@workspace/ui/components/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@workspace/ui/components/dialog";

import { useCrudPage } from "@/hooks/use-crud-page";
import { slugify } from "@/lib/utils";

import { CategoryDialog } from "./dialog";
import { CategoriesTable } from "./table/table";

type CategoryFormState = {
  name: string;
  slug: string;
  description: string;
};

const defaultForm: CategoryFormState = { name: "", slug: "", description: "" };

export function AdminCategoriesSection() {
  const listQuery = useGetCategories();
  const createMutation = useCreateCategory();
  const updateMutation = useUpdateCategory();
  const deleteMutation = useDeleteCategory();

  const [form, setForm] = useState<CategoryFormState>(defaultForm);
  const [editTarget, setEditTarget] = useState<CategoryModel | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<CategoryModel | null>(null);

  const crud = useCrudPage<
    CategoryModel,
    string,
    void,
    CreateCategoryRequest,
    { id: string; request: UpdateCategoryRequest }
  >({
    entityName: "Category",
    listQuery,
    createMutation,
    updateMutation,
    deleteMutation,
    buildCreateRequest: (name) => ({
      name,
      slug: form.slug,
      description: form.description || undefined,
    }),
    buildUpdateRequest: (id, name) => ({
      id,
      request: {
        name,
        slug: form.slug,
        description: form.description || undefined,
      },
    }),
  });

  const handleNameChange = (name: string) => {
    setForm((prev) => ({
      ...prev,
      name,
      slug: prev.slug === slugify(prev.name) ? slugify(name) : prev.slug,
    }));
  };

  const openCreate = () => {
    setForm(defaultForm);
    crud.setIsDialogOpen(true);
  };

  const openEdit = (item: CategoryModel) => {
    setForm({
      name: item.name,
      slug: item.slug,
      description: item.description ?? "",
    });
    setEditTarget(item);
  };

  const handleCreate = async () => {
    try {
      await crud.handleCreate(form.name);
      setForm(defaultForm);
    } catch {
      toast.error("Failed to create category");
    }
  };

  const handleUpdate = async () => {
    if (!editTarget) return;

    try {
      await crud.handleUpdate(editTarget.id, form.name);
      setEditTarget(null);
    } catch {
      toast.error("Failed to update category");
    }
  };

  const handleDeleteConfirm = async () => {
    if (!deleteTarget) return;

    try {
      await crud.handleDelete(deleteTarget.id);
      setDeleteTarget(null);
    } catch {
      toast.error("Failed to delete category");
    }
  };

  return (
    <div>
      <div className="mb-6 flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold tracking-tight">Categories</h1>
          <p className="text-muted-foreground mt-1 text-sm">
            Organize posts into categories.
            {!crud.isLoading && (
              <span className="bg-muted ml-2 rounded-full px-2 py-0.5 font-mono text-xs">
                {crud.items.length}
              </span>
            )}
          </p>
        </div>

        <Button size="sm" className="gap-2" onClick={openCreate}>
          <Plus className="h-4 w-4" />
          New Category
        </Button>
      </div>

      <CategoriesTable
        items={crud.items}
        isLoading={crud.isLoading}
        isSubmitting={crud.isSubmitting}
        onEdit={openEdit}
        onDelete={setDeleteTarget}
      />

      <CategoryDialog
        open={crud.isDialogOpen}
        onOpenChange={crud.setIsDialogOpen}
        title="New Category"
        submitLabel="Create"
        form={form}
        isSubmitting={crud.isCreatePending}
        onNameChange={handleNameChange}
        onChange={setForm}
        onSubmit={handleCreate}
      />

      <CategoryDialog
        open={editTarget !== null}
        onOpenChange={(open) => !open && setEditTarget(null)}
        title="Edit Category"
        submitLabel="Save changes"
        form={form}
        isSubmitting={crud.isSubmitting}
        onNameChange={handleNameChange}
        onChange={setForm}
        onSubmit={handleUpdate}
      />

      <Dialog
        open={deleteTarget !== null}
        onOpenChange={(open) => !open && setDeleteTarget(null)}
      >
        <DialogContent className="sm:max-w-sm">
          <DialogHeader>
            <DialogTitle>Delete category</DialogTitle>
            <DialogDescription>
              Delete <span className="font-medium">&quot;{deleteTarget?.name}&quot;</span>?
              This cannot be undone.
            </DialogDescription>
          </DialogHeader>
          <DialogFooter>
            <Button variant="outline" onClick={() => setDeleteTarget(null)}>
              Cancel
            </Button>
            <Button
              variant="destructive"
              onClick={handleDeleteConfirm}
              disabled={crud.isSubmitting}
              className="gap-2"
            >
              Delete
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
