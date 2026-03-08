import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import profileApiClient from "@workspace/api-client/profile/profiles";
import type {
  OwnProfileDetails,
  UpdateEmploymentRequest,
} from "@workspace/types/profile";

import { profileKeys } from "../keys";

export default function useUpdateEmployment(
  options?: UseMutationOptions<
    OwnProfileDetails,
    Error,
    UpdateEmploymentRequest
  >,
) {
  const queryClient = useQueryClient();

  return useMutation({
    ...options,
    mutationFn: (request) => profileApiClient.updateEmployment(request),
    onSuccess: (...args) => {
      queryClient.invalidateQueries({ queryKey: profileKeys.all });
      options?.onSuccess?.(...args);
    },
    onError: (...args) => {
      options?.onError?.(...args);
    },
  });
}
