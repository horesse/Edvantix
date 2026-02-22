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
  metadataBase: new URL(env.NEXT_PUBLIC_APP_URL || "http://localhost:3002"),
  title: {
    default: "Edvantix Blog",
    template: "%s | Edvantix Blog",
  },
  description:
    "Latest news, product updates, and insights from the Edvantix team.",
  generator: "Next.js",
  applicationName: "Edvantix Blog",
  keywords: ["edvantix", "blog", "news", "education", "changelog", "updates"],
  authors: [{ name: "Edvantix Team" }],
  creator: "Edvantix",
  publisher: "Edvantix",
  formatDetection: {
    email: false,
    address: false,
    telephone: false,
  },
  openGraph: {
    type: "website",
    url: env.NEXT_PUBLIC_APP_URL || "http://localhost:3002",
    title: "Edvantix Blog",
    description:
      "Latest news, product updates, and insights from the Edvantix team.",
    siteName: "Edvantix",
    locale: "en_US",
  },
  twitter: {
    card: "summary_large_image",
    title: "Edvantix Blog",
    description:
      "Latest news, product updates, and insights from the Edvantix team.",
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
      <body className="bg-background text-foreground font-sans antialiased">
        <Providers>{children}</Providers>
      </body>
    </html>
  );
}
