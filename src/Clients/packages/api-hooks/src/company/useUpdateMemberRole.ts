import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import companyApiClient from "@workspace/api-client/company/company";
import type { UpdateMemberRoleRequest } from "@workspace/types/company";

import { companyKeys } from "../keys";

type UpdateMemberRoleParams = {
  orgId: number;
  memberId: string;
  request: UpdateMemberRoleRequest;
};

export default function useUpdateMemberRole(
  options?: UseMutationOptions<void, Error, UpdateMemberRoleParams>,
) {
  const queryClient = useQueryClient();

  return useMutation({
    ...options,
    mutationFn: ({ orgId, memberId, request }) =>
      companyApiClient.updateMemberRole(orgId, memberId, request),
    onSuccess: (...args) => {
      const { orgId } = args[1];
      queryClient.invalidateQueries({ queryKey: companyKeys.members(orgId) });
      options?.onSuccess?.(...args);
    },
    onError: (...args) => {
      options?.onError?.(...args);
    },
  });
}
