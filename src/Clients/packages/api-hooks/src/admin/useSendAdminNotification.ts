import { type UseMutationOptions, useMutation } from "@tanstack/react-query";

import adminApiClient from "@workspace/api-client/admin/admin";
import type { SendAdminNotificationRequest } from "@workspace/types/admin";

type SendNotificationParams = {
  profileId: string;
  request: SendAdminNotificationRequest;
};

/** Sends a direct notification to a specific profile. */
export default function useSendAdminNotification(
  options?: UseMutationOptions<void, Error, SendNotificationParams>,
) {
  return useMutation({
    ...options,
    mutationFn: ({ profileId, request }) =>
      adminApiClient.sendNotification(profileId, request),
    onSuccess: (...args) => {
      options?.onSuccess?.(...args);
    },
    onError: (...args) => {
      options?.onError?.(...args);
    },
  });
}
