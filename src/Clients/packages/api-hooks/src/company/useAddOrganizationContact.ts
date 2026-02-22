import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import companyApiClient from "@workspace/api-client/company/company";
import type { AddOrganizationContactRequest } from "@workspace/types/company";

import { companyKeys } from "../keys";

type AddContactParams = {
  orgId: string;
  request: AddOrganizationContactRequest;
};

export default function useAddOrganizationContact(
  options?: UseMutationOptions<string, Error, AddContactParams>,
) {
  const queryClient = useQueryClient();

  return useMutation({
    ...options,
    mutationFn: ({ orgId, request }) =>
      companyApiClient.addContact(orgId, request),
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
