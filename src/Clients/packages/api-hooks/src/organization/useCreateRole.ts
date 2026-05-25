import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import organizationApiClient from "@workspace/api-client/organization/organization";
import type { CreateRoleRequest } from "@workspace/types/organization";

import { organizationKeys } from "../keys";

type CreateRoleParams = {
  /** ID организации — используется для инвалидации кэша. */
  orgId: string;
  request: CreateRoleRequest;
};

/** Создаёт новую роль в текущей организации. */
export default function useCreateRole(
  options?: UseMutationOptions<string, Error, CreateRoleParams>,
) {
  const queryClient = useQueryClient();

  return useMutation({
    ...options,
    mutationFn: ({ request }) => organizationApiClient.createRole(request),
    onSuccess: (...args) => {
      const { orgId } = args[1];
      queryClient.invalidateQueries({ queryKey: organizationKeys.roles(orgId) });
      options?.onSuccess?.(...args);
    },
    onError: (...args) => {
      options?.onError?.(...args);
    },
  });
}
