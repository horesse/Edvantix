import type { DirectoryItemBase } from "@workspace/types/organization";

import { levelsConfig } from "./configs/levels";
import type { DirectoryConfig } from "./directory-config";

/** Реестр реализованных конфигов справочников: code → config. */
export const directoryRegistry: Record<
  string,
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  DirectoryConfig<DirectoryItemBase, any>
> = {
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  levels: levelsConfig as DirectoryConfig<DirectoryItemBase, any>,
};

/** Проверяет, реализован ли справочник с данным кодом. */
export function isDirectoryImplemented(code: string): boolean {
  return code in directoryRegistry;
}
