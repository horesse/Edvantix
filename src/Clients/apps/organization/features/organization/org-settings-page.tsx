"use client";

import { zodResolver } from "@hookform/resolvers/zod";
import { Loader2 } from "lucide-react";
import { useForm } from "react-hook-form";
import { toast } from "sonner";

import useOrganization from "@workspace/api-hooks/company/useOrganization";
import useUpdateOrganization from "@workspace/api-hooks/company/useUpdateOrganization";
import { Button } from "@workspace/ui/components/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@workspace/ui/components/card";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@workspace/ui/components/form";
import { Input } from "@workspace/ui/components/input";
import { Skeleton } from "@workspace/ui/components/skeleton";
import { Textarea } from "@workspace/ui/components/textarea";
import {
  type UpdateOrganizationInput,
  updateOrganizationSchema,
} from "@workspace/validations/company";

import { useOrganization as useOrgContext } from "@/components/organization-provider";

export function OrgSettingsPage() {
  const { currentOrg, canManage } = useOrgContext();
  const orgId = currentOrg?.id ?? 0;
  const { data: org, isLoading } = useOrganization(orgId);

  if (!canManage) {
    return (
      <p className="text-muted-foreground py-8 text-center">
        Доступ запрещён. Только владелец или менеджер могут изменять настройки
        организации.
      </p>
    );
  }

  if (isLoading) {
    return (
      <div className="mx-auto max-w-2xl space-y-4">
        <Skeleton className="h-8 w-48" />
        <Skeleton className="h-96 w-full" />
      </div>
    );
  }

  if (!org) {
    return (
      <p className="text-muted-foreground py-8 text-center">
        Выберите организацию
      </p>
    );
  }

  return (
    <div className="mx-auto max-w-2xl space-y-4">
      <div>
        <h1 className="text-2xl font-bold">Настройки организации</h1>
        <p className="text-muted-foreground text-sm">
          Основная информация о вашей организации
        </p>
      </div>

      <Card>
        <CardHeader>
          <CardTitle className="text-base">Информация</CardTitle>
          <CardDescription>
            Дата регистрации:{" "}
            {new Date(org.registrationDate).toLocaleDateString("ru-RU")}
          </CardDescription>
        </CardHeader>
        <CardContent>
          <OrganizationForm org={org} />
        </CardContent>
      </Card>
    </div>
  );
}

function OrganizationForm({
  org,
}: {
  org: {
    id: number;
    name: string;
    nameLatin: string;
    shortName: string;
    printName?: string | null;
    description?: string | null;
  };
}) {
  const form = useForm<UpdateOrganizationInput>({
    resolver: zodResolver(updateOrganizationSchema),
    defaultValues: {
      name: org.name,
      nameLatin: org.nameLatin,
      shortName: org.shortName,
      printName: org.printName ?? "",
      description: org.description ?? "",
    },
  });

  const mutation = useUpdateOrganization({
    onSuccess: () => toast.success("Организация обновлена"),
    onError: () => toast.error("Не удалось обновить организацию"),
  });

  function handleSubmit(data: UpdateOrganizationInput) {
    mutation.mutate({
      id: org.id,
      request: {
        name: data.name,
        nameLatin: data.nameLatin,
        shortName: data.shortName,
        printName: data.printName || null,
        description: data.description || null,
      },
    });
  }

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(handleSubmit)} className="space-y-4">
        <FormField
          control={form.control}
          name="name"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Название</FormLabel>
              <FormControl>
                <Input {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          control={form.control}
          name="nameLatin"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Название (латиница)</FormLabel>
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
              <FormLabel>Краткое название</FormLabel>
              <FormControl>
                <Input {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          control={form.control}
          name="printName"
          render={({ field }) => (
            <FormItem>
              <FormLabel>
                Печатное название{" "}
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
        <FormField
          control={form.control}
          name="description"
          render={({ field }) => (
            <FormItem>
              <FormLabel>
                Описание{" "}
                <span className="text-muted-foreground font-normal">
                  (необязательно)
                </span>
              </FormLabel>
              <FormControl>
                <Textarea rows={3} {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <div className="flex justify-end">
          <Button type="submit" disabled={mutation.isPending}>
            {mutation.isPending && <Loader2 className="size-4 animate-spin" />}
            Сохранить
          </Button>
        </div>
      </form>
    </Form>
  );
}
