"use client";

import { useState } from "react";

import { useRouter } from "next/navigation";

import { ChevronsUpDown, GraduationCap, Plus } from "lucide-react";

import type { OrganizationSummaryModel } from "@workspace/types/company";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@workspace/ui/components/popover";

import { useOrganization } from "./provider";

/**
 * Dropdown for switching between organisations.
 * Shows current org name with its abbreviated initials as avatar.
 */
export function OrganizationSelector() {
  const { currentOrg, organizations, isLoading, selectOrganization } =
    useOrganization();
  const [open, setOpen] = useState(false);
  const router = useRouter();

  function handleSelect(org: OrganizationSummaryModel) {
    selectOrganization(org);
    setOpen(false);
  }

  function handleCreate() {
    setOpen(false);
    router.push("/create-organization");
  }

  if (isLoading) {
    return (
      <button
        type="button"
        disabled
        className="bg-sidebar-accent flex w-full items-center gap-2.5 rounded-xl px-3 py-2.5 text-left opacity-50 transition-colors"
      >
        <div className="bg-sidebar-border flex size-8 shrink-0 items-center justify-center rounded-md">
          <GraduationCap className="text-sidebar-foreground/60 size-4" />
        </div>
        <div className="grid flex-1 text-left text-sm leading-tight">
          <span className="text-sidebar-foreground truncate font-semibold">
            Загрузка...
          </span>
        </div>
      </button>
    );
  }

  if (!currentOrg) {
    return (
      <button
        type="button"
        onClick={handleCreate}
        className="bg-sidebar-accent hover:bg-sidebar-accent/80 flex w-full items-center gap-2.5 rounded-xl px-3 py-2.5 text-left transition-colors"
      >
        <div className="bg-primary text-primary-foreground flex size-8 shrink-0 items-center justify-center rounded-md">
          <Plus className="size-4" />
        </div>
        <div className="grid flex-1 text-left text-sm leading-tight">
          <span className="text-sidebar-foreground truncate font-semibold">
            Создать организацию
          </span>
          <span className="text-sidebar-foreground/50 truncate text-xs">
            Нет организаций
          </span>
        </div>
      </button>
    );
  }

  const initials = currentOrg.shortName.slice(0, 2).toUpperCase();

  return (
    <Popover open={open} onOpenChange={setOpen}>
      <PopoverTrigger asChild>
        <button
          type="button"
          className="bg-sidebar-accent hover:bg-sidebar-accent/80 data-[state=open]:bg-sidebar-accent/80 flex w-full items-center gap-2.5 rounded-xl px-3 py-2.5 text-left transition-colors"
        >
          <div className="bg-sidebar-border text-sidebar-primary flex size-8 shrink-0 items-center justify-center rounded-md text-xs font-bold">
            {initials}
          </div>
          <div className="grid flex-1 text-left text-sm leading-tight">
            <span className="text-sidebar-foreground truncate font-semibold">
              {currentOrg.shortName}
            </span>
            <span className="text-sidebar-foreground/50 truncate text-xs">
              {currentOrg.name}
            </span>
          </div>
          <ChevronsUpDown className="text-sidebar-foreground/40 ml-auto size-4 shrink-0" />
        </button>
      </PopoverTrigger>
      <PopoverContent className="w-64 p-2" align="start">
        <div className="space-y-1">
          <p className="text-muted-foreground px-2 py-1.5 text-xs font-medium">
            Организации
          </p>
          {organizations.map((org) => (
            <button
              key={org.id}
              type="button"
              className={`hover:bg-accent flex w-full items-center gap-2 rounded-md px-2 py-1.5 text-sm transition-colors ${
                org.id === currentOrg.id
                  ? "bg-accent text-accent-foreground"
                  : ""
              }`}
              onClick={() => handleSelect(org)}
            >
              <div className="bg-primary/10 text-primary flex size-6 shrink-0 items-center justify-center rounded text-xs font-bold">
                {org.shortName.slice(0, 2).toUpperCase()}
              </div>
              <p className="flex-1 truncate text-left font-medium">
                {org.shortName}
              </p>
            </button>
          ))}
          <div className="border-border my-1 border-t" />
          <button
            type="button"
            className="hover:bg-accent flex w-full items-center gap-2 rounded-md px-2 py-1.5 text-sm transition-colors"
            onClick={handleCreate}
          >
            <div className="flex size-6 shrink-0 items-center justify-center">
              <Plus className="size-4" />
            </div>
            <span>Создать организацию</span>
          </button>
        </div>
      </PopoverContent>
    </Popover>
  );
}
