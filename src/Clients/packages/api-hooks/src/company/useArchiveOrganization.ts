import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import companyApiClient from "@workspace/api-client/company/company";

import { companyKeys } from "../keys";

export default function useArchiveOrganization(
  options?: UseMutationOptions<void, Error, string>,
) {
  const queryClient = useQueryClient();

  return useMutation({
    ...options,
    mutationFn: (id: string) => companyApiClient.archiveOrganization(id),
    onSuccess: (...args) => {
      const id = args[1];
      queryClient.invalidateQueries({ queryKey: companyKeys.organization(id) });
      queryClient.invalidateQueries({
        queryKey: companyKeys.myOrganizations(),
      });
      options?.onSuccess?.(...args);
    },
    onError: (...args) => {
      options?.onError?.(...args);
    },
  });
}
