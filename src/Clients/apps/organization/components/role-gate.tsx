"use client";

import { useOrganization } from "@/components/organization/provider";

type RoleGateProps = {
  children: React.ReactNode;
  fallback?: React.ReactNode;
};

/**
 * Показывает children только когда организация выбрана.
 * Ролевой контроль доступа будет добавлен после реализации ролей в бэкенде.
 */
export function RoleGate({
  children,
  fallback = null,
}: Readonly<RoleGateProps>) {
  const { canManage } = useOrganization();

  if (!canManage) {
    return <>{fallback}</>;
  }

  return <>{children}</>;
}
