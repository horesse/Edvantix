import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import organizationApiClient from "@workspace/api-client/organization/organization";

import { organizationKeys } from "../keys";

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
    mutationFn: ({ memberId }) => organizationApiClient.removeMember(memberId),
    onSuccess: (...args) => {
      const { orgId } = args[1];
      queryClient.invalidateQueries({ queryKey: organizationKeys.members(orgId) });
      options?.onSuccess?.(...args);
    },
    onError: (...args) => {
      options?.onError?.(...args);
    },
  });
}
