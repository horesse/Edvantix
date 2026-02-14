import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import companyApiClient from "@workspace/api-client/company/company";

import { companyKeys } from "../keys";

type RemoveMemberParams = {
  orgId: number;
  memberId: string;
};

export default function useRemoveMember(
  options?: UseMutationOptions<void, Error, RemoveMemberParams>,
) {
  const queryClient = useQueryClient();

  return useMutation({
    ...options,
    mutationFn: ({ orgId, memberId }) =>
      companyApiClient.removeMember(orgId, memberId),
    onSuccess: (...args) => {
      const { orgId } = args[1];
      queryClient.invalidateQueries({ queryKey: companyKeys.members(orgId) });
      queryClient.invalidateQueries({
        queryKey: companyKeys.organization(orgId),
      });
      options?.onSuccess?.(...args);
    },
    onError: (...args) => {
      options?.onError?.(...args);
    },
  });
}
