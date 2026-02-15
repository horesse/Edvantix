import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import companyApiClient from "@workspace/api-client/company/company";
import type { UpdateGroupMemberRoleRequest } from "@workspace/types/company";

import { companyKeys } from "../keys";

type UpdateGroupMemberRoleParams = {
  groupId: number;
  memberId: string;
  request: UpdateGroupMemberRoleRequest;
};

export default function useUpdateGroupMemberRole(
  options?: UseMutationOptions<void, Error, UpdateGroupMemberRoleParams>,
) {
  const queryClient = useQueryClient();

  return useMutation({
    ...options,
    mutationFn: ({ groupId, memberId, request }) =>
      companyApiClient.updateGroupMemberRole(groupId, memberId, request),
    onSuccess: (...args) => {
      const { groupId } = args[1];
      queryClient.invalidateQueries({
        queryKey: companyKeys.groupMembers(groupId),
      });
      options?.onSuccess?.(...args);
    },
    onError: (...args) => {
      options?.onError?.(...args);
    },
  });
}
