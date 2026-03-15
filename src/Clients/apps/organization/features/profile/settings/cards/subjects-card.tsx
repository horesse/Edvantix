"use client";

import { useRef, useState } from "react";

import { Tag, X } from "lucide-react";
import { toast } from "sonner";

import { cn } from "@workspace/ui/lib/utils";

/**
 * Subjects/expertise tags for the profile.
 *
 * Local state only — backend endpoint not yet available.
 * See backend-requirements.md › Profile Subjects endpoint.
 */
export function SubjectsCard() {
  const [tags, setTags] = useState<string[]>([]);
  const [inputValue, setInputValue] = useState("");
  const inputRef = useRef<HTMLInputElement>(null);

  function addTag(value: string) {
    const trimmed = value.trim();
    if (!trimmed) return;
    if (tags.includes(trimmed)) {
      toast.info("Этот тег уже добавлен");
      return;
    }
    if (tags.length >= 20) {
      toast.error("Максимум 20 тегов");
      return;
    }
    setTags((prev) => [...prev, trimmed]);
    setInputValue("");
  }

  function removeTag(tag: string) {
    setTags((prev) => prev.filter((t) => t !== tag));
  }

  function handleKeyDown(e: React.KeyboardEvent<HTMLInputElement>) {
    if (e.key === "Enter") {
      e.preventDefault();
      addTag(inputValue);
    }
    if (e.key === "Backspace" && !inputValue && tags.length > 0) {
      setTags((prev) => prev.slice(0, -1));
    }
  }

  return (
    <div className="bg-card border-border rounded-2xl border p-5 shadow-sm">
      {/* Header */}
      <div className="mb-4 flex items-center gap-2">
        <div className="flex size-6 items-center justify-center rounded-lg bg-violet-100">
          <Tag className="size-3.5 text-violet-600" />
        </div>
        <span className="text-foreground text-sm font-semibold">
          Предметы / Теги
        </span>
      </div>

      {/* Tags */}
      {tags.length > 0 && (
        <div className="mb-3 flex flex-wrap gap-2">
          {tags.map((tag) => (
            <span
              key={tag}
              className={[
                "inline-flex items-center gap-1.5 rounded-full border px-2.5 py-1.5 text-xs font-medium",
                /* light */ "border-brand-200 bg-brand-50 text-brand-700",
                /* dark  */ "dark:border-brand-500/35 dark:bg-brand-500/[0.18] dark:text-brand-300",
              ].join(" ")}
            >
              {tag}
              <button
                type="button"
                onClick={() => removeTag(tag)}
                className="text-brand-400 hover:text-brand-700 dark:text-brand-400 dark:hover:text-brand-300 transition-colors"
                aria-label={`Удалить тег ${tag}`}
              >
                <X className="size-3" />
              </button>
            </span>
          ))}
        </div>
      )}

      {/* Input */}
      <div className="flex gap-2">
        <input
          ref={inputRef}
          type="text"
          value={inputValue}
          onChange={(e) => setInputValue(e.target.value)}
          onKeyDown={handleKeyDown}
          placeholder="Добавить тег…"
          className={cn(
            "border-border bg-muted/40 text-foreground flex-1 rounded-lg border px-3 py-2 text-xs",
            "placeholder:text-muted-foreground transition outline-none",
            "focus:border-brand-400 focus:ring-brand-100 focus:ring-2",
            "dark:bg-surface-850 dark:border-surface-700",
          )}
        />
        <button
          type="button"
          onClick={() => addTag(inputValue)}
          className="bg-primary hover:bg-primary/90 flex size-8 shrink-0 items-center justify-center rounded-lg text-base font-bold text-white transition-colors"
          aria-label="Добавить тег"
        >
          +
        </button>
      </div>

      <p className="text-muted-foreground mt-2 text-[11px]">
        Нажмите Enter или «+» для добавления
      </p>
    </div>
  );
}
