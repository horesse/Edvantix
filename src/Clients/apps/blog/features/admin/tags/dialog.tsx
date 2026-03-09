import { Loader2 } from "lucide-react";

import { Button } from "@workspace/ui/components/button";
import {
  Dialog,
  DialogContent,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@workspace/ui/components/dialog";
import { Input } from "@workspace/ui/components/input";
import { Label } from "@workspace/ui/components/label";

type TagFormState = {
  name: string;
  slug: string;
};

type TagDialogProps = Readonly<{
  open: boolean;
  title: string;
  submitLabel: string;
  form: TagFormState;
  isSubmitting: boolean;
  onOpenChange: (open: boolean) => void;
  onNameChange: (name: string) => void;
  onChange: (form: TagFormState) => void;
  onSubmit: () => void;
}>;

export function TagDialog({
  open,
  title,
  submitLabel,
  form,
  isSubmitting,
  onOpenChange,
  onNameChange,
  onChange,
  onSubmit,
}: TagDialogProps) {
  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>{title}</DialogTitle>
        </DialogHeader>
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
        <DialogFooter>
          <Button
            variant="outline"
            onClick={() => onOpenChange(false)}
            disabled={isSubmitting}
          >
            Cancel
          </Button>
          <Button
            onClick={onSubmit}
            disabled={isSubmitting || !form.name || !form.slug}
          >
            {isSubmitting && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
            {submitLabel}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
