import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import directoriesApiClient from "@workspace/api-client/organization/directories";

import { organizationKeys } from "../keys";

type RestoreDirectoryItemParams = {
  /** ID организации — используется для инвалидации кэша. */
  orgId: string;
  id: string;
};

/** Восстанавливает элемент справочника из архива. */
export default function useRestoreDirectoryItem(
  code: string,
  options?: UseMutationOptions<void, Error, RestoreDirectoryItemParams>,
) {
  const queryClient = useQueryClient();

  return useMutation({
    ...options,
    mutationFn: ({ id }) => directoriesApiClient.restore(code, id),
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
