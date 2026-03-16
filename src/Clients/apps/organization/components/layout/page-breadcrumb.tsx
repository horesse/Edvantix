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

const ROUTE_LABELS: Record<string, string> = {
  organization: "Организация",
  members: "Участники",
  students: "Ученики",
  teachers: "Учителя",
  groups: "Группы",
  invitations: "Приглашения",
  contacts: "Контакты",
  profile: "Профиль",
  career: "Карьера",
  "create-organization": "Создать организацию",
  school: "Школа",
  courses: "Курсы",
  schedule: "Расписание",
  analytics: "Аналитика",
  attendance: "Посещаемость",
  "my-courses": "Мои курсы",
  messages: "Сообщения",
  notifications: "Уведомления",
  settings: "Настройки",
};

function generateBreadcrumbItems(
  pathname: string,
): { label: string; href: string }[] {
  const segments = pathname.split("/").filter(Boolean);
  const items: { label: string; href: string }[] = [];

  let currentPath = "";
  for (let i = 0; i < segments.length; i++) {
    const segment = segments[i];
    if (!segment) continue;

    currentPath += `/${segment}`;

    const label =
      segment === "settings" && segments[i - 1] === "organization"
        ? "Настройки организации"
        : (ROUTE_LABELS[segment] ?? segment);

    items.push({ label, href: currentPath });
  }

  return items;
}

/**
 * Auto-generated breadcrumb from the current pathname.
 * Collapses to ellipsis when more than 3 items deep.
 * On mobile shows only the last segment.
 */
export function PageBreadcrumb({
  items = [],
  currentPage,
}: PageBreadcrumbProps) {
  const pathname = usePathname();

  const allItems = items.length > 0 ? items : generateBreadcrumbItems(pathname);
  const displayedItems =
    allItems.length > 3 ? [allItems[0], ...allItems.slice(-2)] : allItems;
  const hasEllipsis = allItems.length > 3;
  const lastItem = allItems[allItems.length - 1];

  return (
    <Breadcrumb>
      <BreadcrumbList>
        {/* Desktop */}
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

        {/* Mobile — home + last segment only */}
        <div className="contents md:hidden">
          <BreadcrumbItem>
            <BreadcrumbLink asChild>
              <Link href="/">
                <Home className="size-4" />
                <span className="sr-only">Главная</span>
              </Link>
            </BreadcrumbLink>
          </BreadcrumbItem>
          {allItems.length > 0 && (
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
