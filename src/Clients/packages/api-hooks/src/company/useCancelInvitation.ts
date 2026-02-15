import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import companyApiClient from "@workspace/api-client/company/company";

import { companyKeys } from "../keys";

type CancelInvitationParams = {
  orgId: number;
  invitationId: string;
};

export default function useCancelInvitation(
  options?: UseMutationOptions<void, Error, CancelInvitationParams>,
) {
  const queryClient = useQueryClient();

  return useMutation({
    ...options,
    mutationFn: ({ orgId, invitationId }) =>
      companyApiClient.cancelInvitation(orgId, invitationId),
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
