"use client";

import Link from "next/link";

import { Building, Plus, UserPlus, Users, UsersRound } from "lucide-react";

import useMyInvitations from "@workspace/api-hooks/company/useMyInvitations";
import useOrganization from "@workspace/api-hooks/company/useOrganization";
import { InvitationStatus } from "@workspace/types/company";
import { Button } from "@workspace/ui/components/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@workspace/ui/components/card";
import { Skeleton } from "@workspace/ui/components/skeleton";

import { useOrganization as useOrgContext } from "@/components/organization-provider";
import { organizationRoleLabels } from "@/lib/company-options";

import { IncomingInvitationsSection } from "../invitations/incoming-invitations-section";

export function Dashboard() {
  const { currentOrg, userRole, canManage } = useOrgContext();

  if (!currentOrg) {
    return <EmptyState />;
  }

  return (
    <div className="space-y-6">
      <OrgHeader orgId={currentOrg.id} />
      <StatsCards orgId={currentOrg.id} />
      {canManage && <QuickActions />}
      <IncomingInvitationsSection />
    </div>
  );
}

function EmptyState() {
  return (
    <div className="flex flex-col items-center justify-center py-16">
      <Building className="text-muted-foreground/50 mb-4 size-16" />
      <h2 className="text-xl font-semibold">Нет организаций</h2>
      <p className="text-muted-foreground mt-1 text-sm">
        Создайте свою первую организацию, чтобы начать работу
      </p>
      <Button asChild className="mt-4">
        <Link href="/create-organization">
          <Plus className="size-4" />
          Создать организацию
        </Link>
      </Button>
    </div>
  );
}

function OrgHeader({ orgId }: { orgId: number }) {
  const { data: org, isLoading } = useOrganization(orgId);

  if (isLoading) {
    return (
      <div className="space-y-2">
        <Skeleton className="h-8 w-64" />
        <Skeleton className="h-4 w-96" />
      </div>
    );
  }

  if (!org) return null;

  return (
    <div>
      <h1 className="text-3xl font-bold">{org.name}</h1>
      {org.description && (
        <p className="text-muted-foreground mt-1">{org.description}</p>
      )}
    </div>
  );
}

function StatsCards({ orgId }: { orgId: number }) {
  const { data: org, isLoading } = useOrganization(orgId);

  const stats = [
    {
      title: "Участники",
      value: org?.membersCount ?? 0,
      icon: Users,
      href: "/members",
    },
    {
      title: "Группы",
      value: org?.groupsCount ?? 0,
      icon: UsersRound,
      href: "/groups",
    },
  ];

  return (
    <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
      {stats.map((stat) => (
        <Link key={stat.title} href={stat.href}>
          <Card className="hover:bg-muted/50 transition-colors">
            <CardHeader className="flex flex-row items-center justify-between pb-2">
              <CardTitle className="text-sm font-medium">
                {stat.title}
              </CardTitle>
              <stat.icon className="text-muted-foreground size-4" />
            </CardHeader>
            <CardContent>
              {isLoading ? (
                <Skeleton className="h-8 w-16" />
              ) : (
                <p className="text-2xl font-bold">{stat.value}</p>
              )}
            </CardContent>
          </Card>
        </Link>
      ))}
    </div>
  );
}

function QuickActions() {
  return (
    <Card>
      <CardHeader>
        <CardTitle className="text-base">Быстрые действия</CardTitle>
      </CardHeader>
      <CardContent className="flex flex-wrap gap-2">
        <Button variant="outline" size="sm" asChild>
          <Link href="/invitations">
            <UserPlus className="size-4" />
            Пригласить участника
          </Link>
        </Button>
        <Button variant="outline" size="sm" asChild>
          <Link href="/groups">
            <UsersRound className="size-4" />
            Создать группу
          </Link>
        </Button>
      </CardContent>
    </Card>
  );
}
