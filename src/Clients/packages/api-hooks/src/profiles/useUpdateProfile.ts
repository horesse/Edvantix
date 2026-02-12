import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import profileApiClient from "@workspace/api-client/profile/profiles";
import type { UpdateProfileRequest } from "@workspace/types/profile";

import { profileKeys } from "../keys";

export default function useUpdateProfile(
  options?: UseMutationOptions<void, Error, UpdateProfileRequest>,
) {
  const queryClient = useQueryClient();

  return useMutation({
    ...options,
    mutationFn: (request) => profileApiClient.updateProfile(request),
    onSuccess: (...args) => {
      queryClient.invalidateQueries({ queryKey: profileKeys.all });
      options?.onSuccess?.(...args);
    },
    onError: (...args) => {
      options?.onError?.(...args);
    },
  });
}
