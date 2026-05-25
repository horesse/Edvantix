import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import organizationApiClient from "@workspace/api-client/organization/organization";
import type { CreateOrganizationMemberRequest } from "@workspace/types/organization";

import { organizationKeys } from "../keys";

type AddMemberParams = {
  /** ID организации — используется для инвалидации кэша. */
  orgId: string;
  request: CreateOrganizationMemberRequest;
};

/** Добавляет участника в текущую организацию (X-OrganizationId-Id из localStorage). */
export default function useAddMember(
  options?: UseMutationOptions<string, Error, AddMemberParams>,
) {
  const queryClient = useQueryClient();

  return useMutation({
    ...options,
    mutationFn: ({ request }) => organizationApiClient.addMember(request),
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
