import type { UseQueryOptions } from "@tanstack/react-query";
import { useQuery } from "@tanstack/react-query";

import companyApiClient from "@workspace/api-client/company/company";
import type { OrganizationDetailDto } from "@workspace/types/company";

import { companyKeys } from "../keys";

/** Возвращает полные данные организации по её ID. */
export default function useOrganization(
  id: string,
  options?: Omit<
    UseQueryOptions<OrganizationDetailDto>,
    "queryKey" | "queryFn"
  >,
) {
  return useQuery({
    queryKey: companyKeys.organization(id),
    queryFn: () => companyApiClient.getOrganization(id),
    enabled: Boolean(id),
    ...options,
  });
}
