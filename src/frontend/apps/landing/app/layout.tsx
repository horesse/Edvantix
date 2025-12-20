import type { Metadata } from "next";
import "./globals.css";

export const metadata: Metadata = {
  title: "Edvantix - Управление онлайн школой",
  description: "Современная платформа для управления онлайн школой. Создавайте курсы, управляйте студентами и преподавателями, отслеживайте прогресс обучения.",
  keywords: ["онлайн школа", "образование", "управление курсами", "LMS", "edtech"],
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="ru">
      <body className="antialiased">
        {children}
      </body>
    </html>
  );
}
