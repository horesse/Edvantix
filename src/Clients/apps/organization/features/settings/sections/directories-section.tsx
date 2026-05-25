import { Layers } from "lucide-react";

import type { DirectorySummaryDto } from "@workspace/types/organization";

import { DirectoryCard } from "../components/directory-card";
import { EmptySearch } from "../components/empty-search";
import { SectionHeader } from "../components/section-header";
import { DIRECTORY_CATALOG, DIRECTORY_ICONS, directoryRoute } from "../constants";

interface DirectoriesSectionProps {
  readonly items: readonly DirectorySummaryDto[];
  readonly query: string;
}

export function DirectoriesSection({ items, query }: DirectoriesSectionProps) {
  return (
    <section>
      <SectionHeader
        Icon={Layers}
        title="Справочники организации"
        subtitle="Наборы значений, которые используются в курсах, группах и студентах"
        action={
          <button
            type="button"
            className="text-[13px] font-medium text-indigo-600 hover:underline"
            onClick={() => void 0}
          >
            Импорт и экспорт →
          </button>
        }
      />
      {items.length === 0 ? (
        <EmptySearch query={query} label="справочников" />
      ) : (
        <div className="grid gap-3.5 [grid-template-columns:repeat(auto-fill,minmax(280px,1fr))]">
          {items.map((dir) => {
            const Icon = DIRECTORY_ICONS[dir.icon] ?? DIRECTORY_ICONS["FileText"]!;
            const staticEntry = DIRECTORY_CATALOG[dir.code];
            const href = staticEntry ? directoryRoute(dir.code) : undefined;

            return (
              <DirectoryCard
                key={dir.code}
                code={dir.code}
                name={dir.name}
                description={dir.description}
                Icon={Icon}
                badge={dir.badge}
                activeCount={dir.activeCount}
                archivedCount={dir.archivedCount}
                lastModifiedAt={dir.lastModifiedAt}
                href={href}
                isAvailable={dir.isAvailable}
              />
            );
          })}
        </div>
      )}
    </section>
  );
}
