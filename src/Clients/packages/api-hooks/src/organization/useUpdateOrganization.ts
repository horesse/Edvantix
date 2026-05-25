import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import organizationApiClient from "@workspace/api-client/organization/organization";
import type { UpdateOrganizationRequest } from "@workspace/types/organization";

import { organizationKeys } from "../keys";

type UpdateOrganizationParams = {
  id: string;
  request: UpdateOrganizationRequest;
};

export default function useUpdateOrganization(
  options?: UseMutationOptions<void, Error, UpdateOrganizationParams>,
) {
  const queryClient = useQueryClient();

  return useMutation({
    ...options,
    mutationFn: ({ id, request }) =>
      organizationApiClient.updateOrganization(id, request),
    onSuccess: (...args) => {
      const { id } = args[1];
      queryClient.invalidateQueries({
        queryKey: organizationKeys.organization(id),
      });
      queryClient.invalidateQueries({
        queryKey: organizationKeys.organizations(),
      });
      options?.onSuccess?.(...args);
    },
    onError: (...args) => {
      options?.onError?.(...args);
    },
  });
}
