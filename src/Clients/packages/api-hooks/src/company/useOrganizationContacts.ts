import type { UseQueryOptions } from "@tanstack/react-query";
import { useQuery } from "@tanstack/react-query";

import companyApiClient, {
  type OrganizationContactsQuery,
} from "@workspace/api-client/company/company";
import type { OrganizationContactModel } from "@workspace/types/company";
import type { PagedResult } from "@workspace/types/shared";

import { companyKeys } from "../keys";

export default function useOrganizationContacts(
  orgId: string,
  query?: Omit<OrganizationContactsQuery, "organizationId">,
  options?: Omit<
    UseQueryOptions<PagedResult<OrganizationContactModel>>,
    "queryKey" | "queryFn"
  >,
) {
  return useQuery({
    queryKey: companyKeys.contacts(orgId, query),
    queryFn: () =>
      companyApiClient.getContacts({
        organizationId: orgId,
        ...query,
      }),
    enabled: Boolean(orgId),
    ...options,
  });
}
