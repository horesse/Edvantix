import type { UseQueryOptions } from "@tanstack/react-query";
import { useQuery } from "@tanstack/react-query";

import notificationApiClient from "@workspace/api-client/notifications/notifications";
import type { UnreadCountResponse } from "@workspace/types/notifications";

import { notificationKeys } from "../keys";

/**
 * Возвращает количество непрочитанных уведомлений.
 * По умолчанию обновляется каждые 30 секунд для актуального бейджа.
 */
export default function useUnreadCount(
  options?: Omit<
    UseQueryOptions<UnreadCountResponse>,
    "queryKey" | "queryFn"
  >,
) {
  return useQuery({
    queryKey: notificationKeys.unreadCount(),
    queryFn: () => notificationApiClient.getUnreadCount(),
    refetchInterval: 30_000,
    ...options,
  });
}
