import {
  type UseMutationOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import profileApiClient from "@workspace/api-client/profile/profiles";
import type {
  OwnProfileDetails,
  UpdateContactsRequest,
} from "@workspace/types/profile";

import { profileKeys } from "../keys";

export default function useUpdateContacts(
  options?: UseMutationOptions<OwnProfileDetails, Error, UpdateContactsRequest>,
) {
  const queryClient = useQueryClient();

  return useMutation({
    ...options,
    mutationFn: (request) => profileApiClient.updateContacts(request),
    onSuccess: (...args) => {
      queryClient.invalidateQueries({ queryKey: profileKeys.all });
      options?.onSuccess?.(...args);
    },
    onError: (...args) => {
      options?.onError?.(...args);
    },
  });
}
