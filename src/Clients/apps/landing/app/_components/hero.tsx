"use client";

import { useEffect, useRef, useState } from "react";

import Link from "next/link";

import {
  ArrowRight,
  BarChart3,
  Bell,
  BookOpen,
  CheckCircle2,
  ChevronRight,
  Play,
  Settings,
  Star,
  TrendingUp,
  Users,
} from "lucide-react";

import { Button } from "@workspace/ui/components/button";

/* Approximate hex of --primary in dark mode: oklch(0.67 0.13 38.92) ≈ warm orange */
const PRIMARY_RGBA = (alpha: number) => `rgba(201, 107, 44, ${alpha})`;
/*
 * Analogous secondary — warm amber/gold (H≈70°, next to primary H≈39°).
 * Harmonises with orange; avoids the cool-blue contrast that felt jarring.
 */
const SECONDARY_RGBA = (alpha: number) => `rgba(251, 191, 36, ${alpha})`;

export function Hero() {
  const heroRef = useRef<HTMLDivElement>(null);
  const [mousePos, setMousePos] = useState({ x: 0, y: 0 });
  const [isLoaded, setIsLoaded] = useState(false);

  useEffect(() => {
    setIsLoaded(true);

    const handleMouseMove = (e: MouseEvent) => {
      if (heroRef.current) {
        const rect = heroRef.current.getBoundingClientRect();
        setMousePos({
          x: e.clientX - rect.left,
          y: e.clientY - rect.top,
        });
      }
    };

    const hero = heroRef.current;
    hero?.addEventListener("mousemove", handleMouseMove, { passive: true });

    return () => {
      hero?.removeEventListener("mousemove", handleMouseMove);
    };
  }, []);

  return (
    <section
      ref={heroRef}
      className="bg-background relative flex min-h-screen flex-col items-center justify-center overflow-hidden"
      aria-label="Главный экран"
    >
      {/* Dot grid */}
      <div
        className="dot-pattern absolute inset-0 opacity-60"
        aria-hidden="true"
      />

      {/* Aurora glow — primary + ring colors */}
      <div
        className="pointer-events-none absolute inset-0"
        aria-hidden="true"
        style={{
          background: [
            `radial-gradient(ellipse 90% 55% at 50% -10%, ${PRIMARY_RGBA(0.15)} 0%, transparent 60%)`,
            `radial-gradient(ellipse 50% 35% at 75% 60%, ${SECONDARY_RGBA(0.06)} 0%, transparent 55%)`,
            `radial-gradient(ellipse 40% 30% at 20% 70%, ${PRIMARY_RGBA(0.05)} 0%, transparent 50%)`,
          ].join(", "),
        }}
      />

      {/* Mouse spotlight */}
      <div
        className="pointer-events-none absolute inset-0"
        aria-hidden="true"
        style={{
          background: `radial-gradient(700px circle at ${mousePos.x}px ${mousePos.y}px, ${PRIMARY_RGBA(0.04)}, transparent 40%)`,
        }}
      />

      {/* Decorative rings */}
      <div
        className="border-primary/5 animate-spin-slow pointer-events-none absolute top-1/2 left-1/2 h-[600px] w-[600px] -translate-x-1/2 -translate-y-1/2 rounded-full border"
        aria-hidden="true"
      />
      <div
        className="border-primary/[0.03] animate-spin-slow pointer-events-none absolute top-1/2 left-1/2 h-[900px] w-[900px] -translate-x-1/2 -translate-y-1/2 rounded-full border"
        style={{ animationDirection: "reverse", animationDuration: "40s" }}
        aria-hidden="true"
      />

      {/* Content */}
      <div className="relative z-10 mx-auto flex max-w-7xl flex-col items-center px-4 pt-28 pb-16 text-center sm:px-6 lg:px-8">
        {/* Announcement badge */}
        <div
          className={`border-primary/25 bg-primary/8 text-primary mb-10 inline-flex items-center gap-2 rounded-full border px-4 py-1.5 text-sm transition-all duration-700 ${
            isLoaded ? "translate-y-0 opacity-100" : "translate-y-4 opacity-0"
          }`}
          style={{ transitionDelay: "100ms" }}
        >
          <span
            className="bg-primary h-1.5 w-1.5 animate-pulse rounded-full"
            aria-hidden="true"
          />
          <span>Новое: AI-аналитика и умные отчёты 2.0</span>
          <ChevronRight className="h-3.5 w-3.5 opacity-60" aria-hidden="true" />
        </div>

        {/* Headline */}
        <h1
          className={`mb-6 text-5xl leading-[0.95] font-extrabold tracking-tight transition-all duration-700 sm:text-6xl lg:text-7xl xl:text-8xl ${
            isLoaded ? "translate-y-0 opacity-100" : "translate-y-8 opacity-0"
          }`}
          style={{ transitionDelay: "200ms" }}
        >
          <span className="text-card-foreground block">Управляйте</span>
          <span className="text-shimmer block py-1">онлайн-школой</span>
          <span className="text-card-foreground block">без хаоса</span>
        </h1>

        {/* Subtitle */}
        <p
          className={`text-muted-foreground mb-10 max-w-2xl text-lg leading-relaxed transition-all duration-700 sm:text-xl ${
            isLoaded ? "translate-y-0 opacity-100" : "translate-y-6 opacity-0"
          }`}
          style={{ transitionDelay: "300ms" }}
        >
          Единая платформа для студентов, курсов, финансов и аналитики.
          Запустите новую жизнь вашей школы за&nbsp;15&nbsp;минут.
        </p>

        {/* CTA buttons */}
        <div
          className={`mb-10 flex flex-col gap-4 transition-all duration-700 sm:flex-row ${
            isLoaded ? "translate-y-0 opacity-100" : "translate-y-6 opacity-0"
          }`}
          style={{ transitionDelay: "400ms" }}
        >
          <Button
            asChild
            className="bg-primary hover:bg-primary/90 text-primary-foreground shadow-primary/25 hover:shadow-primary/40 focus-visible:ring-ring focus-visible:ring-offset-background h-12 px-8 text-base shadow-xl transition-all duration-300 hover:scale-[1.02] focus-visible:ring-2 focus-visible:ring-offset-2"
          >
            <Link href="/signup">
              Начать бесплатно
              <ArrowRight className="ml-1 h-4 w-4" aria-hidden="true" />
            </Link>
          </Button>
          <Button
            asChild
            variant="outline"
            className="border-border bg-muted/30 hover:bg-muted/60 text-foreground hover:border-border focus-visible:ring-ring h-12 px-8 text-base transition-all duration-300 focus-visible:ring-2"
          >
            <Link href="/demo">
              <Play className="text-primary mr-2 h-4 w-4" aria-hidden="true" />
              Смотреть демо
            </Link>
          </Button>
        </div>

        {/* Trust indicators */}
        <div
          className={`text-muted-foreground mb-24 flex flex-wrap justify-center gap-6 text-sm transition-all duration-700 ${
            isLoaded ? "translate-y-0 opacity-100" : "translate-y-4 opacity-0"
          }`}
          style={{ transitionDelay: "500ms" }}
        >
          {[
            "Без кредитной карты",
            "14 дней бесплатно",
            "Отмена в любое время",
          ].map((item) => (
            <span key={item} className="flex items-center gap-1.5">
              <CheckCircle2
                className="text-primary h-4 w-4 shrink-0"
                aria-hidden="true"
              />
              {item}
            </span>
          ))}
        </div>

        {/* Dashboard mockup */}
        <div
          className={`w-full max-w-5xl transition-all duration-1000 ${
            isLoaded ? "translate-y-0 opacity-100" : "translate-y-16 opacity-0"
          }`}
          style={{ transitionDelay: "600ms" }}
        >
          <DashboardMockup />
        </div>
      </div>

      {/* Bottom fade */}
      <div
        className="from-background pointer-events-none absolute right-0 bottom-0 left-0 h-40 bg-gradient-to-t to-transparent"
        aria-hidden="true"
      />
    </section>
  );
}

function DashboardMockup() {
  return (
    <div className="animate-float relative" style={{ animationDuration: "8s" }}>
      {/* Ambient glow */}
      <div
        className="animate-pulse-glow absolute -inset-8 rounded-[40px] opacity-15 blur-3xl"
        style={{
          background: `radial-gradient(ellipse, ${PRIMARY_RGBA(0.5)} 0%, ${SECONDARY_RGBA(0.15)} 60%, transparent 80%)`,
        }}
        aria-hidden="true"
      />

      {/* Browser window */}
      <div className="border-border bg-card relative overflow-hidden rounded-2xl border shadow-[0_32px_80px_rgba(0,0,0,0.4)]">
        {/* Title bar */}
        <div className="border-border bg-card flex items-center gap-2 border-b px-4 py-3">
          <div className="flex gap-1.5" aria-hidden="true">
            <div className="bg-destructive/70 h-3 w-3 rounded-full" />
            <div className="h-3 w-3 rounded-full bg-yellow-500/70" />
            <div className="h-3 w-3 rounded-full bg-emerald-500/70" />
          </div>
          <div className="mx-4 flex flex-1 justify-center">
            <div className="bg-muted flex h-5 w-52 items-center justify-center rounded-md">
              <span className="text-muted-foreground text-xs">
                app.edvantix.ru/dashboard
              </span>
            </div>
          </div>
          <div className="w-16" aria-hidden="true" />
        </div>

        {/* Dashboard */}
        <div className="flex h-[420px] sm:h-[460px]">
          {/* Sidebar */}
          <aside className="border-border bg-card hidden w-52 shrink-0 flex-col gap-1 border-r p-3 sm:flex">
            <div className="mb-4 flex items-center gap-2 px-3 py-2">
              <div className="bg-primary flex h-6 w-6 items-center justify-center rounded-md">
                <span className="text-primary-foreground text-xs font-bold">
                  E
                </span>
              </div>
              <span className="text-card-foreground text-sm font-semibold">
                Edvantix
              </span>
            </div>
            {[
              { icon: BarChart3, label: "Дашборд", active: true },
              { icon: Users, label: "Студенты", active: false },
              { icon: BookOpen, label: "Курсы", active: false },
              { icon: TrendingUp, label: "Финансы", active: false },
              { icon: Star, label: "Отзывы", active: false },
              { icon: Settings, label: "Настройки", active: false },
            ].map(({ icon: Icon, label, active }) => (
              <div
                key={label}
                className={`flex items-center gap-2.5 rounded-md px-3 py-2 text-sm select-none ${
                  active
                    ? "bg-primary/15 text-primary border-primary/15 border"
                    : "text-muted-foreground hover:text-foreground hover:bg-accent"
                }`}
              >
                <Icon className="h-4 w-4 shrink-0" aria-hidden="true" />
                {label}
              </div>
            ))}
          </aside>

          {/* Main content */}
          <div className="bg-background/50 flex flex-1 flex-col gap-4 overflow-hidden p-5">
            {/* Header row */}
            <div className="flex items-center justify-between">
              <div>
                <h2 className="text-card-foreground text-sm font-semibold">
                  Дашборд
                </h2>
                <p className="text-muted-foreground mt-0.5 text-xs">
                  Июль 2026
                </p>
              </div>
              <div className="flex items-center gap-2">
                <div className="text-muted-foreground rounded-md p-1.5">
                  <Bell className="h-4 w-4" aria-hidden="true" />
                </div>
                <div className="bg-primary flex h-7 w-7 items-center justify-center rounded-full">
                  <span className="text-primary-foreground text-xs font-semibold">
                    А
                  </span>
                </div>
              </div>
            </div>

            {/* Stats row */}
            <div className="grid grid-cols-2 gap-2.5 lg:grid-cols-4">
              {[
                { label: "Студентов", value: "1&nbsp;247", change: "+12%" },
                { label: "Курсов", value: "24", change: "+3" },
                { label: "Доход", value: "₽284K", change: "+18%" },
                { label: "Рейтинг", value: "4.9★", change: "+0.2" },
              ].map((stat) => (
                <div
                  key={stat.label}
                  className="bg-card border-border rounded-xl border p-3"
                >
                  <div className="mb-2 flex items-center justify-between">
                    <span className="text-muted-foreground text-xs">
                      {stat.label}
                    </span>
                    <span className="text-xs text-emerald-500">
                      {stat.change}
                    </span>
                  </div>
                  <div
                    className="text-card-foreground text-base font-semibold"
                    dangerouslySetInnerHTML={{ __html: stat.value }}
                  />
                </div>
              ))}
            </div>

            {/* Chart + Students */}
            <div className="grid min-h-0 flex-1 grid-cols-5 gap-2.5">
              {/* Bar chart */}
              <div className="bg-card border-border col-span-3 flex flex-col rounded-xl border p-4">
                <div className="mb-3 flex items-center justify-between">
                  <span className="text-card-foreground text-xs font-medium">
                    Записи студентов
                  </span>
                  <span className="text-muted-foreground text-xs">7 дней</span>
                </div>
                <div className="flex flex-1 items-end gap-1.5">
                  {[42, 68, 45, 82, 58, 95, 73].map((height, i) => (
                    <div key={i} className="flex flex-1 flex-col items-center">
                      <div
                        className="w-full rounded-sm transition-all"
                        style={{
                          height: `${height}%`,
                          background:
                            i === 5
                              ? `linear-gradient(to top, ${PRIMARY_RGBA(1)}, ${PRIMARY_RGBA(0.6)})`
                              : PRIMARY_RGBA(0.22),
                        }}
                      />
                    </div>
                  ))}
                </div>
                <div className="mt-2 flex justify-between">
                  {["Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Вс"].map((d) => (
                    <span
                      key={d}
                      className="text-muted-foreground flex-1 text-center text-[10px]"
                    >
                      {d}
                    </span>
                  ))}
                </div>
              </div>

              {/* Recent students */}
              <div className="bg-card border-border col-span-2 flex flex-col rounded-xl border p-4">
                <span className="text-card-foreground mb-3 block text-xs font-medium">
                  Новые студенты
                </span>
                <div className="flex flex-1 flex-col gap-3">
                  {[
                    {
                      name: "Анна К.",
                      course: "UI/UX дизайн",
                      color: "from-pink-500 to-rose-400",
                    },
                    {
                      name: "Михаил С.",
                      course: "Python Pro",
                      color: "from-blue-500 to-cyan-400",
                    },
                    {
                      name: "Елена П.",
                      course: "SMM-курс",
                      color: "from-emerald-500 to-green-400",
                    },
                    {
                      name: "Дмитрий В.",
                      course: "Маркетинг",
                      color: "from-orange-500 to-amber-400",
                    },
                  ].map((student) => (
                    <div
                      key={student.name}
                      className="flex min-w-0 items-center gap-2"
                    >
                      <div
                        className={`h-6 w-6 rounded-full bg-gradient-to-br ${student.color} flex shrink-0 items-center justify-center`}
                        aria-hidden="true"
                      >
                        <span className="text-[10px] font-bold text-white">
                          {student.name[0]}
                        </span>
                      </div>
                      <div className="min-w-0">
                        <div className="text-card-foreground truncate text-xs font-medium">
                          {student.name}
                        </div>
                        <div className="text-muted-foreground truncate text-[10px]">
                          {student.course}
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
