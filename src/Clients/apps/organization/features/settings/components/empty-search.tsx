import { Search } from "lucide-react";

interface EmptySearchProps {
  readonly query: string;
  readonly label?: string;
}

export function EmptySearch({ query, label }: EmptySearchProps) {
  const suffix = label ? ` среди ${label}` : "";

  return (
    <div className="rounded-[14px] border border-dashed border-slate-200 bg-white px-6 py-7 text-center text-[13px] text-slate-500">
      По запросу{" "}
      <strong className="text-slate-900">«{query}»</strong>
      {suffix} ничего не найдено.
    </div>
  );
}

/** Заглушка «ничего не найдено» для всей страницы (нет совпадений ни в одной секции). */
export function EmptySearchPage({ query }: { readonly query: string }) {
  return (
    <div className="rounded-[14px] border border-dashed border-slate-200 bg-white px-6 py-14 text-center">
      <div className="mx-auto mb-4 flex size-14 items-center justify-center rounded-[14px] bg-slate-100/70 text-slate-500">
        <Search className="size-6" />
      </div>
      <div className="mb-1.5 text-base font-semibold text-slate-900">
        Ничего не найдено
      </div>
      <div className="text-[13.5px] text-slate-500">
        По запросу «{query}» нет совпадений. Попробуйте другие слова.
      </div>
    </div>
  );
}
