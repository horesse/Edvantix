"use client";

import {
  forwardRef,
  useEffect,
  useImperativeHandle,
  useRef,
  useState,
} from "react";

import { Tag, X } from "lucide-react";
import { toast } from "sonner";

import { useSearchSkills } from "@workspace/api-hooks/profiles/useSearchSkills";
import type { OwnProfileDetails } from "@workspace/types/profile";
import { cn } from "@workspace/ui/lib/utils";

import type { SkillsSectionHandle } from "../types";

const MAX_SKILLS = 20;

export const SubjectsCard = forwardRef<
  SkillsSectionHandle,
  {
    profile: OwnProfileDetails;
    onDirtyChange?: (dirty: boolean) => void;
  }
>(function SubjectsCard({ profile, onDirtyChange }, ref) {
  const [skills, setSkills] = useState<string[]>(() =>
    profile.skills.map((s) => s.name),
  );
  const [savedSnapshot, setSavedSnapshot] = useState(skills);
  const [inputValue, setInputValue] = useState("");
  const [dropdownOpen, setDropdownOpen] = useState(false);
  const inputRef = useRef<HTMLInputElement>(null);
  const containerRef = useRef<HTMLDivElement>(null);

  const { data: suggestions } = useSearchSkills(inputValue);

  const isDirty =
    JSON.stringify([...skills].sort()) !==
    JSON.stringify([...savedSnapshot].sort());

  useEffect(() => {
    onDirtyChange?.(isDirty);
  }, [isDirty, onDirtyChange]);

  useImperativeHandle(ref, () => ({
    getPayload(): string[] {
      return skills;
    },
    acknowledgeServerState() {
      setSavedSnapshot(skills);
    },
  }));

  function addSkill(name: string) {
    const trimmed = name.trim();
    if (!trimmed) return;
    if (skills.some((s) => s.toLowerCase() === trimmed.toLowerCase())) {
      toast.info("Этот навык уже добавлен");
      return;
    }
    if (skills.length >= MAX_SKILLS) {
      toast.error(`Максимум ${MAX_SKILLS} навыков`);
      return;
    }
    setSkills((prev) => [...prev, trimmed]);
    setInputValue("");
    setDropdownOpen(false);
  }

  function removeSkill(name: string) {
    setSkills((prev) => prev.filter((s) => s !== name));
  }

  function handleKeyDown(e: React.KeyboardEvent<HTMLInputElement>) {
    if (e.key === "Enter") {
      e.preventDefault();
      addSkill(inputValue);
    }
    if (e.key === "Backspace" && !inputValue && skills.length > 0) {
      setSkills((prev) => prev.slice(0, -1));
    }
    if (e.key === "Escape") {
      setDropdownOpen(false);
    }
  }

  // Close dropdown when clicking outside the card
  useEffect(() => {
    function handleClickOutside(e: MouseEvent) {
      if (
        containerRef.current &&
        !containerRef.current.contains(e.target as Node)
      ) {
        setDropdownOpen(false);
      }
    }
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  const filteredSuggestions =
    suggestions?.filter(
      (s) => !skills.some((sk) => sk.toLowerCase() === s.name.toLowerCase()),
    ) ?? [];

  return (
    <div className="bg-card border-border rounded-2xl border p-5 shadow-sm">
      {/* Header */}
      <div className="mb-4 flex items-center gap-2">
        <div className="flex size-6 items-center justify-center rounded-lg bg-violet-100">
          <Tag className="size-3.5 text-violet-600" />
        </div>
        <span className="text-foreground text-sm font-semibold">
          Навыки / Предметы
        </span>
        {skills.length > 0 && (
          <span className="text-muted-foreground ml-auto text-[11px]">
            {skills.length}/{MAX_SKILLS}
          </span>
        )}
      </div>

      {/* Current skill tags */}
      {skills.length > 0 && (
        <div className="mb-3 flex flex-wrap gap-2">
          {skills.map((skill) => (
            <span
              key={skill}
              className={[
                "inline-flex items-center gap-1.5 rounded-full border px-2.5 py-1.5 text-xs font-medium",
                "border-brand-200 bg-brand-50 text-brand-700",
                "dark:border-brand-500/35 dark:bg-brand-500/[0.18] dark:text-brand-300",
              ].join(" ")}
            >
              {skill}
              <button
                type="button"
                onClick={() => removeSkill(skill)}
                className="text-brand-400 hover:text-brand-700 dark:text-brand-400 dark:hover:text-brand-300 transition-colors"
                aria-label={`Удалить навык ${skill}`}
              >
                <X className="size-3" />
              </button>
            </span>
          ))}
        </div>
      )}

      {/* Input + autocomplete dropdown */}
      <div ref={containerRef} className="relative">
        <div className="flex gap-2">
          <input
            ref={inputRef}
            type="text"
            value={inputValue}
            onChange={(e) => {
              setInputValue(e.target.value);
              setDropdownOpen(e.target.value.trim().length >= 1);
            }}
            onFocus={() => {
              if (inputValue.trim().length >= 1) setDropdownOpen(true);
            }}
            onKeyDown={handleKeyDown}
            placeholder="Добавить навык…"
            disabled={skills.length >= MAX_SKILLS}
            className={cn(
              "border-border bg-muted/40 text-foreground flex-1 rounded-lg border px-3 py-2 text-xs",
              "placeholder:text-muted-foreground transition outline-none",
              "focus:border-brand-400 focus:ring-brand-100 focus:ring-2",
              "dark:bg-surface-850 dark:border-surface-700",
              "disabled:cursor-not-allowed disabled:opacity-50",
            )}
          />
          <button
            type="button"
            onClick={() => addSkill(inputValue)}
            disabled={skills.length >= MAX_SKILLS}
            className="bg-primary hover:bg-primary/90 flex size-8 shrink-0 items-center justify-center rounded-lg text-base font-bold text-white transition-colors disabled:cursor-not-allowed disabled:opacity-40"
            aria-label="Добавить навык"
          >
            +
          </button>
        </div>

        {/* Autocomplete dropdown */}
        {dropdownOpen && filteredSuggestions.length > 0 && (
          <ul className="bg-card border-border absolute z-20 mt-1 w-full rounded-xl border py-1 shadow-lg">
            {filteredSuggestions.map((s) => (
              <li key={s.id}>
                <button
                  type="button"
                  onMouseDown={(e) => {
                    // Prevent input blur before click registers
                    e.preventDefault();
                    addSkill(s.name);
                  }}
                  className="hover:bg-muted w-full px-3 py-2 text-left text-xs transition-colors"
                >
                  {s.name}
                </button>
              </li>
            ))}
          </ul>
        )}
      </div>

      <p className="text-muted-foreground mt-2 text-[11px]">
        Нажмите Enter или «+» для добавления
      </p>
    </div>
  );
});
