import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import companyApiClient from "@workspace/api-client/company/company";

import { companyKeys } from "../keys";

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
    mutationFn: ({ roleId }) => companyApiClient.deleteRole(roleId),
    onSuccess: (...args) => {
      const { orgId } = args[1];
      queryClient.invalidateQueries({ queryKey: companyKeys.roles(orgId) });
      options?.onSuccess?.(...args);
    },
    onError: (...args) => {
      options?.onError?.(...args);
    },
  });
}
