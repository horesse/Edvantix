import type { UseQueryOptions } from "@tanstack/react-query";
import { useQuery } from "@tanstack/react-query";

import companyApiClient from "@workspace/api-client/company/company";
import type { RoleDetailDto } from "@workspace/types/company";

import { companyKeys } from "../keys";

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
    queryKey: companyKeys.role(orgId, roleId),
    queryFn: () => companyApiClient.getRole(roleId),
    enabled: Boolean(orgId) && Boolean(roleId),
    ...options,
  });
}
