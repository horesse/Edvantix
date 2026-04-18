"use client";

import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
} from "react";

import useOrganizations from "@workspace/api-hooks/company/useOrganizations";
import type { OrganizationDto } from "@workspace/types/company";

const STORAGE_KEY = "selectedOrgId";

type OrganizationContextValue = {
  currentOrg: OrganizationDto | null;
  organizations: OrganizationDto[];
  isLoading: boolean;
  selectOrganization: (org: OrganizationDto) => void;
  /**
   * Роль пользователя в текущей организации.
   * Пока возвращает null — бэкенд не включает роль в список организаций.
   */
  userRole: null;
  /** Временно true — до получения ролей от API. */
  canManage: boolean;
};

const OrganizationContext = createContext<OrganizationContextValue | null>(
  null,
);

export function OrganizationProvider({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  const { data: orgPage, isLoading } = useOrganizations();
  const organizations = orgPage?.items ?? [];
  const [selectedOrgId, setSelectedOrgId] = useState<string | null>(null);

  // Restore persisted selection on mount.
  useEffect(() => {
    const stored = localStorage.getItem(STORAGE_KEY);
    if (stored) setSelectedOrgId(stored);
  }, []);

  // Auto-select first organisation if none is selected or persisted one no longer exists.
  useEffect(() => {
    if (organizations.length === 0) return;

    const match = selectedOrgId
      ? organizations.find((o) => o.id === selectedOrgId)
      : null;

    if (!match) {
      const first = organizations[0];
      if (first) {
        setSelectedOrgId(first.id);
        localStorage.setItem(STORAGE_KEY, String(first.id));
      }
    }
  }, [organizations, selectedOrgId]);

  const selectOrganization = useCallback((org: OrganizationDto) => {
    setSelectedOrgId(org.id);
    localStorage.setItem(STORAGE_KEY, String(org.id));
  }, []);

  const currentOrg = useMemo(
    () => organizations.find((o) => o.id === selectedOrgId) ?? null,
    [organizations, selectedOrgId],
  );

  const value = useMemo<OrganizationContextValue>(
    () => ({
      currentOrg,
      organizations,
      isLoading,
      selectOrganization,
      userRole: null,
      canManage: currentOrg !== null,
    }),
    [currentOrg, organizations, isLoading, selectOrganization],
  );

  return <OrganizationContext value={value}>{children}</OrganizationContext>;
}

export function useOrganization() {
  const ctx = useContext(OrganizationContext);
  if (!ctx) {
    throw new Error("useOrganization must be used within OrganizationProvider");
  }
  return ctx;
}
