import type React from "react";

import type { Metadata } from "next";

import { Separator } from "@workspace/ui/components/separator";

export const metadata: Metadata = {
  title: "Профиль",
  description: "Управление вашим профилем и персональными данными",
};

export default function SettingsLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <div className="mx-auto w-full max-w-2xl">
      <div className="mb-6">
        <h1 className="text-xl font-semibold tracking-tight">Профиль</h1>
        <p className="mt-1 text-sm text-muted-foreground">
          Управляйте вашим профилем и персональными данными
        </p>
      </div>
      <Separator className="mb-6 opacity-50" />
      {children}
    </div>
  );
}
