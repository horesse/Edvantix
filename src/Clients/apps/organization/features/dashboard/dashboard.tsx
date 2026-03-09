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
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@workspace/ui/components/card";
import { Skeleton } from "@workspace/ui/components/skeleton";
import { cn } from "@workspace/ui/lib/utils";

import { PageHeader } from "@/components/page-header";
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
      <div className="grid grid-cols-1 gap-4 lg:grid-cols-2">
        <div className="flex flex-col gap-4">
          <GrowthChart />
          <ActivitySection />
        </div>
        <div className="flex flex-col gap-4">
          {canManage && <QuickActions />}
          <GroupsDistribution orgId={currentOrg.id} />
          <IncomingInvitationsSection />
        </div>
      </div>
    </div>
  );
}

function EmptyState() {
  return (
    <div className="bg-card text-card-foreground flex min-h-100 flex-col items-center justify-center rounded-lg border p-5 shadow-xs">
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

function OrgHeader({ orgId }: { orgId: string }) {
  const { data: org, isLoading } = useOrganization(orgId);

  if (isLoading) {
    return (
      <div className="space-y-1">
        <Skeleton className="h-5 w-48" />
        <Skeleton className="h-4 w-72" />
      </div>
    );
  }

  if (!org) return null;

  return (
    <PageHeader title={org.name} description={org.description ?? undefined} />
  );
}

function KPICards({ orgId }: { orgId: string }) {
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
    <div className="grid grid-cols-2 gap-3 lg:grid-cols-4">
      {kpis.map((kpi) => (
        <div
          key={kpi.title}
          className="bg-card space-y-3 rounded-lg border p-4"
        >
          <div className="flex items-center justify-between">
            <p className="text-muted-foreground text-xs">{kpi.title}</p>
            <div className={cn("rounded-md p-1.5", kpi.bgColor)}>
              <kpi.icon className={cn("size-3.5", kpi.color)} />
            </div>
          </div>
          {isLoading ? (
            <Skeleton className="h-7 w-16" />
          ) : (
            <div className="flex items-end justify-between">
              <p className="text-xl font-semibold tabular-nums">{kpi.value}</p>
              <div
                className={cn(
                  "flex items-center gap-0.5 text-xs",
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
    <Card>
      <CardHeader>
        <CardTitle>Рост участников</CardTitle>
        <CardDescription>Динамика за последние 6 месяцев</CardDescription>
      </CardHeader>
      <CardContent>
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
      </CardContent>
    </Card>
  );
}

function GroupsDistribution({ orgId }: { orgId: string }) {
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
    <Card>
      <CardHeader>
        <CardTitle>Распределение по группам</CardTitle>
        <CardDescription>Участники по категориям</CardDescription>
      </CardHeader>
      <CardContent className="space-y-4">
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
      </CardContent>
    </Card>
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
    <Card>
      <CardHeader>
        <CardTitle>Последняя активность</CardTitle>
        <CardDescription>События за последние 7 дней</CardDescription>
      </CardHeader>
      <CardContent className="space-y-3">
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
      </CardContent>
    </Card>
  );
}

function QuickActions() {
  return (
    <Card>
      <CardHeader>
        <CardTitle>Быстрые действия</CardTitle>
        <CardDescription>Часто используемые операции</CardDescription>
      </CardHeader>
      <CardContent className="flex flex-col gap-1">
        <Button variant="ghost" size="sm" asChild className="justify-start">
          <Link href="/organization/invitations">
            <UserPlus className="size-4" />
            Пригласить участника
          </Link>
        </Button>
        <Button variant="ghost" size="sm" asChild className="justify-start">
          <Link href="/organization/groups">
            <UsersRound className="size-4" />
            Создать группу
          </Link>
        </Button>
      </CardContent>
    </Card>
  );
}
