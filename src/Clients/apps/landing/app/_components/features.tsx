import type { ReactNode } from "react";

import {
  BarChart3,
  BookOpen,
  Calendar,
  CreditCard,
  type LucideIcon,
  MessageSquare,
  Shield,
  Users,
  Zap,
} from "lucide-react";

interface Feature {
  icon: LucideIcon;
  title: string;
  description: string;
  span?: "wide" | "normal";
  preview?: ReactNode;
}

const FEATURES: Feature[] = [
  {
    icon: BarChart3,
    title: "Аналитика и отчёты",
    description:
      "Понимайте, как растёт ваша школа. Конверсии, удержание студентов, выручка — всё в одном дашборде.",
    span: "wide",
    preview: <AnalyticsPreview />,
  },
  {
    icon: BookOpen,
    title: "Управление курсами",
    description:
      "Создавайте курсы с модулями, уроками, тестами и домашними заданиями без единой строки кода.",
  },
  {
    icon: Users,
    title: "База студентов",
    description:
      "Профили, прогресс, история покупок и коммуникация — вся информация о студентах в одном месте.",
  },
  {
    icon: CreditCard,
    title: "Финансы и оплаты",
    description:
      "Выставляйте счета, принимайте оплату, управляйте подписками и отслеживайте выручку в реальном времени.",
  },
  {
    icon: MessageSquare,
    title: "Коммуникация",
    description:
      "Email-рассылки, push-уведомления, внутренний чат — оставайтесь на связи со студентами.",
  },
  {
    icon: Calendar,
    title: "Расписание",
    description:
      "Планируйте живые вебинары, отправляйте приглашения и напоминания автоматически.",
  },
];

function AnalyticsPreview() {
  const bars = [35, 55, 45, 75, 60, 90, 70, 85];

  return (
    <div className="bg-background/60 border-border mt-6 rounded-xl border p-4">
      <div className="mb-4 flex items-center justify-between">
        <span className="text-foreground text-xs font-medium">
          Выручка за месяц
        </span>
        <span className="text-xs font-medium text-emerald-500">+24%</span>
      </div>
      <div className="flex h-20 items-end gap-1.5">
        {bars.map((h, i) => (
          <div key={i} className="flex flex-1 items-end">
            <div
              className="w-full rounded-sm"
              style={{
                height: `${h}%`,
                background:
                  i === bars.length - 1
                    ? "linear-gradient(to top, var(--primary), color-mix(in oklch, var(--primary) 60%, white))"
                    : i === bars.length - 2
                      ? "color-mix(in oklch, var(--primary) 45%, transparent)"
                      : "color-mix(in oklch, var(--primary) 20%, transparent)",
              }}
            />
          </div>
        ))}
      </div>
      <div className="mt-2 flex justify-between">
        {["01", "05", "10", "15", "20", "25", "29", "31"].map((d) => (
          <span key={d} className="text-muted-foreground text-[10px]">
            {d}
          </span>
        ))}
      </div>
      <div className="mt-4 grid grid-cols-3 gap-2">
        {[
          { label: "Сегодня", value: "₽18 400" },
          { label: "Неделя", value: "₽94 200" },
          { label: "Месяц", value: "₽284 000" },
        ].map((item) => (
          <div key={item.label} className="text-center">
            <div className="text-card-foreground text-sm font-semibold">
              {item.value}
            </div>
            <div className="text-muted-foreground text-[10px]">
              {item.label}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}

export function Features() {
  return (
    <section
      id="features"
      aria-label="Возможности платформы"
      className="bg-background relative py-24 sm:py-32"
    >
      {/* Subtle background accent */}
      <div
        className="pointer-events-none absolute inset-0"
        aria-hidden="true"
        style={{
          background:
            "radial-gradient(ellipse 60% 40% at 80% 50%, color-mix(in oklch, var(--primary) 5%, transparent) 0%, transparent 60%)",
        }}
      />

      <div className="relative mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
        {/* Section header */}
        <div className="mb-16 text-center">
          <div className="border-primary/20 bg-primary/5 text-primary mb-4 inline-flex items-center gap-2 rounded-full border px-3 py-1 text-xs font-medium">
            <Zap className="h-3 w-3" aria-hidden="true" />
            Возможности
          </div>
          <h2 className="text-card-foreground mb-4 text-3xl font-bold tracking-tight sm:text-4xl lg:text-5xl">
            Всё, что нужно для работы
            <span className="text-primary block">вашей школы</span>
          </h2>
          <p className="text-muted-foreground mx-auto max-w-2xl text-lg leading-relaxed">
            Единая экосистема вместо десяти разрозненных инструментов.
            Управляйте всем бизнесом из одного окна.
          </p>
        </div>

        {/* Bento grid */}
        <div className="grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-3">
          {FEATURES.map((feature, index) => {
            const isWide = feature.span === "wide";

            return (
              <div
                key={feature.title}
                className={`group border-border bg-card/50 hover:bg-card card-glow relative rounded-2xl border p-6 transition-all duration-300 hover:shadow-xl ${
                  isWide ? "md:col-span-2 lg:col-span-2" : ""
                }`}
                style={{ animationDelay: `${index * 80}ms` }}
              >
                {/* Hover glow orb */}
                <div
                  className="bg-primary/10 absolute top-0 right-0 h-32 w-32 rounded-full opacity-0 blur-3xl transition-opacity duration-500 group-hover:opacity-100"
                  aria-hidden="true"
                />

                {/* Icon */}
                <div className="bg-primary/10 mb-4 inline-flex rounded-xl p-2.5">
                  <feature.icon
                    className="text-primary h-5 w-5"
                    aria-hidden="true"
                  />
                </div>

                {/* Content */}
                <h3 className="text-card-foreground mb-2 text-lg leading-tight font-semibold">
                  {feature.title}
                </h3>
                <p className="text-muted-foreground text-sm leading-relaxed">
                  {feature.description}
                </p>

                {feature.preview}

                {/* Hover beam */}
                <div
                  className="pointer-events-none absolute inset-0 overflow-hidden rounded-2xl opacity-0 transition-opacity duration-300 group-hover:opacity-100"
                  aria-hidden="true"
                >
                  <div className="animate-beam via-primary/40 absolute top-0 right-0 left-0 h-px bg-gradient-to-r from-transparent to-transparent" />
                </div>
              </div>
            );
          })}
        </div>

        {/* Security badge */}
        <div className="text-muted-foreground mt-12 flex items-center justify-center gap-3 text-sm">
          <Shield className="h-4 w-4 text-emerald-500" aria-hidden="true" />
          <span>Соответствует требованиям 152-ФЗ о персональных данных</span>
        </div>
      </div>
    </section>
  );
}
