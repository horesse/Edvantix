"use client";

import { X } from "lucide-react";

import { Badge } from "@workspace/ui/components/badge";
import { Button } from "@workspace/ui/components/button";
import { ScrollArea } from "@workspace/ui/components/scroll-area";
import { Separator } from "@workspace/ui/components/separator";
import { cn } from "@workspace/ui/lib/utils";

export type CourseStatus = "draft" | "active" | "archived";

export interface Course {
  id: string;
  title: string;
  studentsCount: number;
  status: CourseStatus;
  updatedAt: string;
  /** Optional extended fields shown in the context panel. */
  description?: string;
  teacherName?: string;
  nextLesson?: string;
  completionRate?: number;
}

interface CourseContextPanelProps {
  course: Course;
  onClose: () => void;
  className?: string;
}

const statusConfig: Record<
  CourseStatus,
  { label: string; variant: "default" | "secondary" | "outline" }
> = {
  active: { label: "Активный", variant: "default" },
  draft: { label: "Черновик", variant: "outline" },
  archived: { label: "Архив", variant: "secondary" },
};

/**
 * Contextual detail panel shown to the right of the courses list
 * when a course row is selected. Scrolls independently from the list.
 */
export function CourseContextPanel({
  course,
  onClose,
  className,
}: CourseContextPanelProps) {
  const status = statusConfig[course.status];

  return (
    <div
      className={cn(
        "flex w-80 shrink-0 flex-col border-l border-border",
        className,
      )}
    >
      {/* ── Header ─────────────────────────────────────────────── */}
      <div className="flex items-start justify-between gap-2 px-4 py-3">
        <div className="min-w-0 flex-1">
          <p className="truncate text-sm font-semibold">{course.title}</p>
          <p className="text-muted-foreground mt-0.5 text-xs">
            Курс · {course.studentsCount} студентов
          </p>
        </div>
        <Button
          variant="ghost"
          size="icon"
          className="size-7 shrink-0"
          onClick={onClose}
          aria-label="Закрыть"
        >
          <X className="size-3.5" />
        </Button>
      </div>

      <Separator />

      <ScrollArea className="flex-1">
        <div className="space-y-5 px-4 py-4">
          {/* Status */}
          <Section label="Статус">
            <Badge variant={status.variant}>{status.label}</Badge>
          </Section>

          {/* Teacher */}
          {course.teacherName && (
            <Section label="Преподаватель">
              <p className="text-sm">{course.teacherName}</p>
            </Section>
          )}

          {/* Description */}
          {course.description && (
            <Section label="Описание">
              <p className="text-muted-foreground text-sm leading-relaxed">
                {course.description}
              </p>
            </Section>
          )}

          {/* Metrics */}
          <Section label="Метрики">
            <div className="grid grid-cols-2 gap-3">
              <Metric label="Студенты" value={String(course.studentsCount)} />
              {course.completionRate !== undefined && (
                <Metric
                  label="Прохождение"
                  value={`${course.completionRate}%`}
                />
              )}
            </div>
          </Section>

          {/* Next lesson */}
          {course.nextLesson && (
            <Section label="Следующий урок">
              <p className="text-sm">{course.nextLesson}</p>
            </Section>
          )}

          {/* Meta */}
          <Section label="Обновлено">
            <p className="text-muted-foreground text-xs">{course.updatedAt}</p>
          </Section>
        </div>
      </ScrollArea>
    </div>
  );
}

function Section({
  label,
  children,
}: {
  label: string;
  children: React.ReactNode;
}) {
  return (
    <div className="space-y-1.5">
      <p className="text-muted-foreground text-xs font-medium uppercase tracking-wide">
        {label}
      </p>
      {children}
    </div>
  );
}

function Metric({ label, value }: { label: string; value: string }) {
  return (
    <div className="rounded-md border border-border px-3 py-2">
      <p className="text-muted-foreground text-xs">{label}</p>
      <p className="text-sm font-semibold">{value}</p>
    </div>
  );
}
