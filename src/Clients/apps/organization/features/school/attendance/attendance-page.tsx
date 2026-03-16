"use client";

import { useState } from "react";

import {
  AlertCircle,
  BarChart2,
  CheckCircle2,
  ChevronLeft,
  ChevronRight,
  Clock,
  Download,
  XCircle,
} from "lucide-react";

import { Button } from "@workspace/ui/components/button";
import { cn } from "@workspace/ui/lib/utils";

// ── Mock data (replace with real API when available) ─────────────────────────
// See backend-requirements.md › Attendance API

type AttendanceStatus = "present" | "late" | "absent" | "excused" | null;

interface Student {
  id: string;
  name: string;
  initials: string;
}

interface AttendanceDay {
  date: string; // "Пн 13"
  isoDate: string; // "2026-03-13"
}

const MOCK_STUDENTS: Student[] = [
  { id: "1", name: "Александров Д.А.", initials: "АД" },
  { id: "2", name: "Борисова К.В.", initials: "БК" },
  { id: "3", name: "Васильев М.Н.", initials: "ВМ" },
  { id: "4", name: "Григорьева Л.С.", initials: "ГЛ" },
  { id: "5", name: "Дмитриев А.П.", initials: "ДА" },
  { id: "6", name: "Егорова Т.И.", initials: "ЕТ" },
  { id: "7", name: "Жуков С.Е.", initials: "ЖС" },
  { id: "8", name: "Зайцева О.В.", initials: "ЗО" },
  { id: "9", name: "Иванов Н.К.", initials: "ИН" },
  { id: "10", name: "Козлова П.Р.", initials: "КП" },
];

const WEEK_DAYS: AttendanceDay[] = [
  { date: "Пн 13", isoDate: "2026-03-13" },
  { date: "Вт 14", isoDate: "2026-03-14" },
  { date: "Ср 15", isoDate: "2026-03-15" },
  { date: "Чт 16", isoDate: "2026-03-16" },
  { date: "Пт 17", isoDate: "2026-03-17" },
];

const MOCK_RECORDS: Record<string, Record<string, AttendanceStatus>> = {
  "1": {
    "2026-03-13": "present",
    "2026-03-14": "present",
    "2026-03-15": "late",
    "2026-03-16": "present",
    "2026-03-17": "present",
  },
  "2": {
    "2026-03-13": "present",
    "2026-03-14": "absent",
    "2026-03-15": "present",
    "2026-03-16": "present",
    "2026-03-17": "late",
  },
  "3": {
    "2026-03-13": "late",
    "2026-03-14": "present",
    "2026-03-15": "present",
    "2026-03-16": "absent",
    "2026-03-17": "present",
  },
  "4": {
    "2026-03-13": "present",
    "2026-03-14": "present",
    "2026-03-15": "present",
    "2026-03-16": "present",
    "2026-03-17": "excused",
  },
  "5": {
    "2026-03-13": "absent",
    "2026-03-14": "absent",
    "2026-03-15": "present",
    "2026-03-16": "late",
    "2026-03-17": "present",
  },
  "6": {
    "2026-03-13": "present",
    "2026-03-14": "present",
    "2026-03-15": "present",
    "2026-03-16": "present",
    "2026-03-17": "present",
  },
  "7": {
    "2026-03-13": "present",
    "2026-03-14": "late",
    "2026-03-15": "absent",
    "2026-03-16": "present",
    "2026-03-17": "present",
  },
  "8": {
    "2026-03-13": "present",
    "2026-03-14": "present",
    "2026-03-15": "present",
    "2026-03-16": "present",
    "2026-03-17": "present",
  },
  "9": {
    "2026-03-13": "late",
    "2026-03-14": "present",
    "2026-03-15": "present",
    "2026-03-16": "present",
    "2026-03-17": "absent",
  },
  "10": {
    "2026-03-13": "present",
    "2026-03-14": "present",
    "2026-03-15": "excused",
    "2026-03-16": "present",
    "2026-03-17": "present",
  },
};

const STATUS_CONFIG: Record<
  NonNullable<AttendanceStatus>,
  { icon: React.ElementType; bg: string; iconColor: string; label: string }
> = {
  present: {
    icon: CheckCircle2,
    bg: "bg-emerald-50",
    iconColor: "text-emerald-500",
    label: "Присутствовал",
  },
  late: {
    icon: Clock,
    bg: "bg-amber-50",
    iconColor: "text-amber-500",
    label: "Опоздал",
  },
  absent: {
    icon: XCircle,
    bg: "bg-rose-50",
    iconColor: "text-rose-500",
    label: "Отсутствовал",
  },
  excused: {
    icon: AlertCircle,
    bg: "bg-blue-50",
    iconColor: "text-blue-500",
    label: "Уважит. причина",
  },
};

const GROUPS = ["Python для начинающих", "Веб-разработка 1", "Data Science 2"];

// ── KPI card ──────────────────────────────────────────────────────────────────

function KpiCard({
  icon: Icon,
  iconBg,
  iconColor,
  label,
  value,
  sub,
}: {
  icon: React.ElementType;
  iconBg: string;
  iconColor: string;
  label: string;
  value: string;
  sub: string;
}) {
  return (
    <div className="bg-card border-border flex items-center gap-3 rounded-xl border p-4 shadow-sm">
      <div
        className={cn(
          "flex size-10 shrink-0 items-center justify-center rounded-full",
          iconBg,
        )}
      >
        <Icon className={cn("size-5", iconColor)} />
      </div>
      <div>
        <p className="text-muted-foreground text-xs">{label}</p>
        <p className="text-foreground text-xl leading-tight font-bold">
          {value}
        </p>
        <p className="text-muted-foreground/60 text-xs">{sub}</p>
      </div>
    </div>
  );
}

// ── Status cell ───────────────────────────────────────────────────────────────

function StatusCell({ status }: { status: AttendanceStatus }) {
  if (!status) {
    return <div className="bg-muted/50 mx-auto size-6 rounded-full" />;
  }
  const cfg = STATUS_CONFIG[status];
  return (
    <div
      className={cn(
        "mx-auto flex size-7 items-center justify-center rounded-full",
        cfg.bg,
      )}
      title={cfg.label}
    >
      <cfg.icon className={cn("size-4", cfg.iconColor)} />
    </div>
  );
}

// ── Attendance rate bar ───────────────────────────────────────────────────────

function AttendanceBar({ studentId }: { studentId: string }) {
  const records = MOCK_RECORDS[studentId] ?? {};
  const days = WEEK_DAYS.length;
  const present = Object.values(records).filter(
    (s) => s === "present" || s === "late",
  ).length;
  const pct = Math.round((present / days) * 100);

  return (
    <div className="flex items-center gap-2">
      <div className="bg-muted h-1.5 w-16 overflow-hidden rounded-full">
        <div
          className={cn(
            "h-full rounded-full",
            pct >= 80
              ? "bg-emerald-500"
              : pct >= 60
                ? "bg-amber-400"
                : "bg-rose-500",
          )}
          style={{ width: `${pct}%` }}
        />
      </div>
      <span className="text-muted-foreground text-xs tabular-nums">{pct}%</span>
    </div>
  );
}

// ── Main page ─────────────────────────────────────────────────────────────────

export function AttendancePage() {
  const [selectedGroup, setSelectedGroup] = useState(GROUPS[0]);
  const [weekLabel] = useState("13–19 мар");

  // Aggregate stats
  const allStatuses = MOCK_STUDENTS.flatMap((s) =>
    WEEK_DAYS.map((d) => MOCK_RECORDS[s.id]?.[d.isoDate] ?? null),
  );
  const total = allStatuses.length;
  const presentCount = allStatuses.filter((s) => s === "present").length;
  const lateCount = allStatuses.filter((s) => s === "late").length;
  const absentCount = allStatuses.filter((s) => s === "absent").length;
  const presentPct = Math.round(((presentCount + lateCount) / total) * 100);

  return (
    <div className="space-y-5">
      {/* ── Header ── */}
      <div className="flex flex-wrap items-start justify-between gap-4">
        <div>
          <h1 className="text-foreground text-lg font-bold tracking-tight">
            Посещаемость
          </h1>
          <p className="text-muted-foreground mt-0.5 text-sm">
            {selectedGroup} · Неделя {weekLabel} 2026
          </p>
        </div>

        <div className="flex flex-wrap items-center gap-2">
          {/* Group select */}
          <select
            value={selectedGroup}
            onChange={(e) => setSelectedGroup(e.target.value)}
            className="border-border bg-background text-foreground focus:ring-primary/20 rounded-lg border px-3 py-2 text-sm focus:ring-2 focus:outline-none"
          >
            {GROUPS.map((g) => (
              <option key={g} value={g}>
                {g}
              </option>
            ))}
          </select>

          {/* Week navigator */}
          <div className="border-border bg-background flex items-center rounded-lg border">
            <button className="text-muted-foreground hover:bg-muted rounded-l-lg p-2">
              <ChevronLeft className="size-4" />
            </button>
            <span className="text-foreground px-3 text-sm font-medium">
              {weekLabel}
            </span>
            <button className="text-muted-foreground hover:bg-muted rounded-r-lg p-2">
              <ChevronRight className="size-4" />
            </button>
          </div>

          <Button variant="outline" size="sm" className="gap-1.5">
            <Download className="size-4" />
            Экспорт
          </Button>
        </div>
      </div>

      {/* ── KPI cards ── */}
      <div className="grid grid-cols-2 gap-3 sm:grid-cols-4">
        <KpiCard
          icon={CheckCircle2}
          iconBg="bg-emerald-50"
          iconColor="text-emerald-600"
          label="Присутствовали"
          value={`${presentPct}%`}
          sub={`${presentCount + lateCount} из ${MOCK_STUDENTS.length}`}
        />
        <KpiCard
          icon={Clock}
          iconBg="bg-amber-50"
          iconColor="text-amber-600"
          label="Опоздали"
          value={`${Math.round((lateCount / total) * 100)}%`}
          sub={`${lateCount} учеников`}
        />
        <KpiCard
          icon={XCircle}
          iconBg="bg-rose-50"
          iconColor="text-rose-500"
          label="Отсутствовали"
          value={`${Math.round((absentCount / total) * 100)}%`}
          sub={`${absentCount} учеников`}
        />
        <KpiCard
          icon={BarChart2}
          iconBg="bg-primary/10"
          iconColor="text-primary"
          label="Тренд"
          value="−3%"
          sub="vs прошлая неделя"
        />
      </div>

      {/* ── Attendance journal ── */}
      <div className="bg-card border-border rounded-2xl border shadow-sm">
        <div className="border-border flex items-center justify-between border-b px-5 py-4">
          <p className="text-foreground text-sm font-semibold">
            Журнал посещаемости — {selectedGroup}
          </p>
          <div className="text-muted-foreground flex items-center gap-3 text-xs">
            {Object.entries(STATUS_CONFIG).map(([key, cfg]) => (
              <span key={key} className="flex items-center gap-1">
                <cfg.icon className={cn("size-3.5", cfg.iconColor)} />
                {cfg.label}
              </span>
            ))}
          </div>
        </div>

        <div className="overflow-x-auto">
          <table className="w-full text-sm">
            <thead>
              <tr className="border-border border-b">
                <th className="text-muted-foreground px-5 py-2.5 text-left text-[11px] font-semibold tracking-wider uppercase">
                  Ученик
                </th>
                {WEEK_DAYS.map((day) => (
                  <th
                    key={day.isoDate}
                    className="text-muted-foreground w-20 px-2 py-2.5 text-center text-[11px] font-semibold tracking-wider uppercase"
                  >
                    {day.date}
                  </th>
                ))}
                <th className="text-muted-foreground px-3 py-2.5 text-left text-[11px] font-semibold tracking-wider uppercase">
                  Итог
                </th>
              </tr>
            </thead>
            <tbody>
              {MOCK_STUDENTS.map((student) => (
                <tr
                  key={student.id}
                  className="border-border hover:bg-muted/20 border-b last:border-0"
                >
                  <td className="px-5 py-3">
                    <div className="flex items-center gap-2.5">
                      <div className="from-primary/60 to-primary flex size-7 shrink-0 items-center justify-center rounded-full bg-gradient-to-br text-[10px] font-bold text-white">
                        {student.initials}
                      </div>
                      <span className="text-foreground text-sm font-medium">
                        {student.name}
                      </span>
                    </div>
                  </td>
                  {WEEK_DAYS.map((day) => (
                    <td key={day.isoDate} className="px-2 py-3 text-center">
                      <StatusCell
                        status={MOCK_RECORDS[student.id]?.[day.isoDate] ?? null}
                      />
                    </td>
                  ))}
                  <td className="px-3 py-3">
                    <AttendanceBar studentId={student.id} />
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      {/* Backend note */}
      <p className="text-muted-foreground/60 text-center text-[11px]">
        Данные демонстрационные — см. backend-requirements.md › Attendance
      </p>
    </div>
  );
}
