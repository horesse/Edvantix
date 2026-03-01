import { useMutation, useQueryClient } from "@tanstack/react-query";

import notificationApiClient from "@workspace/api-client/notifications/notifications";

import { notificationKeys } from "../keys";

/** Мутация для пометки всех уведомлений пользователя как прочитанных. */
export default function useMarkAllAsRead() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: () => notificationApiClient.markAllAsRead(),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: notificationKeys.all });
    },
  });
}
