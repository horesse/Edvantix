import type { Metadata } from "next";

export const metadata: Metadata = {
  title: "Edvantix Admin — Настройки",
};

export default function Page() {
  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-foreground text-2xl font-bold tracking-tight">Настройки</h1>
        <p className="text-muted-foreground mt-1 text-sm">Конфигурация системы</p>
      </div>
      <div className="bg-card border-border rounded-xl border p-8 text-center">
        <p className="text-muted-foreground text-sm">В разработке</p>
      </div>
    </div>
  );
}
