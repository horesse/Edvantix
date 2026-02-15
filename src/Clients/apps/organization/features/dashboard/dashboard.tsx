"use client";

import Link from "next/link";

import { Building, Plus, TrendingUp, UserPlus, Users, UsersRound } from "lucide-react";

import useOrganization from "@workspace/api-hooks/company/useOrganization";
import { Button } from "@workspace/ui/components/button";
import {
  Island,
  IslandContent,
  IslandDescription,
  IslandHeader,
  IslandTitle,
} from "@workspace/ui/components/island";
import {
  IslandColumn,
  IslandLayout,
} from "@workspace/ui/components/island-layout";
import { Separator } from "@workspace/ui/components/separator";
import { Skeleton } from "@workspace/ui/components/skeleton";

import { useOrganization as useOrgContext } from "@/components/organization-provider";

import { IncomingInvitationsSection } from "../invitations/incoming-invitations-section";

export function Dashboard() {
  const { currentOrg, canManage } = useOrgContext();

  if (!currentOrg) {
    return <EmptyState />;
  }

  return (
    <IslandLayout>
      <IslandColumn>
        <OrgHeader orgId={currentOrg.id} />
        <StatsCards orgId={currentOrg.id} />
      </IslandColumn>
      <IslandColumn>
        {canManage && <QuickActions />}
        <IncomingInvitationsSection />
      </IslandColumn>
    </IslandLayout>
  );
}

function EmptyState() {
  return (
    <Island className="flex min-h-[400px] flex-col items-center justify-center">
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
    </Island>
  );
}

function OrgHeader({ orgId }: { orgId: number }) {
  const { data: org, isLoading } = useOrganization(orgId);

  if (isLoading) {
    return (
      <Island>
        <IslandHeader>
          <Skeleton className="h-8 w-64" />
          <Skeleton className="h-4 w-96" />
        </IslandHeader>
      </Island>
    );
  }

  if (!org) return null;

  return (
    <Island>
      <IslandHeader>
        <IslandTitle>{org.name}</IslandTitle>
        {org.description && (
          <IslandDescription>{org.description}</IslandDescription>
        )}
      </IslandHeader>
    </Island>
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
      change: "+12%",
    },
    {
      title: "Группы",
      value: org?.groupsCount ?? 0,
      icon: UsersRound,
      href: "/groups",
      change: "+8%",
    },
  ];

  return (
    <Island>
      <IslandHeader>
        <IslandTitle>Статистика</IslandTitle>
        <IslandDescription>Обзор активности организации</IslandDescription>
      </IslandHeader>
      <IslandContent className="space-y-4">
        {stats.map((stat, index) => (
          <div key={stat.title}>
            {index > 0 && <Separator className="mb-4" />}
            <Link
              href={stat.href}
              className="group block transition-opacity hover:opacity-80"
            >
              <div className="flex items-center justify-between">
                <div className="flex items-center gap-3">
                  <div className="flex size-10 items-center justify-center rounded-lg bg-primary/10 text-primary">
                    <stat.icon className="size-5" />
                  </div>
                  <div>
                    <p className="text-muted-foreground text-sm font-medium">
                      {stat.title}
                    </p>
                    {isLoading ? (
                      <Skeleton className="mt-1 h-8 w-16" />
                    ) : (
                      <p className="text-2xl font-bold">{stat.value}</p>
                    )}
                  </div>
                </div>
                <div className="flex items-center gap-1 text-green-600 dark:text-green-400">
                  <TrendingUp className="size-4" />
                  <span className="text-sm font-medium">{stat.change}</span>
                </div>
              </div>
            </Link>
          </div>
        ))}
      </IslandContent>
    </Island>
  );
}

function QuickActions() {
  return (
    <Island>
      <IslandHeader>
        <IslandTitle>Быстрые действия</IslandTitle>
        <IslandDescription>
          Часто используемые операции
        </IslandDescription>
      </IslandHeader>
      <IslandContent className="flex flex-col gap-2">
        <Button variant="outline" asChild className="justify-start">
          <Link href="/invitations">
            <UserPlus className="size-4" />
            Пригласить участника
          </Link>
        </Button>
        <Button variant="outline" asChild className="justify-start">
          <Link href="/groups">
            <UsersRound className="size-4" />
            Создать группу
          </Link>
        </Button>
      </IslandContent>
    </Island>
  );
}
