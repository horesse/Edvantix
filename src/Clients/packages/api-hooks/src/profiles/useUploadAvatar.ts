import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import profileApiClient from "@workspace/api-client/profile/profiles";

import { profileKeys } from "../keys";

export default function useUploadAvatar(
  options?: UseMutationOptions<void, Error, File>,
) {
  const queryClient = useQueryClient();

  return useMutation({
    ...options,
    mutationFn: (file) => profileApiClient.uploadAvatar(file),
    onSuccess: (...args) => {
      queryClient.invalidateQueries({ queryKey: profileKeys.all });
      options?.onSuccess?.(...args);
    },
    onError: (...args) => {
      options?.onError?.(...args);
    },
  });
}
