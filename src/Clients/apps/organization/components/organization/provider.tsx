"use client";

import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
} from "react";

import useMyOrganizations from "@workspace/api-hooks/company/useMyOrganizations";
import type { OrganizationSummaryModel } from "@workspace/types/company";
import { OrganizationRole } from "@workspace/types/company";

import { parseOrganizationRole } from "@/lib/company-options";

const STORAGE_KEY = "selectedOrgId";

type OrganizationContextValue = {
  currentOrg: OrganizationSummaryModel | null;
  organizations: OrganizationSummaryModel[];
  isLoading: boolean;
  selectOrganization: (org: OrganizationSummaryModel) => void;
  userRole: OrganizationRole | null;
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
  const { data: organizations = [], isLoading } = useMyOrganizations();
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

  const selectOrganization = useCallback((org: OrganizationSummaryModel) => {
    setSelectedOrgId(org.id);
    localStorage.setItem(STORAGE_KEY, String(org.id));
  }, []);

  const currentOrg = useMemo(
    () => organizations.find((o) => o.id === selectedOrgId) ?? null,
    [organizations, selectedOrgId],
  );

  const userRole = useMemo(
    () => (currentOrg ? parseOrganizationRole(currentOrg.role) : null),
    [currentOrg],
  );

  const canManage = useMemo(
    () =>
      userRole === OrganizationRole.Owner ||
      userRole === OrganizationRole.Manager,
    [userRole],
  );

  const value = useMemo<OrganizationContextValue>(
    () => ({
      currentOrg,
      organizations,
      isLoading,
      selectOrganization,
      userRole,
      canManage,
    }),
    [
      currentOrg,
      organizations,
      isLoading,
      selectOrganization,
      userRole,
      canManage,
    ],
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
