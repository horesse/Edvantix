import type { ReactNode } from "react";

import {
  BarChart3,
  BookOpen,
  CreditCard,
  MessageSquare,
  Users,
  Calendar,
  Zap,
  Shield,
  type LucideIcon,
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
    <div className="mt-6 p-4 rounded-xl bg-background/60 border border-border">
      <div className="flex items-center justify-between mb-4">
        <span className="text-foreground text-xs font-medium">Выручка за месяц</span>
        <span className="text-emerald-500 text-xs font-medium">+24%</span>
      </div>
      <div className="flex items-end gap-1.5 h-20">
        {bars.map((h, i) => (
          <div key={i} className="flex-1 flex items-end">
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
      <div className="flex justify-between mt-2">
        {["01", "05", "10", "15", "20", "25", "29", "31"].map((d) => (
          <span key={d} className="text-muted-foreground text-[10px]">{d}</span>
        ))}
      </div>
      <div className="mt-4 grid grid-cols-3 gap-2">
        {[
          { label: "Сегодня", value: "₽18 400" },
          { label: "Неделя", value: "₽94 200" },
          { label: "Месяц", value: "₽284 000" },
        ].map((item) => (
          <div key={item.label} className="text-center">
            <div className="text-card-foreground text-sm font-semibold">{item.value}</div>
            <div className="text-muted-foreground text-[10px]">{item.label}</div>
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
      className="relative py-24 sm:py-32 bg-background"
    >
      {/* Subtle background accent */}
      <div
        className="absolute inset-0 pointer-events-none"
        aria-hidden="true"
        style={{
          background:
            "radial-gradient(ellipse 60% 40% at 80% 50%, color-mix(in oklch, var(--primary) 5%, transparent) 0%, transparent 60%)",
        }}
      />

      <div className="relative max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        {/* Section header */}
        <div className="text-center mb-16">
          <div className="inline-flex items-center gap-2 px-3 py-1 rounded-full border border-primary/20 bg-primary/5 text-primary text-xs font-medium mb-4">
            <Zap className="w-3 h-3" aria-hidden="true" />
            Возможности
          </div>
          <h2 className="text-3xl sm:text-4xl lg:text-5xl font-bold text-card-foreground mb-4 tracking-tight">
            Всё, что нужно для работы
            <span className="block text-primary">вашей школы</span>
          </h2>
          <p className="text-muted-foreground text-lg max-w-2xl mx-auto leading-relaxed">
            Единая экосистема вместо десяти разрозненных инструментов. Управляйте
            всем бизнесом из одного окна.
          </p>
        </div>

        {/* Bento grid */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {FEATURES.map((feature, index) => {
            const isWide = feature.span === "wide";

            return (
              <div
                key={feature.title}
                className={`group relative rounded-2xl border border-border bg-card/50 p-6 transition-all duration-300 hover:bg-card hover:shadow-xl card-glow ${
                  isWide ? "md:col-span-2 lg:col-span-2" : ""
                }`}
                style={{ animationDelay: `${index * 80}ms` }}
              >
                {/* Hover glow orb */}
                <div
                  className="absolute top-0 right-0 w-32 h-32 rounded-full blur-3xl opacity-0 group-hover:opacity-100 transition-opacity duration-500 bg-primary/10"
                  aria-hidden="true"
                />

                {/* Icon */}
                <div className="inline-flex p-2.5 rounded-xl bg-primary/10 mb-4">
                  <feature.icon
                    className="w-5 h-5 text-primary"
                    aria-hidden="true"
                  />
                </div>

                {/* Content */}
                <h3 className="text-card-foreground font-semibold text-lg mb-2 leading-tight">
                  {feature.title}
                </h3>
                <p className="text-muted-foreground text-sm leading-relaxed">
                  {feature.description}
                </p>

                {feature.preview}

                {/* Hover beam */}
                <div
                  className="absolute inset-0 rounded-2xl overflow-hidden pointer-events-none opacity-0 group-hover:opacity-100 transition-opacity duration-300"
                  aria-hidden="true"
                >
                  <div className="animate-beam absolute top-0 left-0 right-0 h-px bg-gradient-to-r from-transparent via-primary/40 to-transparent" />
                </div>
              </div>
            );
          })}
        </div>

        {/* Security badge */}
        <div className="mt-12 flex items-center justify-center gap-3 text-muted-foreground text-sm">
          <Shield className="w-4 h-4 text-emerald-500" aria-hidden="true" />
          <span>Соответствует требованиям 152-ФЗ о персональных данных</span>
        </div>
      </div>
    </section>
  );
}
