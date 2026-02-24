import type { UseQueryOptions } from "@tanstack/react-query";
import { useQuery } from "@tanstack/react-query";

import companyApiClient from "@workspace/api-client/company/company";
import type { InvitationModel } from "@workspace/types/company";

import { companyKeys } from "../keys";

export default function usePendingInvitations(
  orgId: string,
  options?: Omit<UseQueryOptions<InvitationModel[]>, "queryKey" | "queryFn">,
) {
  return useQuery({
    queryKey: companyKeys.invitations(orgId),
    queryFn: () => companyApiClient.getPendingInvitations(orgId),
    enabled: Boolean(orgId),
    ...options,
  });
}
