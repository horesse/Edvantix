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

export function PageBreadcrumb({ items = [], currentPage }: PageBreadcrumbProps) {
  const pathname = usePathname();

  const allItems = items.length > 0 ? items : generateBreadcrumbItems(pathname);
  const displayedItems = allItems.length > 3 ? [allItems[0]!, ...allItems.slice(-2)] : allItems;
  const hasEllipsis = allItems.length > 3;

  return (
    <Breadcrumb>
      <BreadcrumbList>
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
      </BreadcrumbList>
    </Breadcrumb>
  );
}

function generateBreadcrumbItems(pathname: string): { label: string; href: string }[] {
  const segments = pathname.split("/").filter(Boolean);
  const items: { label: string; href: string }[] = [];

  const routeLabels: Record<string, string> = {
    "org-settings": "Настройки организации",
    "members": "Участники",
    "groups": "Группы",
    "invitations": "Приглашения",
    "contacts": "Контакты",
    "settings": "Настройки",
    "profile": "Профиль",
    "career": "Карьера",
    "create-organization": "Создать организацию",
  };

  let currentPath = "";
  for (const segment of segments) {
    currentPath += `/${segment}`;
    const label = routeLabels[segment] ?? segment;
    items.push({ label, href: currentPath });
  }

  return items;
}
