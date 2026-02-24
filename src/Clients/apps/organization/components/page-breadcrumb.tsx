"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";

import { ChevronRight, Home } from "lucide-react";

import {
  Breadcrumb,
  BreadcrumbEllipsis,
  BreadcrumbItem,
  BreadcrumbLink,
  BreadcrumbList,
  BreadcrumbPage,
  BreadcrumbSeparator,
} from "@workspace/ui/components/breadcrumb";

type PageBreadcrumbProps = Readonly<{
  items?: { label: string; href: string }[];
  currentPage?: string;
}>;

export function PageBreadcrumb({
  items = [],
  currentPage,
}: PageBreadcrumbProps) {
  const pathname = usePathname();

  const allItems = items.length > 0 ? items : generateBreadcrumbItems(pathname);
  const displayedItems =
    allItems.length > 3 ? [allItems[0]!, ...allItems.slice(-2)] : allItems;
  const hasEllipsis = allItems.length > 3;

  // На мобильных показываем только последний элемент
  const lastItem = allItems[allItems.length - 1];
  const showMobileCompact = allItems.length > 0;

  return (
    <Breadcrumb>
      <BreadcrumbList>
        {/* Desktop версия */}
        <div className="hidden md:contents">
          <BreadcrumbItem>
            <BreadcrumbLink asChild>
              <Link href="/">
                <Home className="size-4" />
                <span className="sr-only">Главная</span>
              </Link>
            </BreadcrumbLink>
          </BreadcrumbItem>
          {hasEllipsis && (
            <>
              <BreadcrumbSeparator>
                <ChevronRight />
              </BreadcrumbSeparator>
              <BreadcrumbItem>
                <BreadcrumbEllipsis />
              </BreadcrumbItem>
            </>
          )}
          {displayedItems.map((item) => (
            <span key={item.href} className="contents">
              <BreadcrumbSeparator>
                <ChevronRight />
              </BreadcrumbSeparator>
              <BreadcrumbItem>
                <BreadcrumbLink asChild>
                  <Link href={item.href}>{item.label}</Link>
                </BreadcrumbLink>
              </BreadcrumbItem>
            </span>
          ))}
          {currentPage && (
            <>
              <BreadcrumbSeparator>
                <ChevronRight />
              </BreadcrumbSeparator>
              <BreadcrumbItem>
                <BreadcrumbPage>{currentPage}</BreadcrumbPage>
              </BreadcrumbItem>
            </>
          )}
        </div>

        {/* Mobile версия - только последний элемент */}
        <div className="contents md:hidden">
          <BreadcrumbItem>
            <BreadcrumbLink asChild>
              <Link href="/">
                <Home className="size-4" />
                <span className="sr-only">Главная</span>
              </Link>
            </BreadcrumbLink>
          </BreadcrumbItem>
          {showMobileCompact && (
            <>
              <BreadcrumbSeparator>
                <ChevronRight />
              </BreadcrumbSeparator>
              <BreadcrumbItem>
                <BreadcrumbEllipsis />
              </BreadcrumbItem>
              <BreadcrumbSeparator>
                <ChevronRight />
              </BreadcrumbSeparator>
              <BreadcrumbItem>
                {currentPage ? (
                  <BreadcrumbPage>{currentPage}</BreadcrumbPage>
                ) : (
                  <BreadcrumbPage>{lastItem?.label}</BreadcrumbPage>
                )}
              </BreadcrumbItem>
            </>
          )}
        </div>
      </BreadcrumbList>
    </Breadcrumb>
  );
}

function generateBreadcrumbItems(
  pathname: string,
): { label: string; href: string }[] {
  const segments = pathname.split("/").filter(Boolean);
  const items: { label: string; href: string }[] = [];

  const routeLabels: Record<string, string> = {
    organization: "Организация",
    members: "Участники",
    groups: "Группы",
    invitations: "Приглашения",
    contacts: "Контакты",
    profile: "Профиль",
    career: "Карьера",
    "create-organization": "Создать организацию",
    // School management routes
    school: "Школа",
    courses: "Курсы",
    students: "Ученики",
    teachers: "Преподаватели",
    schedule: "Расписание",
    finance: "Финансы",
    crm: "CRM",
    analytics: "Аналитика",
    "my-courses": "Мои курсы",
    messages: "Сообщения",
    notifications: "Уведомления",
  };

  let currentPath = "";
  for (let i = 0; i < segments.length; i++) {
    const segment = segments[i];
    if (!segment) continue;

    currentPath += `/${segment}`;

    // Специальная логика для "settings"
    let label: string;
    if (segment === "settings") {
      // Если это /organization/settings - "Настройки организации"
      // Если это просто /settings - "Настройки"
      const isOrgSettings = segments[i - 1] === "organization";
      label = isOrgSettings ? "Настройки организации" : "Настройки";
    } else {
      label = routeLabels[segment] ?? segment;
    }

    items.push({ label, href: currentPath });
  }

  return items;
}
