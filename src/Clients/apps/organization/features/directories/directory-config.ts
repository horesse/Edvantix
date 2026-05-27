import type { ReactNode } from "react";

import type { LucideIcon } from "lucide-react";
import type { ZodSchema } from "zod";

import type {
  DirectoryItemBase,
  DirectoryUsageDto,
} from "@workspace/types/organization";

/** Описание колонки таблицы справочника. */
export type DirectoryColumn<T> = {
  key: string;
  header: string;
  render(item: T): ReactNode;
  className?: string;
};

/** Дескриптор поля формы в drawer'е справочника. */
export type DirectoryField =
  | { kind: "text"; name: string; label: string; required?: boolean; maxLength?: number; placeholder?: string; hint?: string }
  | { kind: "code"; name: string; label: string; maxLength?: number; hint?: string }
  | { kind: "textarea"; name: string; label: string; maxLength?: number; rows?: number; hint?: string }
  | { kind: "color"; name: string; label: string }
  | { kind: "enumSelect"; name: string; label: string; options: { value: string | number; label: string }[] }
  | { kind: "number"; name: string; label: string; min?: number; max?: number; suffix?: string }
  | { kind: "switch"; name: string; label: string; hint?: string };

/** Полный конфиг справочника: колонки, поля формы, маппинги запросов. */
export type DirectoryConfig<TItem extends DirectoryItemBase, TForm extends Record<string, unknown>> = {
  /** Код справочника (последний сегмент URL и ключ API). */
  code: string;
  /** Единственное число: «уровень», «предмет». */
  singular: string;
  /** Множественное число: «Уровни», «Предметы». */
  plural: string;
  /** Описание справочника, отображаемое под заголовком. */
  description: string;
  /** Иконка Lucide для заголовка. */
  icon: LucideIcon;
  /** Поддерживаемые возможности. */
  capabilities: {
    reorder: boolean;
    archive: boolean;
  };
  /** Колонки таблицы (без служебных: drag, usage, status, actions). */
  columns: DirectoryColumn<TItem>[];
  /** Поля формы в drawer'е. */
  fields: DirectoryField[];
  /** Zod-схема для валидации формы. */
  schema: ZodSchema<TForm>;
  /** Значения формы по умолчанию для создания. */
  defaults: TForm;
  /** form → тело запроса создания. */
  toCreate(form: TForm): unknown;
  /** form + item → тело запроса обновления. */
  toUpdate(form: TForm, item: TItem): unknown;
  /** item → начальные значения формы редактирования. */
  fromItem(item: TItem): TForm;
  /** Формирует usage-карточки; по умолчанию берёт item.usage. */
  usageCards?: (item: TItem) => DirectoryUsageDto[];
};
