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
import { SidebarMenuButton } from "@workspace/ui/components/sidebar";

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
      <SidebarMenuButton size="lg">
        <div className="bg-sidebar-primary text-sidebar-primary-foreground flex aspect-square size-8 items-center justify-center rounded-lg">
          <GraduationCap className="size-4" />
        </div>
        <div className="grid flex-1 text-left text-sm leading-tight">
          <span className="truncate font-semibold">Загрузка...</span>
        </div>
      </SidebarMenuButton>
    );
  }

  if (!currentOrg) {
    return (
      <SidebarMenuButton size="lg" onClick={handleCreate}>
        <div className="bg-sidebar-primary text-sidebar-primary-foreground flex aspect-square size-8 items-center justify-center rounded-lg">
          <Plus className="size-4" />
        </div>
        <div className="grid flex-1 text-left text-sm leading-tight">
          <span className="truncate font-semibold">Создать организацию</span>
          <span className="truncate text-xs">Нет организаций</span>
        </div>
      </SidebarMenuButton>
    );
  }

  return (
    <Popover open={open} onOpenChange={setOpen}>
      <PopoverTrigger asChild>
        <SidebarMenuButton
          size="lg"
          className="data-[state=open]:bg-sidebar-accent data-[state=open]:text-sidebar-accent-foreground"
        >
          <div className="bg-sidebar-primary text-sidebar-primary-foreground flex aspect-square size-8 items-center justify-center rounded-lg text-xs font-bold">
            {currentOrg.shortName.charAt(0).toUpperCase()}
          </div>
          <div className="grid flex-1 text-left text-sm leading-tight">
            <span className="truncate font-semibold">
              {currentOrg.shortName}
            </span>
            <span className="truncate text-xs">{currentOrg.name}</span>
          </div>
          <ChevronsUpDown className="ml-auto size-4" />
        </SidebarMenuButton>
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
