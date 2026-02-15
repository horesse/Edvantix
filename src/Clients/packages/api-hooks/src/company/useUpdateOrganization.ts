import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import companyApiClient from "@workspace/api-client/company/company";
import type { UpdateOrganizationRequest } from "@workspace/types/company";

import { companyKeys } from "../keys";

type UpdateOrganizationParams = {
  id: number;
  request: UpdateOrganizationRequest;
};

export default function useUpdateOrganization(
  options?: UseMutationOptions<void, Error, UpdateOrganizationParams>,
) {
  const queryClient = useQueryClient();

  return useMutation({
    ...options,
    mutationFn: ({ id, request }) =>
      companyApiClient.updateOrganization(id, request),
    onSuccess: (...args) => {
      const { id } = args[1];
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
