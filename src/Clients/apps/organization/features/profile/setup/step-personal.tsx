"use client";

import { useState } from "react";

import { ChevronDown, ChevronLeft } from "lucide-react";

import { Button } from "@workspace/ui/components/button";
import { Input } from "@workspace/ui/components/input";
import { Label } from "@workspace/ui/components/label";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@workspace/ui/components/popover";
import { cn } from "@workspace/ui/lib/utils";

import { COUNTRIES } from "./schema";
import type { PersonalStepValues } from "./schema";

interface StepPersonalProps {
  values: PersonalStepValues;
  onChange: (values: PersonalStepValues) => void;
  onBack: () => void;
  onSubmit: () => void;
  isSubmitting: boolean;
  error?: string | null;
}

const GENDER_OPTIONS = [
  { value: "male" as const, label: "Мужской", icon: "♂" },
  { value: "female" as const, label: "Женский", icon: "♀" },
  { value: "other" as const, label: "Не указывать" },
] as const;

/**
 * Step 2 of profile setup: personal data form.
 * Collects last name, first name, patronymic, birth date, gender, and phone.
 */
export function StepPersonal({
  values,
  onChange,
  onBack,
  onSubmit,
  isSubmitting,
  error,
}: StepPersonalProps) {
  const [countryOpen, setCountryOpen] = useState(false);
  const [countrySearch, setCountrySearch] = useState("");

  const selectedCountry =
    COUNTRIES.find((c) => c.code === values.countryCode) ?? COUNTRIES[0];

  const filteredCountries = COUNTRIES.filter(
    (c) =>
      c.name.toLowerCase().includes(countrySearch.toLowerCase()) ||
      c.code.includes(countrySearch),
  );

  function set<K extends keyof PersonalStepValues>(
    key: K,
    value: PersonalStepValues[K],
  ) {
    onChange({ ...values, [key]: value });
  }

  return (
    <div className="p-7">
      <h2 className="text-foreground mb-1 text-base font-semibold">
        Персональные данные
      </h2>
      <p className="text-muted-foreground mb-6 text-sm">
        Расскажите немного о себе
      </p>

      <div className="space-y-4">
        {/* Last name */}
        <div className="space-y-1.5">
          <Label htmlFor="lastName">
            Фамилия <span className="text-destructive">*</span>
          </Label>
          <Input
            id="lastName"
            autoComplete="family-name"
            placeholder="Иванов"
            value={values.lastName}
            onChange={(e) => set("lastName", e.target.value)}
          />
        </div>

        {/* First name + patronymic */}
        <div className="grid grid-cols-2 gap-3">
          <div className="space-y-1.5">
            <Label htmlFor="firstName">
              Имя <span className="text-destructive">*</span>
            </Label>
            <Input
              id="firstName"
              autoComplete="given-name"
              placeholder="Иван"
              value={values.firstName}
              onChange={(e) => set("firstName", e.target.value)}
            />
          </div>
          <div className="space-y-1.5">
            <Label htmlFor="patronymic">Отчество</Label>
            <Input
              id="patronymic"
              autoComplete="additional-name"
              placeholder="Иванович"
              value={values.patronymic ?? ""}
              onChange={(e) => set("patronymic", e.target.value)}
            />
          </div>
        </div>

        {/* Birth date */}
        <div className="space-y-1.5">
          <Label htmlFor="birthDate">
            Дата рождения <span className="text-destructive">*</span>
          </Label>
          <Input
            id="birthDate"
            type="date"
            autoComplete="bday"
            value={values.birthDate}
            onChange={(e) => set("birthDate", e.target.value)}
          />
        </div>

        {/* Gender */}
        <div className="space-y-2">
          <Label>Пол</Label>
          <div className="flex gap-2">
            {GENDER_OPTIONS.map((opt) => (
              <button
                key={opt.value}
                type="button"
                onClick={() => set("gender", opt.value)}
                className={cn(
                  "flex flex-1 items-center justify-center gap-1.5 rounded-lg border px-3 py-2 text-sm font-medium transition-colors",
                  values.gender === opt.value
                    ? "border-primary bg-primary/5 text-primary"
                    : "border-border text-muted-foreground hover:bg-muted",
                )}
              >
                {"icon" in opt && <span>{opt.icon}</span>}
                {opt.label}
              </button>
            ))}
          </div>
        </div>

        {/* Phone */}
        <div className="space-y-1.5">
          <Label htmlFor="phone">Телефон</Label>
          <div className="flex">
            {/* Country code picker */}
            <Popover open={countryOpen} onOpenChange={setCountryOpen}>
              <PopoverTrigger asChild>
                <button
                  type="button"
                  className="border-border bg-background hover:bg-muted flex h-10 items-center gap-1.5 rounded-l-md rounded-r-none border border-r-0 px-3 text-sm whitespace-nowrap transition-colors"
                >
                  <span className="text-base">{selectedCountry.flag}</span>
                  <span className="font-medium">{selectedCountry.code}</span>
                  <ChevronDown className="text-muted-foreground size-3" />
                </button>
              </PopoverTrigger>
              <PopoverContent align="start" className="w-64 p-2">
                <Input
                  placeholder="Поиск страны..."
                  value={countrySearch}
                  onChange={(e) => setCountrySearch(e.target.value)}
                  className="mb-2"
                  autoFocus
                />
                <div className="max-h-52 overflow-y-auto">
                  {filteredCountries.map((country) => (
                    <button
                      key={`${country.name}-${country.code}`}
                      type="button"
                      className="hover:bg-accent flex w-full items-center gap-3 rounded-md px-2 py-1.5 text-sm transition-colors"
                      onClick={() => {
                        set("countryCode", country.code);
                        setCountryOpen(false);
                        setCountrySearch("");
                      }}
                    >
                      <span className="w-6 text-center text-base">
                        {country.flag}
                      </span>
                      <span className="flex-1 text-left">{country.name}</span>
                      <span className="text-muted-foreground text-xs font-medium">
                        {country.code}
                      </span>
                    </button>
                  ))}
                </div>
              </PopoverContent>
            </Popover>

            {/* Phone number input */}
            <Input
              id="phone"
              type="tel"
              autoComplete="tel"
              placeholder="(29) 123-45-67"
              value={values.phone ?? ""}
              onChange={(e) => set("phone", e.target.value)}
              className="rounded-l-none"
            />
          </div>
        </div>

        {/* Error message */}
        {error && (
          <div className="border-destructive/50 bg-destructive/10 text-destructive rounded-lg border px-4 py-3 text-sm">
            {error}
          </div>
        )}
      </div>

      <div className="mt-7 flex gap-3">
        <Button variant="outline" className="flex-1" onClick={onBack}>
          <ChevronLeft className="mr-1 size-4" />
          Назад
        </Button>
        <Button className="flex-[2]" onClick={onSubmit} disabled={isSubmitting}>
          {isSubmitting ? "Сохранение..." : "Сохранить и войти"}
        </Button>
      </div>
    </div>
  );
}
