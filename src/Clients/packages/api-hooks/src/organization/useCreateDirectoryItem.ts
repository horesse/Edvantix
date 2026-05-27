import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import directoriesApiClient from "@workspace/api-client/organization/directories";

import { organizationKeys } from "../keys";

type CreateDirectoryItemParams<TReq> = {
  /** ID организации — используется для инвалидации кэша. */
  orgId: string;
  request: TReq;
};

/** Создаёт элемент в указанном справочнике. */
export default function useCreateDirectoryItem<TReq>(
  code: string,
  options?: UseMutationOptions<string, Error, CreateDirectoryItemParams<TReq>>,
) {
  const queryClient = useQueryClient();

  return useMutation({
    ...options,
    mutationFn: ({ request }) => directoriesApiClient.create<TReq>(code, request),
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
