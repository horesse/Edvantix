import type {
  Notification,
  UnreadCountResponse,
} from "@workspace/types/notifications";
import type { PagedResult } from "@workspace/types/shared";

import { apiClient } from "../client";
import type ApiClient from "../client";

/** Параметры запроса списка уведомлений. */
export type GetNotificationsParams = {
  pageIndex?: number;
  pageSize?: number;
  isRead?: boolean;
};

class NotificationApiClient {
  private readonly client: ApiClient;

  constructor() {
    this.client = apiClient;
  }

  /** Получает страницу уведомлений текущего пользователя. */
  public async getNotifications(
    params: GetNotificationsParams = {},
  ): Promise<PagedResult<Notification>> {
    const { pageIndex = 1, pageSize = 20, isRead } = params;
    const query = new URLSearchParams({
      pageIndex: String(pageIndex),
      pageSize: String(pageSize),
    });

    if (isRead !== undefined) {
      query.set("isRead", String(isRead));
    }

    const response = await this.client.get<PagedResult<Notification>>(
      `/notification/api/v1/notifications?${query.toString()}`,
    );

    return response.data;
  }

  /** Возвращает количество непрочитанных уведомлений (для бейджа). */
  public async getUnreadCount(): Promise<UnreadCountResponse> {
    const response = await this.client.get<UnreadCountResponse>(
      `/notification/api/v1/notifications/unread-count`,
    );

    return response.data;
  }

  /** Помечает одно уведомление как прочитанное. */
  public async markAsRead(id: string): Promise<void> {
    await this.client.post<void>(
      `/notification/api/v1/notifications/${id}/read`,
    );
  }

  /** Помечает все уведомления пользователя как прочитанные. */
  public async markAllAsRead(): Promise<void> {
    await this.client.post<void>(`/notification/api/v1/notifications/read-all`);
  }
}

export default new NotificationApiClient();
