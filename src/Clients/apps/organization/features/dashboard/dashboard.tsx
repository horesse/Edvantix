"use client";

import * as React from "react";

import Link from "next/link";

import {
  Activity,
  Building,
  Clock,
  Plus,
  TrendingUp,
  UserPlus,
  Users,
  UsersRound,
} from "lucide-react";

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
import { Skeleton } from "@workspace/ui/components/skeleton";
import { cn } from "@workspace/ui/lib/utils";

import { useOrganization as useOrgContext } from "@/components/organization-provider";

import { IncomingInvitationsSection } from "../invitations/incoming-invitations-section";

export function Dashboard() {
  const { currentOrg, canManage } = useOrgContext();

  if (!currentOrg) {
    return <EmptyState />;
  }

  return (
    <div className="space-y-4">
      {/* Header */}
      <OrgHeader orgId={currentOrg.id} />

      {/* KPI Cards */}
      <KPICards orgId={currentOrg.id} />

      {/* Main Content Grid */}
      <IslandLayout columns={2}>
        <IslandColumn>
          <GrowthChart />
          <ActivitySection />
        </IslandColumn>
        <IslandColumn>
          {canManage && <QuickActions />}
          <GroupsDistribution orgId={currentOrg.id} />
          <IncomingInvitationsSection />
        </IslandColumn>
      </IslandLayout>
    </div>
  );
}

function EmptyState() {
  return (
    <Island className="flex min-h-100 flex-col items-center justify-center">
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
      <Island variant="bordered">
        <IslandHeader>
          <Skeleton className="h-8 w-64" />
          <Skeleton className="h-4 w-96" />
        </IslandHeader>
      </Island>
    );
  }

  if (!org) return null;

  return (
    <Island variant="bordered">
      <IslandHeader>
        <IslandTitle>{org.name}</IslandTitle>
        {org.description && (
          <IslandDescription>{org.description}</IslandDescription>
        )}
      </IslandHeader>
    </Island>
  );
}

function KPICards({ orgId }: { orgId: number }) {
  const { data: org, isLoading } = useOrganization(orgId);

  const kpis = [
    {
      title: "Участники",
      value: org?.membersCount ?? 0,
      change: "+12%",
      trend: "up",
      icon: Users,
      color: "text-blue-600 dark:text-blue-400",
      bgColor: "bg-blue-500/10",
    },
    {
      title: "Группы",
      value: org?.groupsCount ?? 0,
      change: "+8%",
      trend: "up",
      icon: UsersRound,
      color: "text-green-600 dark:text-green-400",
      bgColor: "bg-green-500/10",
    },
    {
      title: "Активные приглашения",
      value: 5,
      change: "-3%",
      trend: "down",
      icon: UserPlus,
      color: "text-orange-600 dark:text-orange-400",
      bgColor: "bg-orange-500/10",
    },
    {
      title: "Новые за неделю",
      value: 8,
      change: "+25%",
      trend: "up",
      icon: TrendingUp,
      color: "text-purple-600 dark:text-purple-400",
      bgColor: "bg-purple-500/10",
    },
  ];

  return (
    <div className="grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-4">
      {kpis.map((kpi) => (
        <div
          key={kpi.title}
          className="bg-card space-y-2 rounded-2xl border p-4 shadow-sm transition-shadow hover:shadow-md"
        >
          <div className="flex items-center justify-between">
            <p className="text-muted-foreground text-xs font-medium">
              {kpi.title}
            </p>
            <div className={cn("rounded-lg p-2", kpi.bgColor)}>
              <kpi.icon className={cn("size-4", kpi.color)} />
            </div>
          </div>
          {isLoading ? (
            <Skeleton className="h-8 w-20" />
          ) : (
            <div className="flex items-end justify-between">
              <p className="text-2xl font-bold">{kpi.value}</p>
              <div
                className={cn(
                  "flex items-center gap-1 text-xs font-medium",
                  kpi.trend === "up"
                    ? "text-green-600 dark:text-green-400"
                    : "text-red-600 dark:text-red-400",
                )}
              >
                <TrendingUp
                  className={cn("size-3", kpi.trend === "down" && "rotate-180")}
                />
                <span>{kpi.change}</span>
              </div>
            </div>
          )}
        </div>
      ))}
    </div>
  );
}

function GrowthChart() {
  const chartData = [
    { month: "Янв", value: 45 },
    { month: "Фев", value: 52 },
    { month: "Мар", value: 48 },
    { month: "Апр", value: 61 },
    { month: "Май", value: 68 },
    { month: "Июн", value: 75 },
  ];

  const maxValue = Math.max(...chartData.map((d) => d.value));

  return (
    <Island variant="bordered">
      <IslandHeader>
        <IslandTitle>Рост участников</IslandTitle>
        <IslandDescription>Динамика за последние 6 месяцев</IslandDescription>
      </IslandHeader>
      <IslandContent>
        <div className="space-y-4">
          <div className="flex h-50 items-end justify-between gap-2">
            {chartData.map((data) => (
              <div
                key={data.month}
                className="flex flex-1 flex-col items-center gap-2"
              >
                <div className="relative w-full">
                  <div
                    className="bg-primary/20 hover:bg-primary/30 w-full rounded-t-md transition-all"
                    style={{ height: `${(data.value / maxValue) * 160}px` }}
                  >
                    <div className="absolute -top-6 left-1/2 -translate-x-1/2 text-xs font-medium">
                      {data.value}
                    </div>
                  </div>
                </div>
                <p className="text-muted-foreground text-xs">{data.month}</p>
              </div>
            ))}
          </div>
          <div className="flex items-center justify-center gap-2 pt-4">
            <div className="flex items-center gap-1 text-green-600 dark:text-green-400">
              <TrendingUp className="size-4" />
              <span className="text-sm font-medium">+67% за период</span>
            </div>
          </div>
        </div>
      </IslandContent>
    </Island>
  );
}

function GroupsDistribution({ orgId }: { orgId: number }) {
  const { data: org } = useOrganization(orgId);

  const groups = [
    { name: "Администраторы", count: 3, color: "bg-blue-500" },
    { name: "Преподаватели", count: 12, color: "bg-green-500" },
    {
      name: "Студенты",
      count: org?.membersCount ? org.membersCount - 15 : 45,
      color: "bg-purple-500",
    },
  ];

  const total = groups.reduce((sum, g) => sum + g.count, 0);

  return (
    <Island variant="bordered">
      <IslandHeader>
        <IslandTitle>Распределение по группам</IslandTitle>
        <IslandDescription>Участники по категориям</IslandDescription>
      </IslandHeader>
      <IslandContent className="space-y-4">
        {groups.map((group) => {
          const percentage = Math.round((group.count / total) * 100);
          return (
            <div key={group.name} className="space-y-2">
              <div className="flex items-center justify-between">
                <div className="flex items-center gap-2">
                  <div className={cn("size-3 rounded-full", group.color)} />
                  <p className="text-sm font-medium">{group.name}</p>
                </div>
                <div className="flex items-center gap-2">
                  <p className="text-muted-foreground text-sm">{group.count}</p>
                  <p className="text-muted-foreground text-xs">
                    ({percentage}%)
                  </p>
                </div>
              </div>
              <div className="bg-muted h-2 w-full overflow-hidden rounded-full">
                <div
                  className={cn("h-full transition-all", group.color)}
                  style={{ width: `${percentage}%` }}
                />
              </div>
            </div>
          );
        })}
      </IslandContent>
    </Island>
  );
}

function ActivitySection() {
  const activities = [
    {
      id: 1,
      user: "Иван Петров",
      action: "присоединился к организации",
      time: "2 часа назад",
    },
    {
      id: 2,
      user: "Мария Сидорова",
      action: "создала группу «Backend Team»",
      time: "5 часов назад",
    },
    {
      id: 3,
      user: "Алексей Иванов",
      action: "пригласил нового участника",
      time: "1 день назад",
    },
  ];

  return (
    <Island variant="bordered">
      <IslandHeader>
        <IslandTitle>Последняя активность</IslandTitle>
        <IslandDescription>События за последние 7 дней</IslandDescription>
      </IslandHeader>
      <IslandContent className="space-y-3">
        {activities.map((activity) => (
          <div
            key={activity.id}
            className="hover:bg-accent/50 flex items-start gap-3 rounded-lg border p-3 transition-colors"
          >
            <div className="bg-primary/10 text-primary flex size-8 shrink-0 items-center justify-center rounded-lg">
              <Activity className="size-4" />
            </div>
            <div className="min-w-0 flex-1">
              <p className="text-sm">
                <span className="font-medium">{activity.user}</span>{" "}
                <span className="text-muted-foreground">{activity.action}</span>
              </p>
              <p className="text-muted-foreground mt-1 flex items-center gap-1 text-xs">
                <Clock className="size-3" />
                {activity.time}
              </p>
            </div>
          </div>
        ))}
      </IslandContent>
    </Island>
  );
}

function QuickActions() {
  return (
    <Island variant="bordered">
      <IslandHeader>
        <IslandTitle>Быстрые действия</IslandTitle>
        <IslandDescription>Часто используемые операции</IslandDescription>
      </IslandHeader>
      <IslandContent className="flex flex-col gap-2">
        <Button variant="outline" asChild className="justify-start">
          <Link href="/organization/invitations">
            <UserPlus className="size-4" />
            Пригласить участника
          </Link>
        </Button>
        <Button variant="outline" asChild className="justify-start">
          <Link href="/organization/groups">
            <UsersRound className="size-4" />
            Создать группу
          </Link>
        </Button>
      </IslandContent>
    </Island>
  );
}
