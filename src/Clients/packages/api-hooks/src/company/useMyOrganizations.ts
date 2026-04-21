import type { UseQueryOptions } from "@tanstack/react-query";
import { useQuery } from "@tanstack/react-query";

import companyApiClient from "@workspace/api-client/company/company";
import type { OrganizationWithRoleDto } from "@workspace/types/company";

import { companyKeys } from "../keys";

/** Возвращает список организаций текущего пользователя с его ролью в каждой. */
export default function useMyOrganizations(
  options?: Omit<
    UseQueryOptions<readonly OrganizationWithRoleDto[]>,
    "queryKey" | "queryFn"
  >,
) {
  return useQuery({
    queryKey: companyKeys.myOrganizations(),
    queryFn: () => companyApiClient.getMyOrganizations(),
    ...options,
  });
}
