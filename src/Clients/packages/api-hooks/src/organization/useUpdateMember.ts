import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import organizationApiClient from "@workspace/api-client/organization/organization";
import type { UpdateOrganizationMemberRequest } from "@workspace/types/organization";

import { organizationKeys } from "../keys";

type UpdateMemberParams = {
  memberId: string;
  /** ID организации — используется для инвалидации кэша. */
  orgId: string;
  request: UpdateOrganizationMemberRequest;
};

/** Обновляет роль участника в текущей организации (X-OrganizationId-Id из localStorage). */
export default function useUpdateMember(
  options?: UseMutationOptions<void, Error, UpdateMemberParams>,
) {
  const queryClient = useQueryClient();

  return useMutation({
    ...options,
    mutationFn: ({ memberId, request }) =>
      organizationApiClient.updateMember(memberId, request),
    onSuccess: (...args) => {
      const { orgId, memberId } = args[1];
      queryClient.invalidateQueries({ queryKey: organizationKeys.members(orgId) });
      queryClient.invalidateQueries({ queryKey: organizationKeys.member(memberId) });
      options?.onSuccess?.(...args);
    },
    onError: (...args) => {
      options?.onError?.(...args);
    },
  });
}
