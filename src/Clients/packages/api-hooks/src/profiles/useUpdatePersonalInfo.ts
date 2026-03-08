import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import profileApiClient from "@workspace/api-client/profile/profiles";
import type {
  OwnProfileDetails,
  UpdatePersonalInfoRequest,
} from "@workspace/types/profile";

import { profileKeys } from "../keys";

export default function useUpdatePersonalInfo(
  options?: UseMutationOptions<
    OwnProfileDetails,
    Error,
    UpdatePersonalInfoRequest
  >,
) {
  const queryClient = useQueryClient();

  return useMutation({
    ...options,
    mutationFn: (request) => profileApiClient.updatePersonalInfo(request),
    onSuccess: (...args) => {
      queryClient.invalidateQueries({ queryKey: profileKeys.all });
      options?.onSuccess?.(...args);
    },
    onError: (...args) => {
      options?.onError?.(...args);
    },
  });
}
