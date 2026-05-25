import type { UseQueryOptions } from "@tanstack/react-query";
import { useQuery } from "@tanstack/react-query";

import organizationApiClient from "@workspace/api-client/organization/organization";
import type { DirectorySummaryDto } from "@workspace/types/organization";

import { organizationKeys } from "../keys";

/** Возвращает каталог справочников с метаданными для страницы настроек. */
export default function useDirectoriesCatalog(
  orgId: string,
  options?: Omit<
    UseQueryOptions<readonly DirectorySummaryDto[]>,
    "queryKey" | "queryFn"
  >,
) {
  return useQuery({
    queryKey: organizationKeys.directoriesCatalog(orgId),
    queryFn: () => organizationApiClient.getDirectoriesCatalog(),
    enabled: Boolean(orgId),
    ...options,
  });
}
