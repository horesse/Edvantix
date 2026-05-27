import type { DirectoryItemBase } from "@workspace/types/organization";

import { levelsConfig } from "./configs/levels";
import type { DirectoryConfig } from "./directory-config";

/** Реестр реализованных конфигов справочников: code → config. */
// eslint-disable-next-line @typescript-eslint/no-explicit-any
export const directoryRegistry: Record<
  string,
  DirectoryConfig<DirectoryItemBase, any>
> = {
  levels: levelsConfig,
};

/** Проверяет, реализован ли справочник с данным кодом. */
export function isDirectoryImplemented(code: string): boolean {
  return code in directoryRegistry;
}
