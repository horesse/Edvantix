import { Tag } from "lucide-react";

import { Skeleton } from "@workspace/ui/components/skeleton";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@workspace/ui/components/table";

import { TagCellAction } from "./cell-action";
import { TAG_COLUMNS, type TagRow } from "./columns";

type TagsTableProps = Readonly<{
  items: TagRow[];
  isLoading: boolean;
  isSubmitting: boolean;
  onEdit: (item: TagRow) => void;
  onDelete: (item: TagRow) => void;
}>;

export function TagsTable({
  items,
  isLoading,
  isSubmitting,
  onEdit,
  onDelete,
}: TagsTableProps) {
  if (isLoading) {
    return (
      <div className="space-y-2">
        {Array.from({ length: 5 }).map((_, i) => (
          <Skeleton key={i} className="h-14 w-full rounded-lg" />
        ))}
      </div>
    );
  }

  return (
    <div className="border-border overflow-hidden rounded-xl border">
      <Table>
        <TableHeader>
          <TableRow className="bg-muted/40">
            {TAG_COLUMNS.map((column) => (
              <TableHead key={column.id} className={column.className}>
                {column.label}
              </TableHead>
            ))}
          </TableRow>
        </TableHeader>
        <TableBody>
          {items.length === 0 ? (
            <TableRow>
              <TableCell
                colSpan={TAG_COLUMNS.length}
                className="py-16 text-center"
              >
                <div className="flex flex-col items-center gap-2">
                  <div className="bg-muted flex h-12 w-12 items-center justify-center rounded-full">
                    <Tag className="text-muted-foreground h-5 w-5" />
                  </div>
                  <p className="text-muted-foreground text-sm">
                    No tags yet. Create one to get started.
                  </p>
                </div>
              </TableCell>
            </TableRow>
          ) : (
            items.map((item) => (
              <TableRow
                key={item.id}
                className="hover:bg-muted/30 transition-colors"
              >
                <TableCell>
                  <div className="flex items-center gap-2.5">
                    <div className="bg-primary/10 flex h-7 w-7 shrink-0 items-center justify-center rounded-lg">
                      <Tag className="text-primary h-3.5 w-3.5" />
                    </div>
                    <span className="font-medium">{item.name}</span>
                  </div>
                </TableCell>
                <TableCell>
                  <code className="bg-muted text-muted-foreground rounded px-1.5 py-0.5 font-mono text-xs">
                    {item.slug}
                  </code>
                </TableCell>
                <TableCell>
                  <TagCellAction
                    item={item}
                    onEdit={onEdit}
                    onDelete={onDelete}
                    isSubmitting={isSubmitting}
                  />
                </TableCell>
              </TableRow>
            ))
          )}
        </TableBody>
      </Table>
    </div>
  );
}
