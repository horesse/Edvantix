import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import companyApiClient from "@workspace/api-client/company/company";
import type { CreateInvitationRequest } from "@workspace/types/company";

import { companyKeys } from "../keys";

type CreateInvitationParams = {
  orgId: string;
  request: CreateInvitationRequest;
};

export default function useCreateInvitation(
  options?: UseMutationOptions<void, Error, CreateInvitationParams>,
) {
  const queryClient = useQueryClient();

  return useMutation({
    ...options,
    mutationFn: ({ orgId, request }) =>
      companyApiClient.createInvitation(orgId, request),
    onSuccess: (...args) => {
      const { orgId } = args[1];
      queryClient.invalidateQueries({
        queryKey: companyKeys.invitations(orgId),
      });
      options?.onSuccess?.(...args);
    },
    onError: (...args) => {
      options?.onError?.(...args);
    },
  });
}
