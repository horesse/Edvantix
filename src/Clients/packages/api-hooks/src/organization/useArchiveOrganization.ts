import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import organizationApiClient from "@workspace/api-client/organization/organization";

import { organizationKeys } from "../keys";

export default function useArchiveOrganization(
  options?: UseMutationOptions<void, Error, string>,
) {
  const queryClient = useQueryClient();

  return useMutation({
    ...options,
    mutationFn: (id: string) => organizationApiClient.archiveOrganization(id),
    onSuccess: (...args) => {
      const id = args[1];
      queryClient.invalidateQueries({ queryKey: organizationKeys.organization(id) });
      queryClient.invalidateQueries({
        queryKey: organizationKeys.myOrganizations(),
      });
      options?.onSuccess?.(...args);
    },
    onError: (...args) => {
      options?.onError?.(...args);
    },
  });
}
