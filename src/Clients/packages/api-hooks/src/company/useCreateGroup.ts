import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import companyApiClient from "@workspace/api-client/company/company";
import type { CreateGroupRequest } from "@workspace/types/company";

import { companyKeys } from "../keys";

type CreateGroupParams = {
  orgId: string;
  request: CreateGroupRequest;
};

export default function useCreateGroup(
  options?: UseMutationOptions<number, Error, CreateGroupParams>,
) {
  const queryClient = useQueryClient();

  return useMutation({
    ...options,
    mutationFn: ({ orgId, request }) =>
      companyApiClient.createGroup(orgId, request),
    onSuccess: (...args) => {
      const { orgId } = args[1];
      queryClient.invalidateQueries({ queryKey: companyKeys.groups(orgId) });
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
