import type { UseQueryOptions } from "@tanstack/react-query";
import { useQuery } from "@tanstack/react-query";

import companyApiClient from "@workspace/api-client/company/company";
import type { RoleDto, RolesQuery } from "@workspace/types/company";
import type { PagedResult } from "@workspace/types/shared";

import { companyKeys } from "../keys";

/**
 * Возвращает список ролей текущей организации.
 * @param orgId  ID организации — используется как часть query key.
 */
export default function useRoles(
  orgId: string,
  query?: RolesQuery,
  options?: Omit<UseQueryOptions<PagedResult<RoleDto>>, "queryKey" | "queryFn">,
) {
  return useQuery({
    queryKey: companyKeys.roles(orgId, query),
    queryFn: () => companyApiClient.getRoles(query),
    enabled: Boolean(orgId),
    ...options,
  });
}
