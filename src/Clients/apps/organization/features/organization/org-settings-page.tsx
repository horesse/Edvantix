"use client";

import { useEffect } from "react";

import { zodResolver } from "@hookform/resolvers/zod";
import { Loader2 } from "lucide-react";
import { useForm } from "react-hook-form";
import { toast } from "sonner";

import useOrganization from "@workspace/api-hooks/company/useOrganization";
import useUpdateOrganization from "@workspace/api-hooks/company/useUpdateOrganization";
import type { OrganizationDetailDto } from "@workspace/types/company";
import {
  LEGAL_FORM_LABELS,
  LegalForm,
  ORGANIZATION_TYPE_LABELS,
  OrganizationType,
} from "@workspace/types/company";
import { Button } from "@workspace/ui/components/button";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@workspace/ui/components/form";
import { Input } from "@workspace/ui/components/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@workspace/ui/components/select";
import { Skeleton } from "@workspace/ui/components/skeleton";
import {
  type UpdateOrganizationInput,
  updateOrganizationSchema,
} from "@workspace/validations/company";

import { PageHeader } from "@/components/layout/page-header";
import { useOrganization as useOrgContext } from "@/components/organization/provider";

const SECTION =
  "grid gap-8 border-t border-border/40 py-6 md:grid-cols-[240px_1fr]";

function SectionMeta({
  title,
  description,
}: Readonly<{
  title: string;
  description: string;
}>) {
  return (
    <div>
      <p className="text-sm font-medium">{title}</p>
      <p className="text-muted-foreground mt-1 text-xs">{description}</p>
    </div>
  );
}

function OrgSettingsSkeleton() {
  return (
    <div className="space-y-0">
      <div className="pb-2">
        <Skeleton className="h-5 w-44" />
      </div>
      {[2, 2].map((count, i) => (
        <div key={i} className={SECTION}>
          <div className="space-y-2">
            <Skeleton className="h-4 w-28" />
            <Skeleton className="h-3 w-44" />
          </div>
          <div className="space-y-3">
            {Array.from({ length: count }).map((_, j) => (
              <div key={j} className="space-y-1.5">
                <Skeleton className="h-3 w-24" />
                <Skeleton className="h-9 w-full" />
              </div>
            ))}
          </div>
        </div>
      ))}
    </div>
  );
}

export function OrgSettingsPage() {
  const { currentOrg, canManage } = useOrgContext();
  const orgId = currentOrg?.id ?? "";
  const { data: org, isLoading } = useOrganization(orgId);

  if (!canManage) {
    return (
      <p className="text-muted-foreground py-8 text-center text-sm">
        Доступ запрещён. Только владелец или менеджер могут изменять настройки
        организации.
      </p>
    );
  }

  if (isLoading) return <OrgSettingsSkeleton />;

  if (!org) {
    return (
      <p className="text-muted-foreground py-8 text-center text-sm">
        Выберите организацию
      </p>
    );
  }

  return (
    <div className="space-y-6">
      <PageHeader
        title="Настройки организации"
        actions={
          <p className="text-muted-foreground text-xs">
            Зарегистрировано:{" "}
            {new Date(org.registrationDate).toLocaleDateString("ru-RU")}
          </p>
        }
      />
      <OrganizationForm org={org} />
    </div>
  );
}

function OrganizationForm({ org }: Readonly<{ org: OrganizationDetailDto }>) {
  const form = useForm<UpdateOrganizationInput>({
    resolver: zodResolver(updateOrganizationSchema),
    defaultValues: {
      fullLegalName: org.fullLegalName,
      shortName: org.shortName ?? "",
      organizationType: org.organizationType,
      legalForm: org.legalForm,
    },
  });

  useEffect(() => {
    form.reset({
      fullLegalName: org.fullLegalName,
      shortName: org.shortName ?? "",
      organizationType: org.organizationType,
      legalForm: org.legalForm,
    });
  }, [
    org.id,
    org.fullLegalName,
    org.shortName,
    org.organizationType,
    org.legalForm,
    form,
  ]);

  const mutation = useUpdateOrganization({
    onSuccess: () => {
      toast.success("Организация обновлена");
      form.reset(form.getValues());
    },
    onError: () => toast.error("Не удалось обновить организацию"),
  });

  function handleSubmit(data: UpdateOrganizationInput) {
    mutation.mutate({
      id: org.id,
      request: {
        fullLegalName: data.fullLegalName,
        shortName: data.shortName || null,
        organizationType: data.organizationType,
        legalForm: data.legalForm,
      },
    });
  }

  const isDirty = form.formState.isDirty;
  const isPending = mutation.isPending;

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(handleSubmit)}>
        {/* ── Наименования ── */}
        <section className={SECTION}>
          <SectionMeta
            title="Наименования"
            description="Полное юридическое и краткое наименование"
          />
          <div className="space-y-3">
            <FormField
              control={form.control}
              name="fullLegalName"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Полное наименование</FormLabel>
                  <FormControl>
                    <Input {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="shortName"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>
                    Краткое название{" "}
                    <span className="text-muted-foreground font-normal">
                      (необязательно)
                    </span>
                  </FormLabel>
                  <FormControl>
                    <Input {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
          </div>
        </section>

        {/* ── Классификация ── */}
        <section className={SECTION}>
          <SectionMeta
            title="Классификация"
            description="Правовая форма и тип деятельности организации"
          />
          <div className="space-y-3">
            <FormField
              control={form.control}
              name="legalForm"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Правовая форма</FormLabel>
                  <Select
                    value={String(field.value)}
                    onValueChange={(v) =>
                      field.onChange(Number(v) as LegalForm)
                    }
                  >
                    <FormControl>
                      <SelectTrigger>
                        <SelectValue placeholder="Выберите форму" />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      {(
                        Object.entries(LEGAL_FORM_LABELS) as [string, string][]
                      ).map(([value, label]) => (
                        <SelectItem key={value} value={value}>
                          {label}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="organizationType"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Тип организации</FormLabel>
                  <Select
                    value={String(field.value)}
                    onValueChange={(v) =>
                      field.onChange(Number(v) as OrganizationType)
                    }
                  >
                    <FormControl>
                      <SelectTrigger>
                        <SelectValue placeholder="Выберите тип" />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      {(
                        Object.entries(ORGANIZATION_TYPE_LABELS) as [
                          string,
                          string,
                        ][]
                      ).map(([value, label]) => (
                        <SelectItem key={value} value={value}>
                          {label}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                  <FormMessage />
                </FormItem>
              )}
            />
          </div>
        </section>

        {/* ── Save bar ── */}
        <div className="border-border/40 flex items-center justify-between border-t pt-4">
          {isDirty && !isPending ? (
            <p className="text-muted-foreground text-xs">
              Есть несохранённые изменения
            </p>
          ) : (
            <span />
          )}
          <Button type="submit" size="sm" disabled={isPending || !isDirty}>
            {isPending && <Loader2 className="size-3.5 animate-spin" />}
            Сохранить изменения
          </Button>
        </div>
      </form>
    </Form>
  );
}
