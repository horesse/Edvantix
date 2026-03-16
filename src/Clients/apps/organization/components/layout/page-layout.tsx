import { cn } from "@workspace/ui/lib/utils";

import { PageHeader } from "./page-header";

type BackProps =
  | { href: string; onClick?: never }
  | { href?: never; onClick: () => void };

interface PageLayoutProps {
  /**
   * Page title — renders a standard `PageHeader` when provided.
   * Omit when you need a fully custom header via the `header` prop.
   */
  title?: string;
  /** Short description shown below the title. */
  description?: string;
  /** Slot for action buttons (top-right of the header). */
  actions?: React.ReactNode;
  /** Back navigation rendered above the title. */
  back?: BackProps & { label: string };
  /**
   * Fully custom header element.
   * Takes precedence over `title` / `description` / `actions` / `back`.
   */
  header?: React.ReactNode;
  /** Main page content. */
  children: React.ReactNode;
  className?: string;
}

/**
 * Standard page wrapper.
 *
 * Usage — shorthand with built-in header:
 * ```tsx
 * <PageLayout title="Участники" actions={<Button>Пригласить</Button>}>
 *   <MembersTable />
 * </PageLayout>
 * ```
 *
 * Usage — custom header:
 * ```tsx
 * <PageLayout header={<StickyBreadcrumb />}>
 *   <ProfileSettingsForm />
 * </PageLayout>
 * ```
 */
export function PageLayout({
  title,
  description,
  actions,
  back,
  header,
  children,
  className,
}: Readonly<PageLayoutProps>) {
  const resolvedHeader =
    header ??
    (title ? (
      <PageHeader
        title={title}
        description={description}
        actions={actions}
        back={back}
      />
    ) : null);

  return (
    <div className={cn("space-y-5", className)}>
      {resolvedHeader}
      {children}
    </div>
  );
}
