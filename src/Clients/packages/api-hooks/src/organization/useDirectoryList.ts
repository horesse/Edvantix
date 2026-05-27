import type { UseQueryOptions } from "@tanstack/react-query";
import { useQuery } from "@tanstack/react-query";

import directoriesApiClient from "@workspace/api-client/organization/directories";
import type {
  DirectoryItemBase,
  DirectoryListQuery,
} from "@workspace/types/organization";
import type { PagedResult } from "@workspace/types/shared";

import { organizationKeys } from "../keys";

/**
 * Возвращает постраничный список элементов справочника по коду.
 * @param orgId  ID организации — используется только как часть query key.
 * @param code   Код справочника (levels, subjects, …).
 */
export default function useDirectoryList<T extends DirectoryItemBase>(
  orgId: string,
  code: string,
  query?: DirectoryListQuery,
  options?: Omit<UseQueryOptions<PagedResult<T>>, "queryKey" | "queryFn">,
) {
  return useQuery({
    queryKey: organizationKeys.directory(orgId, code, query),
    queryFn: () => directoriesApiClient.list<T>(code, query),
    enabled: Boolean(orgId) && Boolean(code),
    ...options,
  });
}
