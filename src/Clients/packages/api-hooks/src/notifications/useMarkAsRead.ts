import { useMutation, useQueryClient } from "@tanstack/react-query";

import notificationApiClient from "@workspace/api-client/notifications/notifications";

import { notificationKeys } from "../keys";

/** Мутация для пометки одного уведомления как прочитанного. */
export default function useMarkAsRead() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => notificationApiClient.markAsRead(id),
    onSuccess: () => {
      // Инвалидируем списки и счётчик при успешном обновлении
      queryClient.invalidateQueries({ queryKey: notificationKeys.all });
    },
  });
}
