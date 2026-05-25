import { notFound } from "next/navigation";
import type { Metadata } from "next";

import { Wrench } from "lucide-react";

import {
  Empty,
  EmptyContent,
  EmptyDescription,
  EmptyHeader,
  EmptyMedia,
  EmptyTitle,
} from "@workspace/ui/components/empty";

import { PageBreadcrumb } from "@/components/layout/page-breadcrumb";
import { PageLayout } from "@/components/layout/page-layout";

import { DIRECTORY_CATALOG } from "@/features/settings/constants";

type Props = {
  params: Promise<{ code: string }>;
};

export async function generateMetadata({ params }: Props): Promise<Metadata> {
  const { code } = await params;
  const entry = DIRECTORY_CATALOG[code];
  return {
    title: entry ? `Edvantix — ${entry.name}` : "Edvantix — Справочник",
  };
}

export default async function DirectoryStubPage({ params }: Props) {
  const { code } = await params;
  const entry = DIRECTORY_CATALOG[code];

  if (!entry) notFound();

  return (
    <PageLayout
      header={
        <PageBreadcrumb
          items={[{ label: "Настройки", href: "/organization/settings" }]}
          currentPage={entry.name}
        />
      }
      title={entry.name}
      description={entry.description}
      back={{ href: "/organization/settings", label: "Назад к настройкам" }}
    >
      <Empty className="border-dashed min-h-[320px]">
        <EmptyHeader>
          <EmptyMedia variant="icon">
            <Wrench />
          </EmptyMedia>
          <EmptyTitle>Раздел в разработке</EmptyTitle>
        </EmptyHeader>
        <EmptyContent>
          <EmptyDescription>
            Справочник «{entry.name}» скоро будет доступен.
          </EmptyDescription>
        </EmptyContent>
      </Empty>
    </PageLayout>
  );
}
