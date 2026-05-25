import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import organizationApiClient from "@workspace/api-client/organization/organization";
import type { UpdateRoleRequest } from "@workspace/types/organization";

import { organizationKeys } from "../keys";

type UpdateRoleParams = {
  /** ID организации — используется для инвалидации кэша. */
  orgId: string;
  roleId: string;
  request: UpdateRoleRequest;
};

/** Обновляет название и описание роли. */
export default function useUpdateRole(
  options?: UseMutationOptions<void, Error, UpdateRoleParams>,
) {
  const queryClient = useQueryClient();

  return useMutation({
    ...options,
    mutationFn: ({ roleId, request }) =>
      organizationApiClient.updateRole(roleId, request),
    onSuccess: (...args) => {
      const { orgId, roleId } = args[1];
      queryClient.invalidateQueries({ queryKey: organizationKeys.roles(orgId) });
      queryClient.invalidateQueries({
        queryKey: organizationKeys.role(orgId, roleId),
      });
      options?.onSuccess?.(...args);
    },
    onError: (...args) => {
      options?.onError?.(...args);
    },
  });
}
