import type { Metadata } from "next";

import { env } from "@/env.mjs";
import { Dashboard } from "@/features/dashboard/dashboard";

export const metadata: Metadata = {
  title: "Edvantix - Главная",
  description:
    "Современная система управления образовательными учреждениями. Управление учениками, преподавателями, расписанием и учебным процессом.",
  keywords: [
    "управление школой",
    "образовательная система",
    "школьный менеджер",
    "учебный процесс",
    "расписание",
  ],
  openGraph: {
    type: "website",
    title: "Edvantix - Онлайн менеджер школ",
    description:
      "Современная система управления образовательными учреждениями.",
    url: env.NEXT_PUBLIC_APP_URL || "http://localhost:3000",
    siteName: "Edvantix",
    locale: "ru_RU",
  },
  twitter: {
    card: "summary_large_image",
    title: "Edvantix - Онлайн менеджер школ",
    description:
      "Современная система управления образовательными учреждениями.",
  },
  alternates: {
    canonical: env.NEXT_PUBLIC_APP_URL || "http://localhost:3000",
  },
  robots: {
    index: true,
    follow: true,
    googleBot: {
      index: true,
      follow: true,
      "max-video-preview": -1,
      "max-image-preview": "large",
      "max-snippet": -1,
    },
  },
};

export default function HomePage() {
  return <Dashboard />;
}
