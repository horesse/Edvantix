import { Edit, Trash2 } from "lucide-react";

import { Button } from "@workspace/ui/components/button";

import type { TagRow } from "./columns";

type TagCellActionProps = Readonly<{
  item: TagRow;
  onEdit: (item: TagRow) => void;
  onDelete: (item: TagRow) => void;
  isSubmitting: boolean;
}>;

export function TagCellAction({
  item,
  onEdit,
  onDelete,
  isSubmitting,
}: TagCellActionProps) {
  return (
    <div className="flex items-center justify-end gap-1">
      <Button
        variant="ghost"
        size="icon"
        className="h-8 w-8 rounded-lg"
        onClick={() => onEdit(item)}
        disabled={isSubmitting}
      >
        <Edit className="h-3.5 w-3.5" />
      </Button>
      <Button
        variant="ghost"
        size="icon"
        className="text-destructive/60 hover:text-destructive hover:bg-destructive/10 h-8 w-8 rounded-lg"
        onClick={() => onDelete(item)}
        disabled={isSubmitting}
      >
        <Trash2 className="h-3.5 w-3.5" />
      </Button>
    </div>
  );
}
