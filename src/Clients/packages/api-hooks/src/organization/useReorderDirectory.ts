import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import directoriesApiClient from "@workspace/api-client/organization/directories";
import type { ReorderDirectoryRequest } from "@workspace/types/organization";

import { organizationKeys } from "../keys";

type ReorderDirectoryParams = {
  /** ID организации — используется для инвалидации кэша. */
  orgId: string;
  request: ReorderDirectoryRequest;
};

/** Переупорядочивает элементы справочника. */
export default function useReorderDirectory(
  code: string,
  options?: UseMutationOptions<void, Error, ReorderDirectoryParams>,
) {
  const queryClient = useQueryClient();

  return useMutation({
    ...options,
    mutationFn: ({ request }) => directoriesApiClient.reorder(code, request),
    onSuccess: (...args) => {
      const { orgId } = args[1];
      queryClient.invalidateQueries({
        queryKey: organizationKeys.directoryList(orgId, code),
      });
      options?.onSuccess?.(...args);
    },
    onError: (...args) => {
      options?.onError?.(...args);
    },
  });
}
