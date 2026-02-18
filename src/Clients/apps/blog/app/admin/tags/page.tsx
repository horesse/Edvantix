"use client";

import { useState } from "react";

import { Edit, Loader2, Plus, Trash2 } from "lucide-react";
import { toast } from "sonner";

import { Badge } from "@workspace/ui/components/badge";
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
import useGetTags from "@workspace/api-hooks/blog/useGetTags";
import useCreateTag from "@workspace/api-hooks/blog/useCreateTag";
import useUpdateTag from "@workspace/api-hooks/blog/useUpdateTag";
import useDeleteTag from "@workspace/api-hooks/blog/useDeleteTag";
import type { TagModel } from "@workspace/types/blog";

import { slugify } from "@/lib/utils";

type TagFormState = {
  name: string;
  slug: string;
};

const defaultForm: TagFormState = { name: "", slug: "" };

export default function AdminTagsPage() {
  const { data: tags = [], isLoading } = useGetTags();
  const { mutate: createTag, isPending: isCreating } = useCreateTag();
  const { mutate: updateTag, isPending: isUpdating } = useUpdateTag();
  const { mutate: deleteTag, isPending: isDeleting } = useDeleteTag();

  const [createOpen, setCreateOpen] = useState(false);
  const [editTarget, setEditTarget] = useState<TagModel | null>(null);
  const [form, setForm] = useState<TagFormState>(defaultForm);

  const handleNameChange = (name: string) => {
    setForm((prev) => ({
      name,
      slug: prev.slug === slugify(prev.name) ? slugify(name) : prev.slug,
    }));
  };

  const openCreate = () => {
    setForm(defaultForm);
    setCreateOpen(true);
  };

  const openEdit = (tag: TagModel) => {
    setForm({ name: tag.name, slug: tag.slug });
    setEditTarget(tag);
  };

  const handleCreate = () => {
    createTag(
      { name: form.name, slug: form.slug },
      {
        onSuccess: () => {
          toast.success("Tag created");
          setCreateOpen(false);
          setForm(defaultForm);
        },
        onError: () => toast.error("Failed to create tag"),
      },
    );
  };

  const handleUpdate = () => {
    if (!editTarget) return;
    updateTag(
      { id: editTarget.id, request: { name: form.name, slug: form.slug } },
      {
        onSuccess: () => {
          toast.success("Tag updated");
          setEditTarget(null);
        },
        onError: () => toast.error("Failed to update tag"),
      },
    );
  };

  const handleDelete = (tag: TagModel) => {
    if (!confirm(`Delete tag "#${tag.name}"? This cannot be undone.`)) return;
    deleteTag(tag.id, {
      onSuccess: () => toast.success("Tag deleted"),
      onError: () => toast.error("Failed to delete tag"),
    });
  };

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <div>
          <h1 className="text-2xl font-bold tracking-tight">Tags</h1>
          <p className="text-muted-foreground mt-1">
            Label posts for better discoverability.
          </p>
        </div>

        <Dialog open={createOpen} onOpenChange={setCreateOpen}>
          <DialogTrigger asChild>
            <Button size="sm" className="gap-2" onClick={openCreate}>
              <Plus className="h-4 w-4" />
              New Tag
            </Button>
          </DialogTrigger>
          <DialogContent>
            <DialogHeader>
              <DialogTitle>New Tag</DialogTitle>
            </DialogHeader>
            <TagForm form={form} onNameChange={handleNameChange} onChange={setForm} />
            <DialogFooter>
              <Button
                onClick={handleCreate}
                disabled={isCreating || !form.name || !form.slug}
              >
                {isCreating && (
                  <Loader2 className="h-4 w-4 animate-spin mr-2" />
                )}
                Create
              </Button>
            </DialogFooter>
          </DialogContent>
        </Dialog>
      </div>

      {isLoading ? (
        <div className="flex flex-wrap gap-3">
          {Array.from({ length: 10 }).map((_, i) => (
            <Skeleton key={i} className="h-8 w-20 rounded-full" />
          ))}
        </div>
      ) : tags.length === 0 ? (
        <div className="rounded-xl border border-border py-16 text-center text-muted-foreground">
          No tags yet. Create one to get started.
        </div>
      ) : (
        <div className="flex flex-wrap gap-3">
          {tags.map((tag) => (
            <div
              key={tag.id}
              className="group flex items-center gap-1 rounded-full border border-border bg-card px-3 py-1.5 text-sm"
            >
              <span className="text-muted-foreground mr-0.5">#</span>
              <span className="font-medium">{tag.name}</span>
              <span className="ml-2 hidden group-hover:flex items-center gap-1">
                <Dialog
                  open={editTarget?.id === tag.id}
                  onOpenChange={(open) => !open && setEditTarget(null)}
                >
                  <DialogTrigger asChild>
                    <button
                      className="text-muted-foreground hover:text-foreground transition-colors"
                      onClick={() => openEdit(tag)}
                    >
                      <Edit className="h-3.5 w-3.5" />
                    </button>
                  </DialogTrigger>
                  <DialogContent>
                    <DialogHeader>
                      <DialogTitle>Edit Tag</DialogTitle>
                    </DialogHeader>
                    <TagForm
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
                <button
                  className="text-destructive/70 hover:text-destructive transition-colors"
                  onClick={() => handleDelete(tag)}
                  disabled={isDeleting}
                >
                  <Trash2 className="h-3.5 w-3.5" />
                </button>
              </span>
            </div>
          ))}
        </div>
      )}

      {tags.length > 0 && (
        <p className="mt-4 text-xs text-muted-foreground">
          {tags.length} tag{tags.length !== 1 ? "s" : ""} total. Hover a tag to edit or delete.
        </p>
      )}
    </div>
  );
}

function TagForm({
  form,
  onNameChange,
  onChange,
}: {
  form: TagFormState;
  onNameChange: (name: string) => void;
  onChange: (form: TagFormState) => void;
}) {
  return (
    <div className="space-y-4 py-2">
      <div className="space-y-1.5">
        <Label htmlFor="tag-name">Name *</Label>
        <Input
          id="tag-name"
          placeholder="devops"
          value={form.name}
          onChange={(e) => onNameChange(e.target.value)}
        />
      </div>
      <div className="space-y-1.5">
        <Label htmlFor="tag-slug">Slug *</Label>
        <Input
          id="tag-slug"
          placeholder="devops"
          value={form.slug}
          onChange={(e) => onChange({ ...form, slug: e.target.value })}
        />
      </div>
    </div>
  );
}
