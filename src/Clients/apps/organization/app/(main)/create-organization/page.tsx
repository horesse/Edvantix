"use client";

import { useRouter } from "next/navigation";

import { zodResolver } from "@hookform/resolvers/zod";
import { Loader2 } from "lucide-react";
import { useForm } from "react-hook-form";
import { toast } from "sonner";

import useCreateOrganization from "@workspace/api-hooks/company/useCreateOrganization";
import useLegalForms from "@workspace/api-hooks/company/useLegalForms";
import {
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
import { Textarea } from "@workspace/ui/components/textarea";
import {
  type CreateOrganizationInput,
  createOrganizationSchema,
} from "@workspace/validations/company";

import { PageHeader } from "@/components/page-header";

export default function CreateOrganizationPage() {
  const router = useRouter();

  const { data: legalForms = [], isLoading: isLegalFormsLoading } =
    useLegalForms();

  const form = useForm<CreateOrganizationInput>({
    resolver: zodResolver(createOrganizationSchema),
    defaultValues: {
      name: "",
      nameLatin: "",
      shortName: "",
      legalFormId: "",
      printName: "",
      description: "",
    },
  });

  const createMutation = useCreateOrganization({
    onSuccess: () => {
      toast.success("Организация создана");
      router.push("/");
    },
    onError: () => {
      toast.error("Не удалось создать организацию");
    },
  });

  function handleSubmit(data: CreateOrganizationInput) {
    createMutation.mutate({
      name: data.name,
      nameLatin: data.nameLatin,
      shortName: data.shortName,
      organizationType: data.organizationType,
      legalFormId: data.legalFormId,
      printName: data.printName || null,
      description: data.description || null,
    });
  }

  return (
    <div className="mx-auto max-w-2xl space-y-6">
      <PageHeader title="Создание организации" />
      <div className="border-t pt-6">
        <Form {...form}>
          <form
            onSubmit={form.handleSubmit(handleSubmit)}
            className="space-y-4"
          >
            <FormField
              control={form.control}
              name="name"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Название</FormLabel>
                  <FormControl>
                    <Input
                      placeholder="Образовательный центр «Знание»"
                      {...field}
                    />
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
                    <Input
                      placeholder="Knowledge Education Center"
                      {...field}
                    />
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
                    <Input placeholder="ОЦ Знание" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            {/* Организационно-правовая форма */}
            <FormField
              control={form.control}
              name="legalFormId"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Правовая форма</FormLabel>
                  <Select
                    value={field.value}
                    onValueChange={field.onChange}
                    disabled={isLegalFormsLoading}
                  >
                    <FormControl>
                      <SelectTrigger>
                        <SelectValue placeholder="Выберите форму" />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      {legalForms.map((lf) => (
                        <SelectItem key={lf.id} value={lf.id}>
                          <span className="font-medium">{lf.shortName}</span>
                          <span className="text-muted-foreground">
                            {" "}
                            — {lf.name}
                          </span>
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                  <FormMessage />
                </FormItem>
              )}
            />

            {/* Тип организации */}
            <FormField
              control={form.control}
              name="organizationType"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Тип организации</FormLabel>
                  <Select
                    value={
                      field.value !== undefined ? String(field.value) : ""
                    }
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

            <FormField
              control={form.control}
              name="printName"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>
                    Печатное название{" "}
                    <span className="font-normal opacity-60">
                      (необязательно)
                    </span>
                  </FormLabel>
                  <FormControl>
                    <Input placeholder="ОЦ «Знание»" {...field} />
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
                    <span className="font-normal opacity-60">
                      (необязательно)
                    </span>
                  </FormLabel>
                  <FormControl>
                    <Textarea
                      placeholder="Краткое описание организации"
                      rows={3}
                      {...field}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <div className="flex justify-end gap-2 pt-2">
              <Button
                type="button"
                variant="outline"
                size="sm"
                onClick={() => router.back()}
              >
                Отмена
              </Button>
              <Button type="submit" size="sm" disabled={createMutation.isPending}>
                {createMutation.isPending && (
                  <Loader2 className="size-4 animate-spin" />
                )}
                Создать
              </Button>
            </div>
          </form>
        </Form>
      </div>
    </div>
  );
}
