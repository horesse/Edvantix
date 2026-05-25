import type { UseQueryOptions } from "@tanstack/react-query";
import { useQuery } from "@tanstack/react-query";

import organizationApiClient from "@workspace/api-client/organization/organization";
import type { RoleDetailDto } from "@workspace/types/organization";

import { organizationKeys } from "../keys";

/**
 * Возвращает детальную информацию о роли организации по ID.
 * @param orgId   ID организации — используется как часть query key.
 * @param roleId  ID роли.
 */
export default function useRole(
  orgId: string,
  roleId: string,
  options?: Omit<UseQueryOptions<RoleDetailDto>, "queryKey" | "queryFn">,
) {
  return useQuery({
    queryKey: organizationKeys.role(orgId, roleId),
    queryFn: () => organizationApiClient.getRole(roleId),
    enabled: Boolean(orgId) && Boolean(roleId),
    ...options,
  });
}
