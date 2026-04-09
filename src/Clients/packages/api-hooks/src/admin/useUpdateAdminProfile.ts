import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import adminApiClient from "@workspace/api-client/admin/admin";
import type { AdminUpdateProfileRequest } from "@workspace/types/admin";

import { adminKeys } from "../keys";

type UpdateProfileParams = {
  profileId: string;
  request: AdminUpdateProfileRequest;
};

/** Updates a profile as admin and invalidates related caches on success. */
export default function useUpdateAdminProfile(
  options?: UseMutationOptions<void, Error, UpdateProfileParams>,
) {
  const queryClient = useQueryClient();

  return useMutation({
    ...options,
    mutationFn: ({ profileId, request }: UpdateProfileParams) =>
      adminApiClient.updateProfile(profileId, request),
    onSuccess: (...args) => {
      const [, variables] = args;
      queryClient.invalidateQueries({ queryKey: adminKeys.profiles() });
      queryClient.invalidateQueries({
        queryKey: adminKeys.profile(variables.profileId),
      });
      options?.onSuccess?.(...args);
    },
    onError: (...args) => {
      options?.onError?.(...args);
    },
  });
}
