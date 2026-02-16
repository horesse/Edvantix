"use client";

import { useState } from "react";

import { useRouter } from "next/navigation";

import { ChevronsUpDown, GraduationCap, Plus } from "lucide-react";

import type { OrganizationSummaryModel } from "@workspace/types/company";
import { Button } from "@workspace/ui/components/button";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@workspace/ui/components/popover";

import { useOrganization } from "./organization-provider";

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
      <Button
        variant="outline"
        className="h-auto w-full justify-start gap-3 px-3 py-2"
      >
        <div className="flex size-8 items-center justify-center rounded-lg bg-primary text-primary-foreground">
          <GraduationCap className="size-4" />
        </div>
        <div className="grid flex-1 text-left text-sm leading-tight">
          <span className="truncate font-semibold">Загрузка...</span>
        </div>
      </Button>
    );
  }

  if (!currentOrg) {
    return (
      <Button
        variant="outline"
        className="h-auto w-full justify-start gap-3 px-3 py-2"
        onClick={handleCreate}
      >
        <div className="flex size-8 items-center justify-center rounded-lg bg-primary text-primary-foreground">
          <Plus className="size-4" />
        </div>
        <div className="grid flex-1 text-left text-sm leading-tight">
          <span className="truncate font-semibold">Создать организацию</span>
          <span className="text-muted-foreground truncate text-xs">
            Нет организаций
          </span>
        </div>
      </Button>
    );
  }

  return (
    <Popover open={open} onOpenChange={setOpen}>
      <PopoverTrigger asChild>
        <Button
          variant="outline"
          className="h-auto w-full justify-start gap-3 px-3 py-2 data-[state=open]:bg-accent"
        >
          <div className="flex size-8 items-center justify-center rounded-lg bg-primary text-xs font-bold text-primary-foreground">
            {currentOrg.shortName.charAt(0).toUpperCase()}
          </div>
          <div className="grid flex-1 text-left text-sm leading-tight">
            <span className="truncate font-semibold">
              {currentOrg.shortName}
            </span>
            <span className="text-muted-foreground truncate text-xs">
              {currentOrg.name}
            </span>
          </div>
          <ChevronsUpDown className="ml-auto size-4" />
        </Button>
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
              <div className="bg-primary text-primary-foreground flex size-6 shrink-0 items-center justify-center rounded text-xs font-bold">
                {org.shortName.charAt(0).toUpperCase()}
              </div>
              <div className="min-w-0 flex-1 text-left">
                <p className="truncate font-medium">{org.shortName}</p>
              </div>
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
