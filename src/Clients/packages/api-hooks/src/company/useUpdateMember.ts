import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import companyApiClient from "@workspace/api-client/company/company";
import type { UpdateOrganizationMemberRequest } from "@workspace/types/company";

import { companyKeys } from "../keys";

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
      companyApiClient.updateMember(memberId, request),
    onSuccess: (...args) => {
      const { orgId, memberId } = args[1];
      queryClient.invalidateQueries({ queryKey: companyKeys.members(orgId) });
      queryClient.invalidateQueries({ queryKey: companyKeys.member(memberId) });
      options?.onSuccess?.(...args);
    },
    onError: (...args) => {
      options?.onError?.(...args);
    },
  });
}
