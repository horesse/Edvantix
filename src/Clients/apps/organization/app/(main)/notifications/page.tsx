import type { Metadata } from "next";

import { NotificationsFeature } from "@/features/notifications/notifications-feature";

export const metadata: Metadata = {
  title: "Уведомления",
};

export default function NotificationsPage() {
  return <NotificationsFeature />;
}
