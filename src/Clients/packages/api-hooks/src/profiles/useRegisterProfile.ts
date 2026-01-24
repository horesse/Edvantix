import type { UseMutationOptions } from "@tanstack/react-query";
import { useMutation, useQueryClient } from "@tanstack/react-query";

import profileApiClient from "@workspace/api-client/profile/profiles";
import type { RegisterProfileRequest } from "@workspace/types/profile";

import { profileKeys } from "../keys";

export default function useRegisterProfile(
  options?: UseMutationOptions<number, Error, RegisterProfileRequest>,
) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request) => profileApiClient.registerProfile(request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: profileKeys.profile() });
    },
    ...options,
  });
}
