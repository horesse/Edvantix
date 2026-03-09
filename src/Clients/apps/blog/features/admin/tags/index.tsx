"use client";

import { useState } from "react";

import { Plus } from "lucide-react";
import { toast } from "sonner";

import useCreateTag from "@workspace/api-hooks/blog/useCreateTag";
import useDeleteTag from "@workspace/api-hooks/blog/useDeleteTag";
import useGetTags from "@workspace/api-hooks/blog/useGetTags";
import useUpdateTag from "@workspace/api-hooks/blog/useUpdateTag";
import type {
  CreateTagRequest,
  TagModel,
  UpdateTagRequest,
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

import { TagDialog } from "./dialog";
import { TagsTable } from "./table/table";

type TagFormState = {
  name: string;
  slug: string;
};

const defaultForm: TagFormState = { name: "", slug: "" };

export function AdminTagsSection() {
  const listQuery = useGetTags();
  const createMutation = useCreateTag();
  const updateMutation = useUpdateTag();
  const deleteMutation = useDeleteTag();

  const [form, setForm] = useState<TagFormState>(defaultForm);
  const [editTarget, setEditTarget] = useState<TagModel | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<TagModel | null>(null);

  const crud = useCrudPage<
    TagModel,
    string,
    void,
    CreateTagRequest,
    { id: string; request: UpdateTagRequest }
  >({
    entityName: "Tag",
    listQuery,
    createMutation,
    updateMutation,
    deleteMutation,
    buildCreateRequest: (name) => ({ name, slug: form.slug }),
    buildUpdateRequest: (id, name) => ({
      id,
      request: { name, slug: form.slug },
    }),
  });

  const handleNameChange = (name: string) => {
    setForm((prev) => ({
      name,
      slug: prev.slug === slugify(prev.name) ? slugify(name) : prev.slug,
    }));
  };

  const openCreate = () => {
    setForm(defaultForm);
    crud.setIsDialogOpen(true);
  };

  const openEdit = (item: TagModel) => {
    setForm({ name: item.name, slug: item.slug });
    setEditTarget(item);
  };

  const handleCreate = async () => {
    try {
      await crud.handleCreate(form.name);
      setForm(defaultForm);
    } catch {
      toast.error("Failed to create tag");
    }
  };

  const handleUpdate = async () => {
    if (!editTarget) return;

    try {
      await crud.handleUpdate(editTarget.id, form.name);
      setEditTarget(null);
    } catch {
      toast.error("Failed to update tag");
    }
  };

  const handleDeleteConfirm = async () => {
    if (!deleteTarget) return;

    try {
      await crud.handleDelete(deleteTarget.id);
      setDeleteTarget(null);
    } catch {
      toast.error("Failed to delete tag");
    }
  };

  return (
    <div>
      <div className="mb-6 flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold tracking-tight">Tags</h1>
          <p className="text-muted-foreground mt-1 text-sm">
            Label posts for better discoverability.
            {!crud.isLoading && (
              <span className="bg-muted ml-2 rounded-full px-2 py-0.5 font-mono text-xs">
                {crud.items.length}
              </span>
            )}
          </p>
        </div>

        <Button size="sm" className="gap-2" onClick={openCreate}>
          <Plus className="h-4 w-4" />
          New Tag
        </Button>
      </div>

      <TagsTable
        items={crud.items}
        isLoading={crud.isLoading}
        isSubmitting={crud.isSubmitting}
        onEdit={openEdit}
        onDelete={setDeleteTarget}
      />

      <TagDialog
        open={crud.isDialogOpen}
        onOpenChange={crud.setIsDialogOpen}
        title="New Tag"
        submitLabel="Create"
        form={form}
        isSubmitting={crud.isCreatePending}
        onNameChange={handleNameChange}
        onChange={setForm}
        onSubmit={handleCreate}
      />

      <TagDialog
        open={editTarget !== null}
        onOpenChange={(open) => !open && setEditTarget(null)}
        title="Edit Tag"
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
            <DialogTitle>Delete tag</DialogTitle>
            <DialogDescription>
              Delete tag{" "}
              <span className="font-medium">#{deleteTarget?.name}</span>? This
              cannot be undone.
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
