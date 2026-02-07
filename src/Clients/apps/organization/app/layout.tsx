import type React from "react";

import type { Metadata, Viewport } from "next";
import { Geist, Geist_Mono } from "next/font/google";

import "@workspace/ui/globals.css";

import { env } from "@/env.mjs";

import { Providers } from "./providers";

export const viewport: Viewport = {
  width: "device-width",
  initialScale: 1,
  maximumScale: 5,
  userScalable: true,
  themeColor: [
    { media: "(prefers-color-scheme: light)", color: "#fbf8f1" },
    { media: "(prefers-color-scheme: dark)", color: "#1a1a1a" },
  ],
};

export const metadata: Metadata = {
  metadataBase: new URL(env.NEXT_PUBLIC_APP_URL || "http://localhost:3000"),
  title: {
    default: "Edvantix - Онлайн менеджер школ",
    template: "%s | Edvantix",
  },
  description:
    "Современная система управления образовательными учреждениями. Управление учениками, преподавателями, расписанием и учебным процессом.",
  generator: "Next.js",
  applicationName: "Edvantix",
  keywords: [
    "управление школой",
    "образовательная система",
    "школьный менеджер",
    "учебный процесс",
    "расписание",
    "ученики",
    "преподаватели",
  ],
  authors: [{ name: "Edvantix Team" }],
  creator: "Edvantix",
  publisher: "Edvantix",
  formatDetection: {
    email: false,
    address: false,
    telephone: false,
  },
  manifest: "/manifest.json",
  openGraph: {
    type: "website",
    url: env.NEXT_PUBLIC_APP_URL || "http://localhost:3000",
    title: "Edvantix - Онлайн менеджер школ",
    description:
      "Современная система управления образовательными учреждениями.",
    siteName: "Edvantix",
    locale: "ru_RU",
  },
  twitter: {
    card: "summary_large_image",
    title: "Edvantix - Онлайн менеджер школ",
    description:
      "Современная система управления образовательными учреждениями.",
  },
  icons: {
    icon: [
      {
        url: "/favicon.svg",
        type: "image/svg+xml",
      },
      {
        url: "/favicon-96x96.png",
        sizes: "96x96",
        type: "image/png",
      },
      {
        url: "/favicon.ico",
        sizes: "any",
      },
    ],
    apple: "/apple-touch-icon.png",
  },
  appleWebApp: {
    capable: true,
    statusBarStyle: "black-translucent",
    title: "Edvantix",
  },
};

const _geist = Geist({ subsets: ["latin"] });
const _geistMono = Geist_Mono({ subsets: ["latin"] });

export default async function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en" suppressHydrationWarning>
      <body className={`font-sans antialiased`}>
        <div id="main-content">
          <Providers>{children}</Providers>
        </div>
      </body>
    </html>
  );
}
