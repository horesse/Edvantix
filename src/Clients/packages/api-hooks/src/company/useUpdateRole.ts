import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import companyApiClient from "@workspace/api-client/company/company";
import type { UpdateRoleRequest } from "@workspace/types/company";

import { companyKeys } from "../keys";

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
      companyApiClient.updateRole(roleId, request),
    onSuccess: (...args) => {
      const { orgId, roleId } = args[1];
      queryClient.invalidateQueries({ queryKey: companyKeys.roles(orgId) });
      queryClient.invalidateQueries({
        queryKey: companyKeys.role(orgId, roleId),
      });
      options?.onSuccess?.(...args);
    },
    onError: (...args) => {
      options?.onError?.(...args);
    },
  });
}
