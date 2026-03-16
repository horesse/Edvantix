import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import profileApiClient from "@workspace/api-client/profile/profiles";
import type { OwnProfileDetails } from "@workspace/types/profile";

import { profileKeys } from "../keys";

export default function useDeleteAvatar(
  options?: UseMutationOptions<OwnProfileDetails, Error, void>,
) {
  const queryClient = useQueryClient();

  return useMutation({
    ...options,
    mutationFn: () => profileApiClient.deleteAvatar(),
    onSuccess: (...args) => {
      queryClient.invalidateQueries({ queryKey: profileKeys.all });
      options?.onSuccess?.(...args);
    },
    onError: (...args) => {
      options?.onError?.(...args);
    },
  });
}
