import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import companyApiClient from "@workspace/api-client/company/company";

import { companyKeys } from "../keys";

type RemoveMemberParams = {
  /** ID организации — используется для инвалидации кэша. */
  orgId: string;
  memberId: string;
};

/** Удаляет участника из текущей организации (X-OrganizationId-Id из localStorage). */
export default function useRemoveMember(
  options?: UseMutationOptions<void, Error, RemoveMemberParams>,
) {
  const queryClient = useQueryClient();

  return useMutation({
    ...options,
    mutationFn: ({ memberId }) => companyApiClient.removeMember(memberId),
    onSuccess: (...args) => {
      const { orgId } = args[1];
      queryClient.invalidateQueries({ queryKey: companyKeys.members(orgId) });
      options?.onSuccess?.(...args);
    },
    onError: (...args) => {
      options?.onError?.(...args);
    },
  });
}
