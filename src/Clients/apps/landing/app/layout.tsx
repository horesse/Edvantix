import type React from "react";

import type { Metadata, Viewport } from "next";
import { Geist, Geist_Mono } from "next/font/google";

import "@workspace/ui/globals.css";

import "./landing.css";

/* Initialize Geist so Next.js optimizes it — same pattern as organization */
Geist({ subsets: ["latin"] });
Geist_Mono({ subsets: ["latin"] });

export const viewport: Viewport = {
  width: "device-width",
  initialScale: 1,
  maximumScale: 5,
  userScalable: true,
  themeColor: [
    { media: "(prefers-color-scheme: light)", color: "#fbf8f1" },
    { media: "(prefers-color-scheme: dark)", color: "#333333" },
  ],
};

export const metadata: Metadata = {
  title: "Edvantix — Управление онлайн-школой",
  description:
    "Единая платформа для управления онлайн-школой: студенты, курсы, финансы и аналитика. Запустите новую жизнь вашей школы за 15 минут.",
  keywords: ["онлайн-школа", "управление", "LMS", "EdTech", "платформа"],
  openGraph: {
    title: "Edvantix — Управление онлайн-школой",
    description: "Единая платформа для управления онлайн-школой без хаоса.",
    locale: "ru_RU",
    type: "website",
  },
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="ru" className="dark scroll-smooth">
      <body className="bg-background text-foreground font-sans antialiased">
        {children}
      </body>
    </html>
  );
}
