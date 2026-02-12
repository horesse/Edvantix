import type React from "react";

import type { Metadata } from "next";

import { Separator } from "@workspace/ui/components/separator";

import { SettingsNav } from "@/features/settings/settings-nav";

export const metadata: Metadata = {
  title: "Настройки",
  description: "Управление настройками вашего аккаунта",
};

export default function SettingsLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <div className="mx-auto w-full max-w-4xl space-y-6">
      <div>
        <h1 className="text-2xl font-bold tracking-tight">Настройки</h1>
        <p className="text-muted-foreground text-sm">
          Управляйте настройками вашего аккаунта и профиля
        </p>
      </div>
      <Separator />
      <div className="flex flex-col gap-6 sm:gap-8 lg:flex-row">
        <aside className="w-full shrink-0 lg:w-44">
          <div className="lg:sticky lg:top-4">
            <SettingsNav />
          </div>
        </aside>
        <div className="min-w-0 flex-1">{children}</div>
      </div>
    </div>
  );
}
