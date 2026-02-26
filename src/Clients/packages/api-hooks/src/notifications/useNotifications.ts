import type { UseQueryOptions } from "@tanstack/react-query";
import { useQuery } from "@tanstack/react-query";

import notificationApiClient from "@workspace/api-client/notifications/notifications";
import type { GetNotificationsParams } from "@workspace/api-client/notifications/notifications";
import type { Notification } from "@workspace/types/notifications";
import type { PagedResult } from "@workspace/types/shared";

import { notificationKeys } from "../keys";

export default function useNotifications(
  params: GetNotificationsParams = {},
  options?: Omit<UseQueryOptions<PagedResult<Notification>>, "queryKey" | "queryFn">,
) {
  return useQuery({
    queryKey: notificationKeys.list(params),
    queryFn: () => notificationApiClient.getNotifications(params),
    ...options,
  });
}
