import type { UseQueryOptions } from "@tanstack/react-query";
import { useQuery } from "@tanstack/react-query";

import organizationApiClient from "@workspace/api-client/organization/organization";
import type { OrganizationWithRoleDto } from "@workspace/types/organization";

import { organizationKeys } from "../keys";

/** Возвращает список организаций текущего пользователя с его ролью в каждой. */
export default function useMyOrganizations(
  options?: Omit<
    UseQueryOptions<readonly OrganizationWithRoleDto[]>,
    "queryKey" | "queryFn"
  >,
) {
  return useQuery({
    queryKey: organizationKeys.myOrganizations(),
    queryFn: () => organizationApiClient.getMyOrganizations(),
    ...options,
  });
}
