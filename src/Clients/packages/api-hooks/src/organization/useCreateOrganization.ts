import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import organizationApiClient from "@workspace/api-client/organization/organization";
import type { CreateOrganizationRequest } from "@workspace/types/organization";

import { organizationKeys } from "../keys";

export default function useCreateOrganization(
  options?: UseMutationOptions<string, Error, CreateOrganizationRequest>,
) {
  const queryClient = useQueryClient();

  return useMutation({
    ...options,
    mutationFn: (request) => organizationApiClient.createOrganization(request),
    onSuccess: (...args) => {
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
