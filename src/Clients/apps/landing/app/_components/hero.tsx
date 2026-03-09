"use client";

import { useRef, useEffect, useState } from "react";

import Link from "next/link";
import {
  ArrowRight,
  Play,
  CheckCircle2,
  TrendingUp,
  Users,
  BookOpen,
  Star,
  BarChart3,
  Bell,
  Settings,
  ChevronRight,
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
      className="relative min-h-screen flex flex-col items-center justify-center overflow-hidden bg-background"
      aria-label="Главный экран"
    >
      {/* Dot grid */}
      <div className="absolute inset-0 dot-pattern opacity-60" aria-hidden="true" />

      {/* Aurora glow — primary + ring colors */}
      <div
        className="absolute inset-0 pointer-events-none"
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
        className="absolute inset-0 pointer-events-none"
        aria-hidden="true"
        style={{
          background: `radial-gradient(700px circle at ${mousePos.x}px ${mousePos.y}px, ${PRIMARY_RGBA(0.04)}, transparent 40%)`,
        }}
      />

      {/* Decorative rings */}
      <div
        className="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-[600px] h-[600px] rounded-full border border-primary/5 animate-spin-slow pointer-events-none"
        aria-hidden="true"
      />
      <div
        className="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-[900px] h-[900px] rounded-full border border-primary/[0.03] animate-spin-slow pointer-events-none"
        style={{ animationDirection: "reverse", animationDuration: "40s" }}
        aria-hidden="true"
      />

      {/* Content */}
      <div className="relative z-10 max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 pt-28 pb-16 flex flex-col items-center text-center">
        {/* Announcement badge */}
        <div
          className={`inline-flex items-center gap-2 px-4 py-1.5 rounded-full border border-primary/25 bg-primary/8 text-primary text-sm mb-10 transition-all duration-700 ${
            isLoaded ? "opacity-100 translate-y-0" : "opacity-0 translate-y-4"
          }`}
          style={{ transitionDelay: "100ms" }}
        >
          <span
            className="w-1.5 h-1.5 rounded-full bg-primary animate-pulse"
            aria-hidden="true"
          />
          <span>Новое: AI-аналитика и умные отчёты 2.0</span>
          <ChevronRight className="w-3.5 h-3.5 opacity-60" aria-hidden="true" />
        </div>

        {/* Headline */}
        <h1
          className={`text-5xl sm:text-6xl lg:text-7xl xl:text-8xl font-extrabold tracking-tight mb-6 leading-[0.95] transition-all duration-700 ${
            isLoaded ? "opacity-100 translate-y-0" : "opacity-0 translate-y-8"
          }`}
          style={{ transitionDelay: "200ms" }}
        >
          <span className="block text-card-foreground">Управляйте</span>
          <span className="block text-shimmer py-1">онлайн-школой</span>
          <span className="block text-card-foreground">без хаоса</span>
        </h1>

        {/* Subtitle */}
        <p
          className={`text-lg sm:text-xl text-muted-foreground max-w-2xl mb-10 leading-relaxed transition-all duration-700 ${
            isLoaded ? "opacity-100 translate-y-0" : "opacity-0 translate-y-6"
          }`}
          style={{ transitionDelay: "300ms" }}
        >
          Единая платформа для студентов, курсов, финансов и аналитики.
          Запустите новую жизнь вашей школы за&nbsp;15&nbsp;минут.
        </p>

        {/* CTA buttons */}
        <div
          className={`flex flex-col sm:flex-row gap-4 mb-10 transition-all duration-700 ${
            isLoaded ? "opacity-100 translate-y-0" : "opacity-0 translate-y-6"
          }`}
          style={{ transitionDelay: "400ms" }}
        >
          <Button
            asChild
            className="bg-primary hover:bg-primary/90 text-primary-foreground px-8 h-12 text-base shadow-xl shadow-primary/25 hover:shadow-primary/40 transition-all duration-300 hover:scale-[1.02] focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 focus-visible:ring-offset-background"
          >
            <Link href="/signup">
              Начать бесплатно
              <ArrowRight className="w-4 h-4 ml-1" aria-hidden="true" />
            </Link>
          </Button>
          <Button
            asChild
            variant="outline"
            className="border-border bg-muted/30 hover:bg-muted/60 text-foreground px-8 h-12 text-base hover:border-border transition-all duration-300 focus-visible:ring-2 focus-visible:ring-ring"
          >
            <Link href="/demo">
              <Play className="w-4 h-4 mr-2 text-primary" aria-hidden="true" />
              Смотреть демо
            </Link>
          </Button>
        </div>

        {/* Trust indicators */}
        <div
          className={`flex flex-wrap justify-center gap-6 text-sm text-muted-foreground mb-24 transition-all duration-700 ${
            isLoaded ? "opacity-100 translate-y-0" : "opacity-0 translate-y-4"
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
                className="w-4 h-4 text-primary shrink-0"
                aria-hidden="true"
              />
              {item}
            </span>
          ))}
        </div>

        {/* Dashboard mockup */}
        <div
          className={`w-full max-w-5xl transition-all duration-1000 ${
            isLoaded ? "opacity-100 translate-y-0" : "opacity-0 translate-y-16"
          }`}
          style={{ transitionDelay: "600ms" }}
        >
          <DashboardMockup />
        </div>
      </div>

      {/* Bottom fade */}
      <div
        className="absolute bottom-0 left-0 right-0 h-40 bg-gradient-to-t from-background to-transparent pointer-events-none"
        aria-hidden="true"
      />
    </section>
  );
}

function DashboardMockup() {
  return (
    <div className="relative animate-float" style={{ animationDuration: "8s" }}>
      {/* Ambient glow */}
      <div
        className="absolute -inset-8 rounded-[40px] blur-3xl opacity-15 animate-pulse-glow"
        style={{
          background: `radial-gradient(ellipse, ${PRIMARY_RGBA(0.5)} 0%, ${SECONDARY_RGBA(0.15)} 60%, transparent 80%)`,
        }}
        aria-hidden="true"
      />

      {/* Browser window */}
      <div className="relative rounded-2xl border border-border bg-card shadow-[0_32px_80px_rgba(0,0,0,0.4)] overflow-hidden">
        {/* Title bar */}
        <div className="flex items-center gap-2 px-4 py-3 border-b border-border bg-card">
          <div className="flex gap-1.5" aria-hidden="true">
            <div className="w-3 h-3 rounded-full bg-destructive/70" />
            <div className="w-3 h-3 rounded-full bg-yellow-500/70" />
            <div className="w-3 h-3 rounded-full bg-emerald-500/70" />
          </div>
          <div className="flex-1 mx-4 flex justify-center">
            <div className="w-52 h-5 bg-muted rounded-md flex items-center justify-center">
              <span className="text-muted-foreground text-xs">app.edvantix.ru/dashboard</span>
            </div>
          </div>
          <div className="w-16" aria-hidden="true" />
        </div>

        {/* Dashboard */}
        <div className="flex h-[420px] sm:h-[460px]">
          {/* Sidebar */}
          <aside className="w-52 shrink-0 border-r border-border bg-card p-3 hidden sm:flex flex-col gap-1">
            <div className="flex items-center gap-2 px-3 py-2 mb-4">
              <div className="w-6 h-6 rounded-md bg-primary flex items-center justify-center">
                <span className="text-primary-foreground text-xs font-bold">E</span>
              </div>
              <span className="text-card-foreground text-sm font-semibold">Edvantix</span>
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
                className={`flex items-center gap-2.5 px-3 py-2 rounded-md text-sm select-none ${
                  active
                    ? "bg-primary/15 text-primary border border-primary/15"
                    : "text-muted-foreground hover:text-foreground hover:bg-accent"
                }`}
              >
                <Icon className="w-4 h-4 shrink-0" aria-hidden="true" />
                {label}
              </div>
            ))}
          </aside>

          {/* Main content */}
          <div className="flex-1 p-5 overflow-hidden flex flex-col gap-4 bg-background/50">
            {/* Header row */}
            <div className="flex items-center justify-between">
              <div>
                <h2 className="text-card-foreground text-sm font-semibold">Дашборд</h2>
                <p className="text-muted-foreground text-xs mt-0.5">Июль 2026</p>
              </div>
              <div className="flex items-center gap-2">
                <div className="p-1.5 rounded-md text-muted-foreground">
                  <Bell className="w-4 h-4" aria-hidden="true" />
                </div>
                <div className="w-7 h-7 rounded-full bg-primary flex items-center justify-center">
                  <span className="text-primary-foreground text-xs font-semibold">А</span>
                </div>
              </div>
            </div>

            {/* Stats row */}
            <div className="grid grid-cols-2 lg:grid-cols-4 gap-2.5">
              {[
                { label: "Студентов", value: "1&nbsp;247", change: "+12%" },
                { label: "Курсов", value: "24", change: "+3" },
                { label: "Доход", value: "₽284K", change: "+18%" },
                { label: "Рейтинг", value: "4.9★", change: "+0.2" },
              ].map((stat) => (
                <div
                  key={stat.label}
                  className="bg-card border border-border rounded-xl p-3"
                >
                  <div className="flex items-center justify-between mb-2">
                    <span className="text-muted-foreground text-xs">{stat.label}</span>
                    <span className="text-emerald-500 text-xs">{stat.change}</span>
                  </div>
                  <div
                    className="text-card-foreground font-semibold text-base"
                    dangerouslySetInnerHTML={{ __html: stat.value }}
                  />
                </div>
              ))}
            </div>

            {/* Chart + Students */}
            <div className="grid grid-cols-5 gap-2.5 flex-1 min-h-0">
              {/* Bar chart */}
              <div className="col-span-3 bg-card border border-border rounded-xl p-4 flex flex-col">
                <div className="flex items-center justify-between mb-3">
                  <span className="text-card-foreground text-xs font-medium">Записи студентов</span>
                  <span className="text-muted-foreground text-xs">7 дней</span>
                </div>
                <div className="flex items-end gap-1.5 flex-1">
                  {[42, 68, 45, 82, 58, 95, 73].map((height, i) => (
                    <div key={i} className="flex-1 flex flex-col items-center">
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
                <div className="flex justify-between mt-2">
                  {["Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Вс"].map((d) => (
                    <span key={d} className="text-muted-foreground text-[10px] flex-1 text-center">
                      {d}
                    </span>
                  ))}
                </div>
              </div>

              {/* Recent students */}
              <div className="col-span-2 bg-card border border-border rounded-xl p-4 flex flex-col">
                <span className="text-card-foreground text-xs font-medium block mb-3">
                  Новые студенты
                </span>
                <div className="flex flex-col gap-3 flex-1">
                  {[
                    { name: "Анна К.", course: "UI/UX дизайн", color: "from-pink-500 to-rose-400" },
                    { name: "Михаил С.", course: "Python Pro", color: "from-blue-500 to-cyan-400" },
                    { name: "Елена П.", course: "SMM-курс", color: "from-emerald-500 to-green-400" },
                    { name: "Дмитрий В.", course: "Маркетинг", color: "from-orange-500 to-amber-400" },
                  ].map((student) => (
                    <div key={student.name} className="flex items-center gap-2 min-w-0">
                      <div
                        className={`w-6 h-6 rounded-full bg-gradient-to-br ${student.color} flex items-center justify-center shrink-0`}
                        aria-hidden="true"
                      >
                        <span className="text-white text-[10px] font-bold">
                          {student.name[0]}
                        </span>
                      </div>
                      <div className="min-w-0">
                        <div className="text-card-foreground text-xs font-medium truncate">
                          {student.name}
                        </div>
                        <div className="text-muted-foreground text-[10px] truncate">
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
