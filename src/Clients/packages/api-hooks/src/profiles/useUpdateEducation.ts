import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import profileApiClient from "@workspace/api-client/profile/profiles";
import type { UpdateEducationRequest } from "@workspace/types/profile";

import { profileKeys } from "../keys";

export default function useUpdateEducation(
  options?: UseMutationOptions<void, Error, UpdateEducationRequest[]>,
) {
  const queryClient = useQueryClient();

  return useMutation({
    ...options,
    mutationFn: (educations) => profileApiClient.updateEducations(educations),
    onSuccess: (...args) => {
      queryClient.invalidateQueries({ queryKey: profileKeys.all });
      options?.onSuccess?.(...args);
    },
    onError: (...args) => {
      options?.onError?.(...args);
    },
  });
}
