"use client";

import type { ReactNode } from "react";

import {
  DndContext,
  type DragEndEvent,
  KeyboardSensor,
  PointerSensor,
  closestCenter,
  useSensor,
  useSensors,
} from "@dnd-kit/core";
import {
  SortableContext,
  sortableKeyboardCoordinates,
  useSortable,
  verticalListSortingStrategy,
} from "@dnd-kit/sortable";
import { CSS } from "@dnd-kit/utilities";
import {
  Archive,
  GripVertical,
  MoreHorizontal,
  Pencil,
  RotateCcw,
} from "lucide-react";

import type { DirectoryItemBase } from "@workspace/types/organization";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@workspace/ui/components/dropdown-menu";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@workspace/ui/components/table";
import { cn } from "@workspace/ui/lib/utils";

import type { DirectoryConfig } from "./directory-config";
import { StatusBadge } from "./status-badge";
import { UsageCell } from "./usage-cell";

interface DirectoryTableProps<
  TItem extends DirectoryItemBase,
  TForm extends Record<string, unknown>,
> {
  config: DirectoryConfig<TItem, TForm>;
  items: TItem[];
  activeTab: "active" | "archived";
  onEdit: (item: TItem) => void;
  onArchive: (item: TItem) => void;
  onRestore: (item: TItem) => void;
  onReorder: (orderedIds: string[]) => void;
}

// ── Sortable row ─────────────────────────────────────────────────────────────

interface SortableRowProps<TItem extends DirectoryItemBase> {
  item: TItem;
  draggable: boolean;
  children: ReactNode;
}

function SortableRow<TItem extends DirectoryItemBase>({
  item,
  draggable,
  children,
}: Readonly<SortableRowProps<TItem>>) {
  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition,
    isDragging,
  } = useSortable({ id: item.id, disabled: !draggable });

  return (
    <TableRow
      ref={setNodeRef}
      style={{ transform: CSS.Transform.toString(transform), transition }}
      className={cn(
        "transition-colors",
        isDragging && "bg-indigo-50/40 opacity-50",
        item.isArchived && "opacity-60",
      )}
      data-drag-handle-listeners={JSON.stringify(listeners)}
      data-drag-handle-attributes={JSON.stringify(attributes)}
    >
      {/* Проброс listeners/attributes вниз через дата-атрибуты не работает в React.
          Рендерим drag-handle прямо здесь, принимая callback как prop. */}
      {children}
    </TableRow>
  );
}

/** Тип для пробрасывания drag-handle из SortableRow наружу нельзя сделать чисто.
 *  Поэтому делаем отдельный компонент, который держит useSortable сам. */
interface SortableTableRowProps<
  TItem extends DirectoryItemBase,
  TForm extends Record<string, unknown>,
> {
  item: TItem;
  config: DirectoryConfig<TItem, TForm>;
  draggable: boolean;
  onEdit: (item: TItem) => void;
  onArchive: (item: TItem) => void;
  onRestore: (item: TItem) => void;
}

function DirectoryTableRow<
  TItem extends DirectoryItemBase,
  TForm extends Record<string, unknown>,
>({
  item,
  config,
  draggable,
  onEdit,
  onArchive,
  onRestore,
}: Readonly<SortableTableRowProps<TItem, TForm>>) {
  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition,
    isDragging,
  } = useSortable({ id: item.id, disabled: !draggable });

  const usageCards =
    config.usageCards?.(item) ?? (item.usage ? [...item.usage] : []);

  return (
    <TableRow
      ref={setNodeRef}
      style={{ transform: CSS.Transform.toString(transform), transition }}
      className={cn(
        "group cursor-pointer transition-colors",
        isDragging && "bg-indigo-50/40 opacity-50",
        item.isArchived && "opacity-70",
      )}
      onClick={() => onEdit(item)}
    >
      {config.capabilities.reorder && (
        <TableCell
          className="w-9 px-2"
          onClick={(e) => e.stopPropagation()}
        >
          <button
            type="button"
            {...attributes}
            {...listeners}
            aria-label="Перетащить"
            title={
              item.isArchived
                ? "Архивные записи нельзя сортировать"
                : "Перетащите для изменения порядка"
            }
            className={cn(
              "flex size-7 items-center justify-center rounded text-slate-300 transition-colors",
              draggable
                ? "cursor-grab hover:text-slate-500 active:cursor-grabbing"
                : "cursor-not-allowed",
            )}
          >
            <GripVertical className="size-4" />
          </button>
        </TableCell>
      )}

      {config.columns.map((col) => (
        <TableCell key={col.key} className={cn("py-3", col.className)}>
          {col.render(item)}
        </TableCell>
      ))}

      <TableCell className="py-3">
        <UsageCell usage={usageCards} dim={item.isArchived} />
      </TableCell>

      <TableCell className="py-3">
        <StatusBadge isArchived={item.isArchived} />
      </TableCell>

      <TableCell
        className="w-12 py-3 text-right"
        onClick={(e) => e.stopPropagation()}
      >
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <button
              type="button"
              aria-label="Действия"
              className="flex size-8 items-center justify-center rounded-lg text-slate-400 opacity-0 transition-all group-hover:opacity-100 hover:bg-slate-100 hover:text-slate-600"
            >
              <MoreHorizontal className="size-4" />
            </button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end" className="w-48">
            <DropdownMenuItem onClick={() => onEdit(item)}>
              <Pencil className="mr-2 size-4" />
              Редактировать
            </DropdownMenuItem>
            {config.capabilities.archive && (
              <>
                <DropdownMenuSeparator />
                {item.isArchived ? (
                  <DropdownMenuItem onClick={() => onRestore(item)}>
                    <RotateCcw className="mr-2 size-4" />
                    Восстановить
                  </DropdownMenuItem>
                ) : (
                  <DropdownMenuItem
                    onClick={() => onArchive(item)}
                    className="text-amber-600 focus:text-amber-700"
                  >
                    <Archive className="mr-2 size-4" />В архив
                  </DropdownMenuItem>
                )}
              </>
            )}
          </DropdownMenuContent>
        </DropdownMenu>
      </TableCell>
    </TableRow>
  );
}

// ── Main table ────────────────────────────────────────────────────────────────

/** Таблица справочника с drag-and-drop сортировкой (только активные записи). */
export function DirectoryTable<
  TItem extends DirectoryItemBase,
  TForm extends Record<string, unknown>,
>({
  config,
  items,
  activeTab,
  onEdit,
  onArchive,
  onRestore,
  onReorder,
}: Readonly<DirectoryTableProps<TItem, TForm>>) {
  const sensors = useSensors(
    useSensor(PointerSensor),
    useSensor(KeyboardSensor, {
      coordinateGetter: sortableKeyboardCoordinates,
    }),
  );

  function handleDragEnd(event: DragEndEvent) {
    const { active, over } = event;
    if (!over || active.id === over.id) return;

    const ids = items.map((i) => i.id);
    const fromIdx = ids.indexOf(active.id as string);
    const toIdx = ids.indexOf(over.id as string);
    if (fromIdx === -1 || toIdx === -1) return;

    const reordered = [...ids];
    const [moved] = reordered.splice(fromIdx, 1);
    // fromIdx проверен выше, поэтому moved гарантированно определён
    reordered.splice(toIdx, 0, moved!);
    onReorder(reordered);
  }

  const canDrag = config.capabilities.reorder && activeTab === "active";

  return (
    <div className="overflow-hidden rounded-xl border bg-white shadow-xs">
      <DndContext
        sensors={sensors}
        collisionDetection={closestCenter}
        onDragEnd={handleDragEnd}
      >
        <SortableContext
          items={items.map((i) => i.id)}
          strategy={verticalListSortingStrategy}
        >
          <Table>
            <TableHeader>
              <TableRow className="bg-slate-50/80">
                {config.capabilities.reorder && (
                  <TableHead className="w-9 px-2" />
                )}
                {config.columns.map((col) => (
                  <TableHead
                    key={col.key}
                    className={cn(
                      "text-[11px] font-semibold tracking-wider text-slate-500 uppercase",
                      col.className,
                    )}
                  >
                    {col.header}
                  </TableHead>
                ))}
                <TableHead className="text-[11px] font-semibold tracking-wider text-slate-500 uppercase">
                  Использование
                </TableHead>
                <TableHead className="text-[11px] font-semibold tracking-wider text-slate-500 uppercase">
                  Статус
                </TableHead>
                <TableHead className="w-12" />
              </TableRow>
            </TableHeader>
            <TableBody>
              {items.map((item) => (
                <DirectoryTableRow
                  key={item.id}
                  item={item}
                  config={config}
                  draggable={canDrag && !item.isArchived}
                  onEdit={onEdit}
                  onArchive={onArchive}
                  onRestore={onRestore}
                />
              ))}
            </TableBody>
          </Table>
        </SortableContext>
      </DndContext>
    </div>
  );
}
