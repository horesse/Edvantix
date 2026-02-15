import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import companyApiClient from "@workspace/api-client/company/company";
import type { UpdateGroupRequest } from "@workspace/types/company";

import { companyKeys } from "../keys";

type UpdateGroupParams = {
  id: number;
  orgId: number;
  request: UpdateGroupRequest;
};

export default function useUpdateGroup(
  options?: UseMutationOptions<void, Error, UpdateGroupParams>,
) {
  const queryClient = useQueryClient();

  return useMutation({
    ...options,
    mutationFn: ({ id, request }) => companyApiClient.updateGroup(id, request),
    onSuccess: (...args) => {
      const { id, orgId } = args[1];
      queryClient.invalidateQueries({ queryKey: companyKeys.group(id) });
      queryClient.invalidateQueries({ queryKey: companyKeys.groups(orgId) });
      options?.onSuccess?.(...args);
    },
    onError: (...args) => {
      options?.onError?.(...args);
    },
  });
}
