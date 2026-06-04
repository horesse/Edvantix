import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import directoriesApiClient from "@workspace/api-client/organization/directories";

import { organizationKeys } from "../keys";

type UpdateDirectoryItemParams<TReq> = {
  /** ID организации — используется для инвалидации кэша. */
  orgId: string;
  id: string;
  request: TReq;
};

/** Обновляет элемент в указанном справочнике. */
export default function useUpdateDirectoryItem<TReq>(
  code: string,
  options?: UseMutationOptions<void, Error, UpdateDirectoryItemParams<TReq>>,
) {
  const queryClient = useQueryClient();

  return useMutation({
    ...options,
    mutationFn: ({ id, request }) =>
      directoriesApiClient.update<TReq>(code, id, request),
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
