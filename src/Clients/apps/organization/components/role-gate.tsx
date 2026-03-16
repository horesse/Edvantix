"use client";

import type { OrganizationRole } from "@workspace/types/company";

import { useOrganization } from "@/components/organization/provider";

type RoleGateProps = {
  allowedRoles: OrganizationRole[];
  children: React.ReactNode;
  fallback?: React.ReactNode;
};

/** Display-only guard that conditionally renders children based on the user's role. */
export function RoleGate({
  allowedRoles,
  children,
  fallback = null,
}: RoleGateProps) {
  const { userRole } = useOrganization();

  if (!userRole || !allowedRoles.includes(userRole)) {
    return <>{fallback}</>;
  }

  return <>{children}</>;
}
