import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import companyApiClient from "@workspace/api-client/company/company";
import type { UpdateOrganizationContactRequest } from "@workspace/types/company";

import { companyKeys } from "../keys";

type UpdateContactParams = {
  orgId: number;
  contactId: number;
  request: UpdateOrganizationContactRequest;
};

export default function useUpdateOrganizationContact(
  options?: UseMutationOptions<void, Error, UpdateContactParams>,
) {
  const queryClient = useQueryClient();

  return useMutation({
    ...options,
    mutationFn: ({ orgId, contactId, request }) =>
      companyApiClient.updateContact(orgId, contactId, request),
    onSuccess: (...args) => {
      const { orgId } = args[1];
      queryClient.invalidateQueries({ queryKey: companyKeys.contacts(orgId) });
      options?.onSuccess?.(...args);
    },
    onError: (...args) => {
      options?.onError?.(...args);
    },
  });
}
