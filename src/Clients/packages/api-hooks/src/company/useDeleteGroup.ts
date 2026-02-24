import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import companyApiClient from "@workspace/api-client/company/company";

import { companyKeys } from "../keys";

type DeleteGroupParams = {
  id: string;
  orgId: string;
};

export default function useDeleteGroup(
  options?: UseMutationOptions<void, Error, DeleteGroupParams>,
) {
  const queryClient = useQueryClient();

  return useMutation({
    ...options,
    mutationFn: ({ id }) => companyApiClient.deleteGroup(id),
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
