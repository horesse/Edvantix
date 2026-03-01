/** Тип уведомления — определяет иконку и акцентный цвет. */
export const NotificationType = {
  Info: 0,
  Success: 1,
  Warning: 2,
  Error: 3,
  Invitation: 4,
  Achievement: 5,
  System: 6,
} as const;

export type NotificationType = (typeof NotificationType)[keyof typeof NotificationType];

/** DTO одного уведомления с сервера. */
export type Notification = {
  id: string;
  type: NotificationType;
  title: string;
  message: string;
  metadata: string | null;
  isRead: boolean;
  createdAt: string; // ISO 8601
  readAt: string | null;
};

/** Ответ на запрос количества непрочитанных. */
export type UnreadCountResponse = {
  count: number;
};
