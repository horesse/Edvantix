import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import companyApiClient from "@workspace/api-client/company/company";
import type { CreateOrganizationRequest } from "@workspace/types/company";

import { companyKeys } from "../keys";

export default function useCreateOrganization(
  options?: UseMutationOptions<string, Error, CreateOrganizationRequest>,
) {
  const queryClient = useQueryClient();

  return useMutation({
    ...options,
    mutationFn: (request) => companyApiClient.createOrganization(request),
    onSuccess: (...args) => {
      queryClient.invalidateQueries({
        queryKey: companyKeys.organizations(),
      });
      options?.onSuccess?.(...args);
    },
    onError: (...args) => {
      options?.onError?.(...args);
    },
  });
}
