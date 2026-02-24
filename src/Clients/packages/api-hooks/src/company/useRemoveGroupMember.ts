import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import companyApiClient from "@workspace/api-client/company/company";

import { companyKeys } from "../keys";

type RemoveGroupMemberParams = {
  groupId: string;
  memberId: string;
};

export default function useRemoveGroupMember(
  options?: UseMutationOptions<void, Error, RemoveGroupMemberParams>,
) {
  const queryClient = useQueryClient();

  return useMutation({
    ...options,
    mutationFn: ({ groupId, memberId }) =>
      companyApiClient.removeGroupMember(groupId, memberId),
    onSuccess: (...args) => {
      const { groupId } = args[1];
      queryClient.invalidateQueries({
        queryKey: companyKeys.groupMembers(groupId),
      });
      queryClient.invalidateQueries({ queryKey: companyKeys.group(groupId) });
      options?.onSuccess?.(...args);
    },
    onError: (...args) => {
      options?.onError?.(...args);
    },
  });
}
