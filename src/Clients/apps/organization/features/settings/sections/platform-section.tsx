import { Sparkles } from "lucide-react";

import { PlatformCard } from "../components/platform-card";
import { SectionHeader } from "../components/section-header";
import type { PlatformItem } from "../constants";

interface PlatformSectionProps {
  readonly items: readonly PlatformItem[];
}

export function PlatformSection({ items }: PlatformSectionProps) {
  return (
    <section>
      <SectionHeader
        Icon={Sparkles}
        title="Платформа"
        subtitle="Подключения, оповещения, безопасность и тариф"
      />
      <div className="grid [grid-template-columns:repeat(auto-fill,minmax(280px,1fr))] gap-3.5">
        {items.map((item) => (
          <PlatformCard key={item.id} item={item} />
        ))}
      </div>
    </section>
  );
}
