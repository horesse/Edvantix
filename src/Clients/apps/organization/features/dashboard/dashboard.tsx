"use client";

import Link from "next/link";

import {
  BarChart2,
  BookOpen,
  Building2,
  CalendarDays,
  ChevronRight,
  GraduationCap,
  Plus,
  TrendingDown,
  TrendingUp,
  UserPlus,
  Users,
} from "lucide-react";

import useOrganization from "@workspace/api-hooks/company/useOrganization";
import { Button } from "@workspace/ui/components/button";
import { Skeleton } from "@workspace/ui/components/skeleton";
import { cn } from "@workspace/ui/lib/utils";

import { useOrganization as useOrgContext } from "@/components/organization/provider";

import { IncomingInvitationsSection } from "../invitations/incoming-invitations-section";

// ── Types ────────────────────────────────────────────────────────────────────

interface KpiConfig {
  label: string;
  value: number | string;
  change: string;
  trend: "up" | "down" | "neutral";
  icon: React.ElementType;
  iconBg: string;
  iconColor: string;
}

// ── Helpers ──────────────────────────────────────────────────────────────────

function today(): string {
  return new Date().toLocaleDateString("ru-RU", {
    weekday: "long",
    day: "numeric",
    month: "long",
  });
}

// ── Sub-components ───────────────────────────────────────────────────────────

function KpiCard({ kpi, isLoading }: { kpi: KpiConfig; isLoading: boolean }) {
  return (
    <div className="bg-card border-border rounded-2xl border p-5 shadow-sm">
      <div className="mb-3 flex items-center justify-between">
        <p className="text-muted-foreground text-xs font-medium">{kpi.label}</p>
        <div
          className={cn(
            "flex size-8 items-center justify-center rounded-lg",
            kpi.iconBg,
          )}
        >
          <kpi.icon className={cn("size-4", kpi.iconColor)} />
        </div>
      </div>
      {isLoading ? (
        <Skeleton className="h-7 w-16" />
      ) : (
        <div className="flex items-end justify-between">
          <p className="text-foreground text-2xl font-bold tabular-nums">
            {kpi.value}
          </p>
          {kpi.trend !== "neutral" && (
            <div
              className={cn(
                "flex items-center gap-0.5 text-xs font-medium",
                kpi.trend === "up" ? "text-emerald-600" : "text-rose-500",
              )}
            >
              {kpi.trend === "up" ? (
                <TrendingUp className="size-3.5" />
              ) : (
                <TrendingDown className="size-3.5" />
              )}
              {kpi.change}
            </div>
          )}
          {kpi.trend === "neutral" && (
            <span className="text-muted-foreground text-xs">{kpi.change}</span>
          )}
        </div>
      )}
    </div>
  );
}

/** Inline donut SVG for attendance rate. */
function AttendanceDonut({ pct }: { pct: number }) {
  const r = 36;
  const circ = 2 * Math.PI * r;
  const late = 7;
  const absent = 100 - pct - late;

  const presentDash = (pct / 100) * circ;
  const lateDash = (late / 100) * circ;
  const absentDash = (absent / 100) * circ;

  const presentOffset = 0;
  const lateOffset = -presentDash;
  const absentOffset = -(presentDash + lateDash);

  return (
    <div className="relative flex size-32 items-center justify-center">
      <svg width="128" height="128" viewBox="0 0 88 88" className="-rotate-90">
        {/* Track */}
        <circle
          cx="44"
          cy="44"
          r={r}
          fill="none"
          stroke="#f1f5f9"
          strokeWidth="10"
        />
        {/* Present — emerald */}
        <circle
          cx="44"
          cy="44"
          r={r}
          fill="none"
          stroke="#10b981"
          strokeWidth="10"
          strokeLinecap="butt"
          strokeDasharray={`${presentDash} ${circ - presentDash}`}
          strokeDashoffset={presentOffset}
        />
        {/* Late — amber */}
        <circle
          cx="44"
          cy="44"
          r={r}
          fill="none"
          stroke="#f59e0b"
          strokeWidth="10"
          strokeLinecap="butt"
          strokeDasharray={`${lateDash} ${circ - lateDash}`}
          strokeDashoffset={lateOffset}
        />
        {/* Absent — rose */}
        <circle
          cx="44"
          cy="44"
          r={r}
          fill="none"
          stroke="#f43f5e"
          strokeWidth="10"
          strokeLinecap="butt"
          strokeDasharray={`${absentDash} ${circ - absentDash}`}
          strokeDashoffset={absentOffset}
        />
      </svg>
      <div className="absolute flex flex-col items-center">
        <span className="text-foreground text-xl font-bold">{pct}%</span>
        <span className="text-muted-foreground text-[10px]">посещ.</span>
      </div>
    </div>
  );
}

function AttendanceWidget() {
  const pct = 87;
  const items = [
    { label: "Присутствуют", count: 216, color: "bg-emerald-500" },
    { label: "Опаздывают", count: 17, color: "bg-amber-400" },
    { label: "Отсутствуют", count: 15, color: "bg-rose-500" },
  ];

  return (
    <div className="bg-card border-border flex flex-col rounded-2xl border p-5 shadow-sm">
      <div className="mb-4 flex items-center justify-between">
        <div>
          <p className="text-foreground text-sm font-semibold">Посещаемость</p>
          <p className="text-muted-foreground mt-0.5 text-xs">Сегодня</p>
        </div>
        <Link
          href="/school/attendance"
          className="text-primary text-xs font-medium hover:underline"
        >
          Полный отчёт →
        </Link>
      </div>

      <div className="flex flex-col items-center gap-4 sm:flex-row">
        <AttendanceDonut pct={pct} />
        <div className="flex flex-1 flex-col gap-2">
          {items.map((item) => (
            <div key={item.label} className="flex items-center justify-between">
              <div className="flex items-center gap-2">
                <div className={cn("size-2.5 rounded-full", item.color)} />
                <span className="text-muted-foreground text-xs">
                  {item.label}
                </span>
              </div>
              <span className="text-foreground text-xs font-semibold tabular-nums">
                {item.count}
              </span>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}

/** Recent activity / alerts card. */
function AlertsCard() {
  const alerts = [
    {
      icon: CalendarDays,
      iconBg: "bg-amber-50",
      iconColor: "text-amber-600",
      title: "Занятие отменено",
      desc: "Иванова С.Д. отменила урок по Python на 14:00",
      time: "30 мин назад",
    },
    {
      icon: UserPlus,
      iconBg: "bg-blue-50",
      iconColor: "text-blue-600",
      title: "Новый запрос",
      desc: "Алексей Петров ожидает подтверждения участия",
      time: "2 часа назад",
    },
    {
      icon: TrendingDown,
      iconBg: "bg-rose-50",
      iconColor: "text-rose-500",
      title: "Низкая посещаемость",
      desc: "Группа «Веб-дизайн 2» — 61% за последние 2 недели",
      time: "вчера",
    },
    {
      icon: GraduationCap,
      iconBg: "bg-emerald-50",
      iconColor: "text-emerald-600",
      title: "Курс завершён",
      desc: "«Основы Python» — 24 ученика получили сертификаты",
      time: "2 дня назад",
    },
  ];

  return (
    <div className="bg-card border-border rounded-2xl border p-5 shadow-sm">
      <div className="mb-4">
        <p className="text-foreground text-sm font-semibold">Уведомления</p>
        <p className="text-muted-foreground mt-0.5 text-xs">
          Последние события
        </p>
      </div>
      <div className="space-y-3">
        {alerts.map((a, i) => (
          <div key={i} className="flex items-start gap-3">
            <div
              className={cn(
                "mt-0.5 flex size-8 shrink-0 items-center justify-center rounded-lg",
                a.iconBg,
              )}
            >
              <a.icon className={cn("size-4", a.iconColor)} />
            </div>
            <div className="min-w-0 flex-1">
              <p className="text-foreground text-xs font-semibold">{a.title}</p>
              <p className="text-muted-foreground mt-0.5 line-clamp-1 text-xs">
                {a.desc}
              </p>
            </div>
            <span className="text-muted-foreground/60 shrink-0 text-[10px]">
              {a.time}
            </span>
          </div>
        ))}
      </div>
    </div>
  );
}

/** Simple bar chart for member growth. */
function GrowthCard() {
  const data = [
    { month: "Янв", value: 45 },
    { month: "Фев", value: 52 },
    { month: "Мар", value: 61 },
    { month: "Апр", value: 68 },
    { month: "Май", value: 75 },
    { month: "Июн", value: 87 },
  ];
  const max = Math.max(...data.map((d) => d.value));

  return (
    <div className="bg-card border-border rounded-2xl border p-5 shadow-sm">
      <div className="mb-4 flex items-center justify-between">
        <div>
          <p className="text-foreground text-sm font-semibold">
            Рост участников
          </p>
          <p className="text-muted-foreground mt-0.5 text-xs">
            Последние 6 месяцев
          </p>
        </div>
        <div className="flex items-center gap-1 text-emerald-600">
          <TrendingUp className="size-4" />
          <span className="text-xs font-semibold">+93%</span>
        </div>
      </div>
      <div className="flex h-36 items-end gap-2">
        {data.map((d) => (
          <div
            key={d.month}
            className="flex flex-1 flex-col items-center gap-1.5"
          >
            <div
              className="bg-primary/20 hover:bg-primary/40 w-full rounded-t-md transition-all"
              style={{ height: `${(d.value / max) * 120}px` }}
            />
            <span className="text-muted-foreground text-[10px]">{d.month}</span>
          </div>
        ))}
      </div>
    </div>
  );
}

/** Groups summary table. */
function GroupsCard({ orgId }: { orgId: string }) {
  const { data: org } = useOrganization(orgId);

  const groups = [
    {
      name: "Веб-разработка 1",
      teacher: "Иванова С.Д.",
      students: 24,
      status: "active",
    },
    {
      name: "Python Advanced",
      teacher: "Смирнов А.В.",
      students: 18,
      status: "active",
    },
    {
      name: "Data Science 2",
      teacher: "Козлова М.И.",
      students: 21,
      status: "pending",
    },
    {
      name: "Дизайн интерфейсов",
      teacher: "Новиков Д.Е.",
      students: 15,
      status: "active",
    },
  ];

  return (
    <div className="bg-card border-border rounded-2xl border shadow-sm">
      <div className="border-border flex items-center justify-between border-b px-5 py-4">
        <div>
          <p className="text-foreground text-sm font-semibold">Группы</p>
          <p className="text-muted-foreground mt-0.5 text-xs">
            {org?.groupsCount ?? groups.length} активных
          </p>
        </div>
        <Button asChild size="sm" variant="outline">
          <Link href="/organization/groups">
            <Plus className="size-3.5" />
            Создать
          </Link>
        </Button>
      </div>
      <div className="divide-border divide-y">
        {groups.map((g) => (
          <div
            key={g.name}
            className="hover:bg-muted/30 flex items-center gap-3 px-5 py-3"
          >
            <div className="bg-primary/10 text-primary flex size-8 shrink-0 items-center justify-center rounded-lg text-xs font-bold">
              {g.name.slice(0, 2).toUpperCase()}
            </div>
            <div className="min-w-0 flex-1">
              <p className="text-foreground truncate text-sm font-medium">
                {g.name}
              </p>
              <p className="text-muted-foreground truncate text-xs">
                {g.teacher}
              </p>
            </div>
            <div className="flex items-center gap-2">
              <span className="text-muted-foreground text-xs">
                {g.students} уч.
              </span>
              <span
                className={cn(
                  "rounded-full px-2 py-0.5 text-[10px] font-semibold",
                  g.status === "active"
                    ? "bg-emerald-50 text-emerald-700"
                    : "bg-amber-50 text-amber-700",
                )}
              >
                {g.status === "active" ? "Активна" : "Ожидает"}
              </span>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}

// ── No-org empty state ────────────────────────────────────────────────────────

function NoOrgState() {
  const features = [
    {
      icon: Users,
      iconBg: "bg-blue-100",
      iconColor: "text-blue-600",
      title: "Учителя",
      desc: "Профили и нагрузка",
    },
    {
      icon: GraduationCap,
      iconBg: "bg-violet-100",
      iconColor: "text-violet-600",
      title: "Ученики",
      desc: "Прогресс и оценки",
    },
    {
      icon: CalendarDays,
      iconBg: "bg-amber-100",
      iconColor: "text-amber-600",
      title: "Расписание",
      desc: "Группы и занятия",
    },
    {
      icon: BarChart2,
      iconBg: "bg-emerald-100",
      iconColor: "text-emerald-600",
      title: "Аналитика",
      desc: "Статистика и отчёты",
    },
  ] as const;

  return (
    <div className="flex flex-1 items-center justify-center px-6 py-12">
      <div className="w-full max-w-4xl">
        {/* Hero */}
        <div className="mb-12 text-center">
          <div className="relative mb-8 inline-flex items-center justify-center">
            {/* Outer pulsing ring */}
            <div className="border-primary/20 absolute size-48 animate-pulse rounded-full border-2 border-dashed" />
            {/* Mid ring */}
            <div className="bg-primary/5 absolute size-36 rounded-full" />
            {/* Center icon */}
            <div className="from-primary to-primary/80 shadow-primary/30 relative flex size-24 items-center justify-center rounded-2xl bg-gradient-to-br shadow-xl">
              <Building2
                className="text-primary-foreground size-12"
                strokeWidth={1.5}
              />
            </div>
            {/* Floating badges */}
            <div className="border-border bg-card absolute -top-1 -right-4 flex items-center gap-1.5 rounded-xl border px-2.5 py-1.5 shadow-md">
              <div className="size-2 rounded-full bg-emerald-400" />
              <span className="text-foreground text-xs font-medium">
                Учителя
              </span>
            </div>
            <div className="border-border bg-card absolute -bottom-2 -left-6 flex items-center gap-1.5 rounded-xl border px-2.5 py-1.5 shadow-md">
              <div className="size-2 rounded-full bg-violet-400" />
              <span className="text-foreground text-xs font-medium">
                Ученики
              </span>
            </div>
            <div className="border-border bg-card absolute top-10 -left-10 flex items-center gap-1.5 rounded-xl border px-2.5 py-1.5 shadow-md">
              <div className="size-2 rounded-full bg-amber-400" />
              <span className="text-foreground text-xs font-medium">
                Группы
              </span>
            </div>
          </div>

          <h1 className="text-foreground mb-3 text-2xl font-bold">
            У вас пока нет организации
          </h1>
          <p className="text-muted-foreground mx-auto max-w-md text-base leading-relaxed">
            Создайте свою первую организацию, чтобы начать управлять учителями,
            учениками, группами и расписанием.
          </p>
        </div>

        {/* Action cards */}
        <div className="mb-8 grid grid-cols-1 gap-4 md:grid-cols-2">
          {/* Primary: create */}
          <Link
            href="/create-organization"
            className="group border-primary/30 bg-card hover:border-primary/60 hover:shadow-primary/10 relative rounded-2xl border-2 p-6 text-left transition-all hover:shadow-lg"
          >
            <div className="absolute top-4 right-4">
              <span className="bg-primary/10 text-primary rounded-full px-2 py-0.5 text-[10px] font-bold tracking-wider uppercase">
                Рекомендуется
              </span>
            </div>
            <div className="from-primary to-primary/80 mb-4 flex size-12 items-center justify-center rounded-2xl bg-gradient-to-br shadow-md transition-transform group-hover:scale-105">
              <Plus className="text-primary-foreground size-6" />
            </div>
            <h3 className="text-foreground mb-1.5 text-base font-bold">
              Создать организацию
            </h3>
            <p className="text-muted-foreground text-sm leading-relaxed">
              Настройте новую образовательную организацию с нуля — школу,
              академию или онлайн-курс.
            </p>
            <div className="text-primary mt-4 flex items-center gap-1.5 text-sm font-semibold transition-all group-hover:gap-2.5">
              Начать
              <ChevronRight className="size-4" />
            </div>
          </Link>

          {/* Secondary: join */}
          <Link
            href="/invitations"
            className="group border-border bg-card hover:border-border/80 rounded-2xl border-2 p-6 text-left transition-all hover:shadow-md"
          >
            <div className="bg-muted group-hover:bg-muted/80 mb-4 flex size-12 items-center justify-center rounded-2xl transition-all group-hover:scale-105">
              <UserPlus className="text-muted-foreground size-6" />
            </div>
            <h3 className="text-foreground mb-1.5 text-base font-bold">
              Войти в организацию
            </h3>
            <p className="text-muted-foreground text-sm leading-relaxed">
              Введите код приглашения или перейдите по ссылке, полученной от
              администратора.
            </p>
            <div className="text-muted-foreground mt-4 flex items-center gap-1.5 text-sm font-semibold transition-all group-hover:gap-2.5">
              Посмотреть приглашения
              <ChevronRight className="size-4" />
            </div>
          </Link>
        </div>

        {/* Feature grid */}
        <div className="border-border bg-card rounded-2xl border p-6">
          <p className="text-muted-foreground mb-4 text-xs font-semibold tracking-wider uppercase">
            Что доступно в организации
          </p>
          <div className="grid grid-cols-2 gap-4 md:grid-cols-4">
            {features.map((f) => (
              <div
                key={f.title}
                className="hover:bg-muted/50 flex flex-col items-center gap-2 rounded-xl p-3 text-center transition-colors"
              >
                <div
                  className={cn(
                    "flex size-10 items-center justify-center rounded-xl",
                    f.iconBg,
                  )}
                >
                  <f.icon className={cn("size-5", f.iconColor)} />
                </div>
                <div>
                  <p className="text-foreground text-sm font-semibold">
                    {f.title}
                  </p>
                  <p className="text-muted-foreground mt-0.5 text-xs">
                    {f.desc}
                  </p>
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
}

// ── Main ─────────────────────────────────────────────────────────────────────

export function Dashboard() {
  const { currentOrg, canManage } = useOrgContext();

  if (!currentOrg) {
    return <NoOrgState />;
  }

  return <DashboardContent orgId={currentOrg.id} canManage={canManage} />;
}

function DashboardContent({
  orgId,
  canManage,
}: {
  orgId: string;
  canManage: boolean;
}) {
  const { data: org, isLoading } = useOrganization(orgId);

  const kpis: KpiConfig[] = [
    {
      label: "Учеников",
      value: 248,
      change: "+12%",
      trend: "up",
      icon: Users,
      iconBg: "bg-blue-50",
      iconColor: "text-blue-600",
    },
    {
      label: "Активных групп",
      value: org?.groupsCount ?? 18,
      change: "+3",
      trend: "up",
      icon: BookOpen,
      iconBg: "bg-amber-50",
      iconColor: "text-amber-600",
    },
    {
      label: "Посещаемость",
      value: "87%",
      change: "-3%",
      trend: "down",
      icon: CalendarDays,
      iconBg: "bg-emerald-50",
      iconColor: "text-emerald-600",
    },
    {
      label: "Учителей",
      value: 32,
      change: "6 свободны сегодня",
      trend: "neutral",
      icon: GraduationCap,
      iconBg: "bg-violet-50",
      iconColor: "text-violet-600",
    },
  ];

  return (
    <div className="space-y-5">
      {/* ── Page header ── */}
      <div className="flex items-start justify-between gap-4">
        <div>
          {isLoading ? (
            <>
              <Skeleton className="h-6 w-48" />
              <Skeleton className="mt-1.5 h-4 w-32" />
            </>
          ) : (
            <>
              <h1 className="text-foreground text-lg font-bold tracking-tight">
                {org?.name ?? "Главная"}
              </h1>
              <p className="text-muted-foreground mt-0.5 text-sm capitalize">
                {today()}
              </p>
            </>
          )}
        </div>
        {canManage && (
          <Button asChild size="sm">
            <Link href="/organization/invitations">
              <UserPlus className="size-4" />
              Пригласить
            </Link>
          </Button>
        )}
      </div>

      {/* ── KPI Cards ── */}
      <div className="grid grid-cols-2 gap-3 sm:grid-cols-4">
        {kpis.map((kpi) => (
          <KpiCard key={kpi.label} kpi={kpi} isLoading={isLoading} />
        ))}
      </div>

      {/* ── Main grid ── */}
      <div className="grid grid-cols-1 gap-5 lg:grid-cols-3">
        {/* Left col: groups + growth */}
        <div className="flex flex-col gap-5 lg:col-span-2">
          <GroupsCard orgId={orgId} />
          <GrowthCard />
        </div>

        {/* Right col: attendance + alerts + invitations */}
        <div className="flex flex-col gap-5">
          <AttendanceWidget />
          <AlertsCard />
          <IncomingInvitationsSection />
        </div>
      </div>
    </div>
  );
}
