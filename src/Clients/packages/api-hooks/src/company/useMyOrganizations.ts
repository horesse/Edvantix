import type { UseQueryOptions } from "@tanstack/react-query";
import { useQuery } from "@tanstack/react-query";

import companyApiClient from "@workspace/api-client/company/company";
import type { OrganizationSummaryModel } from "@workspace/types/company";

import { companyKeys } from "../keys";

export default function useMyOrganizations(
  options?: Omit<
    UseQueryOptions<OrganizationSummaryModel[]>,
    "queryKey" | "queryFn"
  >,
) {
  return useQuery({
    queryKey: companyKeys.myOrganizations(),
    queryFn: () => companyApiClient.getMyOrganizations(),
    ...options,
  });
}
