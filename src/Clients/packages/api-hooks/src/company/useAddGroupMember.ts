import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import companyApiClient from "@workspace/api-client/company/company";
import type { AddGroupMemberRequest } from "@workspace/types/company";

import { companyKeys } from "../keys";

type AddGroupMemberParams = {
  groupId: string;
  request: AddGroupMemberRequest;
};

export default function useAddGroupMember(
  options?: UseMutationOptions<void, Error, AddGroupMemberParams>,
) {
  const queryClient = useQueryClient();

  return useMutation({
    ...options,
    mutationFn: ({ groupId, request }) =>
      companyApiClient.addGroupMember(groupId, request),
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
