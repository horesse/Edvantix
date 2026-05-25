import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import organizationApiClient from "@workspace/api-client/organization/organization";

import { organizationKeys } from "../keys";

type DeleteRoleParams = {
  /** ID организации — используется для инвалидации кэша. */
  orgId: string;
  roleId: string;
};

/** Удаляет роль из организации. */
export default function useDeleteRole(
  options?: UseMutationOptions<void, Error, DeleteRoleParams>,
) {
  const queryClient = useQueryClient();

  return useMutation({
    ...options,
    mutationFn: ({ roleId }) => organizationApiClient.deleteRole(roleId),
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
