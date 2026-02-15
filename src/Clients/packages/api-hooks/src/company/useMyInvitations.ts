import type { UseQueryOptions } from "@tanstack/react-query";
import { useQuery } from "@tanstack/react-query";

import companyApiClient from "@workspace/api-client/company/company";
import type { InvitationModel } from "@workspace/types/company";

import { companyKeys } from "../keys";

export default function useMyInvitations(
  options?: Omit<UseQueryOptions<InvitationModel[]>, "queryKey" | "queryFn">,
) {
  return useQuery({
    queryKey: companyKeys.myInvitations(),
    queryFn: () => companyApiClient.getMyInvitations(),
    ...options,
  });
}
