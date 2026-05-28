/** Карточка использования элемента справочника. */
export type DirectoryUsageDto = {
  readonly label: string;
  readonly count: number;
};

/** Базовые поля любого элемента справочника. */
export type DirectoryItemBase = {
  readonly id: string;
  readonly name: string;
  readonly order: number;
  readonly isArchived: boolean;
  readonly usage?: readonly DirectoryUsageDto[];
};

/** Запрос на переупорядочивание элементов справочника. */
export type ReorderDirectoryRequest = {
  readonly orderedIds: readonly string[];
};

/** Параметры фильтрации/пагинации для списка справочника. */
export type DirectoryListQuery = {
  search?: string;
  includeArchived?: boolean;
  pageIndex?: number;
  pageSize?: number;
};

// --- Уровни ---

/** Тональность уровня (цветовая метка). */
export enum LevelTone {
  Sky = 0,
  Teal = 1,
  Indigo = 2,
  Violet = 3,
  Amber = 4,
  Rose = 5,
  Pink = 6,
  Slate = 7,
  Emerald = 8,
  Blue = 9,
}

/** Маппинг LevelTone → HEX-цвет. */
export const LEVEL_TONE_COLORS: Record<LevelTone, string> = {
  [LevelTone.Sky]: "#0ea5e9",
  [LevelTone.Teal]: "#14b8a6",
  [LevelTone.Indigo]: "#6366f1",
  [LevelTone.Violet]: "#8b5cf6",
  [LevelTone.Amber]: "#f59e0b",
  [LevelTone.Rose]: "#f43f5e",
  [LevelTone.Pink]: "#ec4899",
  [LevelTone.Slate]: "#94a3b8",
  [LevelTone.Emerald]: "#10b981",
  [LevelTone.Blue]: "#3b82f6",
};

/** Маппинг LevelTone → метка для UI. */
export const LEVEL_TONE_LABELS: Record<LevelTone, string> = {
  [LevelTone.Sky]: "Голубой",
  [LevelTone.Teal]: "Бирюзовый",
  [LevelTone.Indigo]: "Индиго",
  [LevelTone.Violet]: "Фиолетовый",
  [LevelTone.Amber]: "Янтарный",
  [LevelTone.Rose]: "Розовый",
  [LevelTone.Pink]: "Пурпурный",
  [LevelTone.Slate]: "Серый",
  [LevelTone.Emerald]: "Изумрудный",
  [LevelTone.Blue]: "Синий",
};

/** Элемент справочника «Уровни» в списке. */
export type LevelDirectoryListItem = DirectoryItemBase & {
  readonly description?: string;
  readonly code: string;
  readonly tone: LevelTone;
};

/** Детальный элемент справочника «Уровни». */
export type LevelDirectoryDetailItem = LevelDirectoryListItem;

/** Запрос на создание уровня. */
export type CreateLevelDirectoryRequest = {
  readonly name: string;
  readonly code: string;
  readonly order: number;
  readonly description?: string;
  readonly tone?: LevelTone;
};

/** Запрос на обновление уровня. */
export type UpdateLevelDirectoryRequest = {
  readonly name: string;
  readonly code: string;
  readonly order: number;
  readonly description?: string;
  readonly tone: LevelTone;
};
