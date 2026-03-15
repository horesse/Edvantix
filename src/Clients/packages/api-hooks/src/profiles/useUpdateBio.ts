import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import profileApiClient from "@workspace/api-client/profile/profiles";
import type {
  OwnProfileDetails,
  UpdateBioRequest,
} from "@workspace/types/profile";

import { profileKeys } from "../keys";

export default function useUpdateBio(
  options?: UseMutationOptions<OwnProfileDetails, Error, UpdateBioRequest>,
) {
  const queryClient = useQueryClient();

  return useMutation({
    ...options,
    mutationFn: (request) => profileApiClient.updateBio(request),
    onSuccess: (...args) => {
      queryClient.invalidateQueries({ queryKey: profileKeys.all });
      options?.onSuccess?.(...args);
    },
    onError: (...args) => {
      options?.onError?.(...args);
    },
  });
}
