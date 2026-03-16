"use client";

import { useState } from "react";

import { BookOpen, Plus } from "lucide-react";

import { Badge } from "@workspace/ui/components/badge";
import { Button } from "@workspace/ui/components/button";
import { ScrollArea } from "@workspace/ui/components/scroll-area";
import { cn } from "@workspace/ui/lib/utils";

import type { Course, CourseStatus } from "./course-context-panel";
import { CourseContextPanel } from "./course-context-panel";

// ── Mock data ────────────────────────────────────────────────────────────
const MOCK_COURSES: Course[] = [
  {
    id: "1",
    title: "Введение в программирование",
    studentsCount: 32,
    status: "active",
    updatedAt: "23 фев 2026",
    teacherName: "Анна Смирнова",
    description:
      "Базовый курс по основам программирования на Python. Охватывает переменные, условия, циклы и функции.",
    completionRate: 68,
    nextLesson: "25 фев 2026, 10:00",
  },
  {
    id: "2",
    title: "Математический анализ",
    studentsCount: 18,
    status: "active",
    updatedAt: "22 фев 2026",
    teacherName: "Игорь Петров",
    description: "Пределы, производные, интегралы для студентов 1–2 курса.",
    completionRate: 45,
  },
  {
    id: "3",
    title: "Дизайн интерфейсов",
    studentsCount: 24,
    status: "draft",
    updatedAt: "20 фев 2026",
    teacherName: "Мария Козлова",
    description:
      "Принципы UX/UI-дизайна, работа с Figma, прототипирование интерфейсов.",
  },
  {
    id: "4",
    title: "Английский язык (B2)",
    studentsCount: 15,
    status: "active",
    updatedAt: "19 фев 2026",
    teacherName: "Елена Новикова",
    completionRate: 82,
    nextLesson: "24 фев 2026, 14:00",
  },
  {
    id: "5",
    title: "История архитектуры",
    studentsCount: 0,
    status: "archived",
    updatedAt: "10 янв 2026",
  },
  {
    id: "6",
    title: "Веб-разработка: React",
    studentsCount: 41,
    status: "active",
    updatedAt: "23 фев 2026",
    teacherName: "Дмитрий Волков",
    description: "Современный React 19, TypeScript, Tailwind CSS.",
    completionRate: 31,
    nextLesson: "26 фев 2026, 11:00",
  },
  {
    id: "7",
    title: "Основы маркетинга",
    studentsCount: 22,
    status: "draft",
    updatedAt: "18 фев 2026",
    teacherName: "Ольга Белова",
  },
];

// ── Status display helpers ───────────────────────────────────────────────
const statusConfig: Record<
  CourseStatus,
  { label: string; variant: "default" | "secondary" | "outline" }
> = {
  active: { label: "Активный", variant: "default" },
  draft: { label: "Черновик", variant: "outline" },
  archived: { label: "Архив", variant: "secondary" },
};

// ── Component ────────────────────────────────────────────────────────────

/**
 * Full-page feature: courses list on the left + context panel on the right.
 * Both panels scroll independently. The context panel opens when a row is clicked.
 */
export function CoursesPage() {
  const [selectedCourse, setSelectedCourse] = useState<Course | null>(null);

  function handleSelect(course: Course) {
    setSelectedCourse((prev) => (prev?.id === course.id ? null : course));
  }

  return (
    <div className="flex h-full overflow-hidden">
      {/* ── Courses list ──────────────────────────────────────────── */}
      <div className="flex min-w-0 flex-1 flex-col">
        {/* Toolbar */}
        <div className="border-border flex items-center justify-between border-b px-4 py-2.5">
          <p className="text-muted-foreground text-sm">
            {MOCK_COURSES.length} курсов
          </p>
          <Button size="sm" className="gap-1.5">
            <Plus className="size-3.5" />
            Создать курс
          </Button>
        </div>

        {/* List */}
        <ScrollArea className="flex-1">
          {MOCK_COURSES.length === 0 ? (
            <EmptyState />
          ) : (
            <ul>
              {MOCK_COURSES.map((course) => (
                <CourseRow
                  key={course.id}
                  course={course}
                  isSelected={selectedCourse?.id === course.id}
                  onClick={() => handleSelect(course)}
                />
              ))}
            </ul>
          )}
        </ScrollArea>
      </div>

      {/* ── Context panel ─────────────────────────────────────────── */}
      {selectedCourse && (
        <CourseContextPanel
          course={selectedCourse}
          onClose={() => setSelectedCourse(null)}
          // Hide panel on small screens — it would overlap the list
          className="hidden md:flex"
        />
      )}
    </div>
  );
}

// ── Sub-components ───────────────────────────────────────────────────────

function CourseRow({
  course,
  isSelected,
  onClick,
}: {
  course: Course;
  isSelected: boolean;
  onClick: () => void;
}) {
  const status = statusConfig[course.status];

  return (
    <li>
      <button
        type="button"
        onClick={onClick}
        className={cn(
          "border-border flex w-full items-center gap-3 border-b px-4 py-3 text-left transition-colors",
          isSelected ? "bg-accent" : "hover:bg-muted/40",
        )}
      >
        {/* Title */}
        <div className="min-w-0 flex-1">
          <p className="truncate text-sm font-medium">{course.title}</p>
          {course.teacherName && (
            <p className="text-muted-foreground truncate text-xs">
              {course.teacherName}
            </p>
          )}
        </div>

        {/* Students count */}
        <Badge variant="secondary" className="shrink-0 text-xs font-normal">
          {course.studentsCount} студ.
        </Badge>

        {/* Status */}
        <Badge
          variant={status.variant}
          className="hidden shrink-0 text-xs font-normal sm:inline-flex"
        >
          {status.label}
        </Badge>

        {/* Updated at */}
        <p className="text-muted-foreground hidden shrink-0 text-xs lg:block">
          {course.updatedAt}
        </p>
      </button>
    </li>
  );
}

function EmptyState() {
  return (
    <div className="flex flex-col items-center justify-center gap-3 py-20 text-center">
      <div className="bg-muted flex size-12 items-center justify-center rounded-full">
        <BookOpen className="text-muted-foreground size-5" />
      </div>
      <div>
        <p className="text-sm font-medium">Курсов пока нет</p>
        <p className="text-muted-foreground mt-1 text-xs">
          Создайте первый курс, чтобы начать
        </p>
      </div>
      <Button size="sm" variant="outline" className="gap-1.5">
        <Plus className="size-3.5" />
        Создать курс
      </Button>
    </div>
  );
}
