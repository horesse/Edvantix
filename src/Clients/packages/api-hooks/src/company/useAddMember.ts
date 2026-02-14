import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import companyApiClient from "@workspace/api-client/company/company";
import type { AddMemberRequest } from "@workspace/types/company";

import { companyKeys } from "../keys";

type AddMemberParams = {
  orgId: number;
  request: AddMemberRequest;
};

export default function useAddMember(
  options?: UseMutationOptions<void, Error, AddMemberParams>,
) {
  const queryClient = useQueryClient();

  return useMutation({
    ...options,
    mutationFn: ({ orgId, request }) =>
      companyApiClient.addMember(orgId, request),
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
