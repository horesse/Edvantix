import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import companyApiClient from "@workspace/api-client/company/company";
import type { CreateRoleRequest } from "@workspace/types/company";

import { companyKeys } from "../keys";

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
    mutationFn: ({ request }) => companyApiClient.createRole(request),
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
