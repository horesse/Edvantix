import type { UseQueryOptions } from "@tanstack/react-query";
import { useQuery } from "@tanstack/react-query";

import organizationApiClient from "@workspace/api-client/organization/organization";
import type { RoleDto, RolesQuery } from "@workspace/types/organization";
import type { PagedResult } from "@workspace/types/shared";

import { organizationKeys } from "../keys";

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
    queryKey: organizationKeys.roles(orgId, query),
    queryFn: () => organizationApiClient.getRoles(query),
    enabled: Boolean(orgId),
    ...options,
  });
}
