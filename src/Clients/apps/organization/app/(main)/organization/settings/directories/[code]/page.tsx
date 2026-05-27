import type { Metadata } from "next";
import { notFound } from "next/navigation";

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
import {
  directoryRegistry,
  isDirectoryImplemented,
} from "@/features/directories/registry";
import { DirectoryPage } from "@/features/directories/directory-page";

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

export default async function DirectoryCodePage({ params }: Props) {
  const { code } = await params;
  const entry = DIRECTORY_CATALOG[code];

  if (!entry) notFound();

  if (isDirectoryImplemented(code)) {
    const config = directoryRegistry[code];
    return <DirectoryPage config={config} />;
  }

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
      <Empty className="min-h-[320px] border-dashed">
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
