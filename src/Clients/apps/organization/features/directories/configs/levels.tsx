import { Layers } from "lucide-react";

import type {
  CreateLevelDirectoryRequest,
  DirectoryUsageDto,
  LevelDirectoryListItem,
} from "@workspace/types/organization";
import { levelDirectorySchema } from "@workspace/validations/organization/directories/levels";
import type { LevelDirectoryFormValues } from "@workspace/validations/organization/directories/levels";

import type { DirectoryConfig } from "../directory-config";

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
    description: "",
  },
  toCreate(form): CreateLevelDirectoryRequest {
    return {
      name: form.name,
      description: form.description || undefined,
      order: 0,
    };
  },
  toUpdate(form) {
    return {
      name: form.name,
      description: form.description || undefined,
    };
  },
  fromItem(item): LevelDirectoryFormValues {
    return {
      name: item.name,
      description: item.description ?? "",
    };
  },
  usageCards(item): DirectoryUsageDto[] {
    return item.usage ? [...item.usage] : [];
  },
};
