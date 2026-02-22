"use client";

import { useState } from "react";

import { Edit, Loader2, Plus, Tag, Trash2 } from "lucide-react";
import { toast } from "sonner";

import useCreateTag from "@workspace/api-hooks/blog/useCreateTag";
import useDeleteTag from "@workspace/api-hooks/blog/useDeleteTag";
import useGetTags from "@workspace/api-hooks/blog/useGetTags";
import useUpdateTag from "@workspace/api-hooks/blog/useUpdateTag";
import type { TagModel } from "@workspace/types/blog";
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
  const [deleteTarget, setDeleteTarget] = useState<TagModel | null>(null);
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

  const handleDeleteConfirm = () => {
    if (!deleteTarget) return;
    deleteTag(deleteTarget.id, {
      onSuccess: () => {
        toast.success("Tag deleted");
        setDeleteTarget(null);
      },
      onError: () => toast.error("Failed to delete tag"),
    });
  };

  return (
    <div>
      {/* ── Page header ── */}
      <div className="mb-6 flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold tracking-tight">Tags</h1>
          <p className="text-muted-foreground mt-1 text-sm">
            Label posts for better discoverability.
            {!isLoading && (
              <span className="bg-muted ml-2 rounded-full px-2 py-0.5 font-mono text-xs">
                {tags.length}
              </span>
            )}
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
            <TagForm
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

      {/* ── Tag cloud ── */}
      {isLoading ? (
        <div className="flex flex-wrap gap-3">
          {Array.from({ length: 10 }).map((_, i) => (
            <Skeleton key={i} className="h-10 w-24 rounded-full" />
          ))}
        </div>
      ) : tags.length === 0 ? (
        <div className="border-border rounded-xl border py-16 text-center">
          <div className="flex flex-col items-center gap-2">
            <div className="bg-muted flex h-12 w-12 items-center justify-center rounded-full">
              <Tag className="text-muted-foreground h-5 w-5" />
            </div>
            <p className="text-muted-foreground text-sm">
              No tags yet. Create one to get started.
            </p>
          </div>
        </div>
      ) : (
        <div className="flex flex-wrap gap-2.5">
          {tags.map((tag) => (
            <div
              key={tag.id}
              className="group border-border bg-card hover:border-primary/30 hover:bg-primary/5 flex items-center gap-1.5 rounded-full border px-3.5 py-2 text-sm transition-colors"
            >
              <span className="text-primary/60 font-medium">#</span>
              <span className="text-foreground font-medium">{tag.name}</span>
              {/* Action buttons — always visible, not hover-only */}
              <div className="border-border ml-1.5 flex items-center gap-0.5 border-l pl-1.5">
                <Dialog
                  open={editTarget?.id === tag.id}
                  onOpenChange={(open) => !open && setEditTarget(null)}
                >
                  <DialogTrigger asChild>
                    <button
                      className="text-muted-foreground/60 hover:text-foreground flex h-5 w-5 items-center justify-center rounded transition-colors"
                      onClick={() => openEdit(tag)}
                      title="Edit tag"
                    >
                      <Edit className="h-3 w-3" />
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
                          <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                        )}
                        Save changes
                      </Button>
                    </DialogFooter>
                  </DialogContent>
                </Dialog>
                <button
                  className="text-destructive/50 hover:text-destructive flex h-5 w-5 items-center justify-center rounded transition-colors"
                  onClick={() => setDeleteTarget(tag)}
                  disabled={isDeleting}
                  title="Delete tag"
                >
                  <Trash2 className="h-3 w-3" />
                </button>
              </div>
            </div>
          ))}
        </div>
      )}

      {tags.length > 0 && (
        <p className="text-muted-foreground mt-5 text-xs">
          {tags.length} tag{tags.length !== 1 ? "s" : ""} total
        </p>
      )}

      {/* Delete confirmation */}
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
          className="font-mono text-sm"
          value={form.slug}
          onChange={(e) => onChange({ ...form, slug: e.target.value })}
        />
      </div>
    </div>
  );
}
