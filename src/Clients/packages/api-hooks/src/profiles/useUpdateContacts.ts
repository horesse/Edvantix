import { type UseMutationOptions, useQueryClient } from "@tanstack/react-query";

import type { OwnProfile, UpdateProfileRequest } from "@workspace/types/profile";

import { profileKeys } from "../keys";
import useUpdateProfile from "./useUpdateProfile";

export default function useUpdateContacts(
  options?: UseMutationOptions<OwnProfile, Error, UpdateProfileRequest>,
) {
  const queryClient = useQueryClient();
  const mutation = useUpdateProfile({
    ...options,
    onSuccess: (...args) => {
      queryClient.invalidateQueries({ queryKey: profileKeys.all });
      options?.onSuccess?.(...args);
    },
  });

  return mutation;
}
