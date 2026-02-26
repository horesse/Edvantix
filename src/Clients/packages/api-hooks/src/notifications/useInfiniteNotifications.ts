import { useInfiniteQuery } from "@tanstack/react-query";

import notificationApiClient from "@workspace/api-client/notifications/notifications";
import { notificationKeys } from "@workspace/api-hooks/keys";

const PAGE_SIZE = 25;

export function useInfiniteNotifications(isRead: boolean | undefined) {
  return useInfiniteQuery({
    queryKey: notificationKeys.list({ isRead }),
    queryFn: ({ pageParam }) =>
      notificationApiClient.getNotifications({
        pageIndex: pageParam,
        pageSize: PAGE_SIZE,
        isRead,
      }),
    initialPageParam: 1,
    getNextPageParam: (lastPage, allPages) => {
      const loaded = allPages.reduce((acc, p) => acc + p.items.length, 0);

      return loaded < lastPage.totalCount ? allPages.length + 1 : undefined;
    },
  });
}
