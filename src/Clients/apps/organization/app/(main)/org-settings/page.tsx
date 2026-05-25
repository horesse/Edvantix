import { redirect } from "next/navigation";

/** Старый маршрут — перенаправляет на хаб настроек. */
export default function Page() {
  redirect("/organization/settings/organization");
}
