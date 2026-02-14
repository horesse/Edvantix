import type { UseQueryOptions } from "@tanstack/react-query";
import { useQuery } from "@tanstack/react-query";

import companyApiClient from "@workspace/api-client/company/company";
import type { OrganizationContactModel } from "@workspace/types/company";

import { companyKeys } from "../keys";

export default function useOrganizationContacts(
  orgId: number,
  options?: Omit<
    UseQueryOptions<OrganizationContactModel[]>,
    "queryKey" | "queryFn"
  >,
) {
  return useQuery({
    queryKey: companyKeys.contacts(orgId),
    queryFn: () => companyApiClient.getContacts(orgId),
    enabled: orgId > 0,
    ...options,
  });
}
