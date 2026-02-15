import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import companyApiClient from "@workspace/api-client/company/company";

import { companyKeys } from "../keys";

export default function useAcceptInvitation(
  options?: UseMutationOptions<void, Error, string>,
) {
  const queryClient = useQueryClient();

  return useMutation({
    ...options,
    mutationFn: (token) => companyApiClient.acceptInvitation(token),
    onSuccess: (...args) => {
      queryClient.invalidateQueries({ queryKey: companyKeys.myInvitations() });
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
