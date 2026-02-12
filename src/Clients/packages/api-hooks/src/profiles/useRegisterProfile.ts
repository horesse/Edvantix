import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import profileApiClient from "@workspace/api-client/profile/profiles";
import type { RegisterProfileRequest } from "@workspace/types/profile";

import { profileKeys } from "../keys";

export default function useRegisterProfile(
  options?: UseMutationOptions<number, Error, RegisterProfileRequest>,
) {
  const queryClient = useQueryClient();

  return useMutation({
    ...options,
    mutationFn: (request) => profileApiClient.registerProfile(request),
    onSuccess: (...args) => {
      queryClient.invalidateQueries({ queryKey: profileKeys.all });
      options?.onSuccess?.(...args);
    },
    onError: (...args) => {
      options?.onError?.(...args);
    },
  });
}
