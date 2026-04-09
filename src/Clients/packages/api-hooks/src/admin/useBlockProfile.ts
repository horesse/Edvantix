import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import adminApiClient from "@workspace/api-client/admin/admin";

import { adminKeys } from "../keys";

/** Blocks a profile and invalidates the profiles query cache on success. */
export default function useBlockProfile(
  options?: UseMutationOptions<void, Error, string>,
) {
  const queryClient = useQueryClient();

  return useMutation({
    ...options,
    mutationFn: (profileId: string) => adminApiClient.blockProfile(profileId),
    onSuccess: (...args) => {
      const [, profileId] = args;
      queryClient.invalidateQueries({ queryKey: adminKeys.profiles() });
      queryClient.invalidateQueries({
        queryKey: adminKeys.profile(profileId),
      });
      options?.onSuccess?.(...args);
    },
    onError: (...args) => {
      options?.onError?.(...args);
    },
  });
}
