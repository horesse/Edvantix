import { Layers } from "lucide-react";

import {
  LEVEL_TONE_COLORS,
  LevelTone,
  type CreateLevelDirectoryRequest,
  type DirectoryUsageDto,
  type LevelDirectoryListItem,
  type UpdateLevelDirectoryRequest,
} from "@workspace/types/organization";
import { levelDirectorySchema } from "@workspace/validations/organization/directories/levels";
import type { LevelDirectoryFormValues } from "@workspace/validations/organization/directories/levels";

import { COLOR_DOTS } from "../color-palette";
import type { DirectoryConfig } from "../directory-config";

/** Преобразует числовой LevelTone → строковый ключ палитры (например, 1 → "teal"). */
function toneToColorKey(tone: LevelTone): string {
  return LevelTone[tone].toLowerCase();
}

/** Преобразует строковый ключ палитры → числовой LevelTone (например, "teal" → 1). */
function colorKeyToTone(key: string): LevelTone {
  const name = key.charAt(0).toUpperCase() + key.slice(1);
  const value = LevelTone[name as keyof typeof LevelTone];
  return value ?? LevelTone.Indigo;
}

export const levelsConfig: DirectoryConfig<
  LevelDirectoryListItem,
  LevelDirectoryFormValues
> = {
  code: "levels",
  singular: "уровень",
  plural: "Уровни",
  description:
    "Уровни обучения, по которым формируются группы и подбираются программы. Используется в курсах, расписании и зачислении.",
  icon: Layers,
  capabilities: {
    reorder: true,
    archive: true,
  },
  columns: [
    {
      key: "tone",
      header: "",
      className: "w-7 px-0 pl-4",
      render(item) {
        const hex = LEVEL_TONE_COLORS[item.tone] ?? COLOR_DOTS.indigo;
        return (
          <span
            className="block size-3.5 rounded-full"
            style={{
              background: hex,
              boxShadow: `0 0 0 3px ${hex}1a`,
            }}
          />
        );
      },
    },
    {
      key: "name",
      header: "Название",
      render(item) {
        return (
          <div className="min-w-0">
            <p className="truncate text-sm font-semibold text-slate-900">
              {item.name}
            </p>
            {item.description && (
              <p className="mt-0.5 line-clamp-1 text-xs text-slate-500">
                {item.description}
              </p>
            )}
          </div>
        );
      },
    },
    {
      key: "code",
      header: "Код",
      className: "w-24",
      render(item) {
        return (
          <span className="rounded-md bg-slate-100 px-2 py-0.5 font-mono text-xs tracking-wider text-slate-600">
            {item.code}
          </span>
        );
      },
    },
  ],
  fields: [
    {
      kind: "text",
      name: "name",
      label: "Название",
      required: true,
      maxLength: 64,
      placeholder: "Например, B1 — Средний",
    },
    {
      kind: "row",
      name: "__row_code_status",
      children: [
        {
          kind: "code",
          name: "code",
          label: "Код",
          maxLength: 16,
          hint: "до 16 символов",
        },
        {
          kind: "statusToggle",
          name: "isArchived",
          label: "Статус",
          showOnlyInEdit: true,
        },
      ],
    },
    {
      kind: "color",
      name: "tone",
      label: "Цвет метки",
    },
    {
      kind: "textarea",
      name: "description",
      label: "Описание",
      maxLength: 256,
      rows: 4,
      hint: "видно только в справочнике",
    },
  ],
  schema: levelDirectorySchema,
  defaults: {
    name: "",
    code: "",
    description: "",
    tone: "indigo",
    isArchived: false,
  },
  getHeaderColor(form) {
    return COLOR_DOTS[form.tone ?? "indigo"];
  },
  toStatusChange(form, item) {
    if (form.isArchived && !item.isArchived) return "archive";
    if (!form.isArchived && item.isArchived) return "restore";
    return null;
  },
  toCreate(form): CreateLevelDirectoryRequest {
    return {
      name: form.name,
      code: form.code,
      description: form.description || undefined,
      order: 0,
      tone: colorKeyToTone(form.tone ?? "indigo"),
    };
  },
  toUpdate(form, item): UpdateLevelDirectoryRequest {
    return {
      name: form.name,
      code: form.code,
      description: form.description || undefined,
      tone: colorKeyToTone(form.tone ?? "indigo"),
      order: item.order,
    };
  },
  fromItem(item): LevelDirectoryFormValues {
    return {
      name: item.name,
      description: item.description ?? "",
      tone: toneToColorKey(item.tone),
      code: item.code,
      isArchived: item.isArchived,
    };
  },
  usageCards(item): DirectoryUsageDto[] {
    return item.usage ? [...item.usage] : [];
  },
};
