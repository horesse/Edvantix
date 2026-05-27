import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import directoriesApiClient from "@workspace/api-client/organization/directories";

import { organizationKeys } from "../keys";

type ArchiveDirectoryItemParams = {
  /** ID организации — используется для инвалидации кэша. */
  orgId: string;
  id: string;
};

/** Переводит элемент справочника в архив. */
export default function useArchiveDirectoryItem(
  code: string,
  options?: UseMutationOptions<void, Error, ArchiveDirectoryItemParams>,
) {
  const queryClient = useQueryClient();

  return useMutation({
    ...options,
    mutationFn: ({ id }) => directoriesApiClient.archive(code, id),
    onSuccess: (...args) => {
      const { orgId } = args[1];
      queryClient.invalidateQueries({
        queryKey: organizationKeys.directory(orgId, code),
      });
      options?.onSuccess?.(...args);
    },
    onError: (...args) => {
      options?.onError?.(...args);
    },
  });
}
