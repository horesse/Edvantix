"use client";

import { useRouter } from "next/navigation";

import { ArrowLeft } from "lucide-react";

import { Button } from "@workspace/ui/components/button";

type BackProps =
  | { href: string; onClick?: never }
  | { href?: never; onClick: () => void };

type PageHeaderProps = Readonly<{
  title: string;
  description?: string;
  actions?: React.ReactNode;
  back?: BackProps & { label: string };
}>;

export function PageHeader({
  title,
  description,
  actions,
  back,
}: PageHeaderProps) {
  const router = useRouter();

  function handleBack() {
    if (back?.href) {
      router.push(back.href);
    } else {
      back?.onClick?.();
    }
  }

  return (
    <div className="space-y-2">
      {back && (
        <Button
          variant="ghost"
          size="sm"
          className="text-muted-foreground -ml-2 h-7 gap-1.5 px-2 text-xs"
          onClick={handleBack}
        >
          <ArrowLeft className="size-3.5" />
          {back.label}
        </Button>
      )}
      <div
        className={`flex justify-between gap-4 ${description ? "items-start" : "items-center"}`}
      >
        <div className="min-w-0 space-y-0.5">
          <h1 className="text-lg font-semibold">{title}</h1>
          {description && (
            <p className="text-muted-foreground text-sm">{description}</p>
          )}
        </div>
        {actions && (
          <div className="flex shrink-0 items-center gap-1.5">{actions}</div>
        )}
      </div>
    </div>
  );
}
