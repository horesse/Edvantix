"use client";

import { useRouter } from "next/navigation";

import { zodResolver } from "@hookform/resolvers/zod";
import { Loader2 } from "lucide-react";
import { useForm } from "react-hook-form";
import { toast } from "sonner";

import useCreateOrganization from "@workspace/api-hooks/company/useCreateOrganization";
import {
  ContactType,
  LEGAL_FORM_LABELS,
  LegalForm,
  ORGANIZATION_TYPE_LABELS,
  OrganizationType,
} from "@workspace/types/company";
import { Button } from "@workspace/ui/components/button";
import { Checkbox } from "@workspace/ui/components/checkbox";
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
import {
  type CreateOrganizationInput,
  createOrganizationSchema,
} from "@workspace/validations/company";

import { PageHeader } from "@/components/layout/page-header";

const CONTACT_TYPE_LABELS: Record<ContactType, string> = {
  [ContactType.Email]: "Email",
  [ContactType.MobilePhone]: "Мобильный телефон",
  [ContactType.Telegram]: "Telegram",
  [ContactType.WhatsApp]: "WhatsApp",
  [ContactType.Viber]: "Viber",
};

export default function CreateOrganizationPage() {
  const router = useRouter();

  const form = useForm<CreateOrganizationInput>({
    resolver: zodResolver(createOrganizationSchema),
    defaultValues: {
      fullLegalName: "",
      shortName: "",
      isLegalEntity: true,
      registrationDate: "",
      legalForm: LegalForm.Llc,
      organizationType: OrganizationType.PrivateEducationalCenter,
      primaryContactType: ContactType.Email,
      primaryContactValue: "",
      primaryContactDescription: "",
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
      fullLegalName: data.fullLegalName,
      shortName: data.shortName || null,
      isLegalEntity: data.isLegalEntity,
      registrationDate: data.registrationDate,
      legalForm: data.legalForm,
      organizationType: data.organizationType,
      primaryContactType: data.primaryContactType,
      primaryContactValue: data.primaryContactValue,
      primaryContactDescription: data.primaryContactDescription,
    });
  }

  return (
    <div className="mx-auto max-w-2xl space-y-6">
      <PageHeader title="Создание организации" />
      <div className="border-t pt-6">
        <Form {...form}>
          <form
            onSubmit={form.handleSubmit(handleSubmit)}
            className="space-y-5"
          >
            {/* Основные реквизиты */}
            <div className="space-y-4">
              <p className="text-muted-foreground text-xs font-medium tracking-wide uppercase">
                Основные реквизиты
              </p>

              <FormField
                control={form.control}
                name="fullLegalName"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>
                      Полное наименование{" "}
                      <span className="text-destructive">*</span>
                    </FormLabel>
                    <FormControl>
                      <Input
                        placeholder='Частное учреждение образования "Образовательный центр «Знание»"'
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
                    <FormLabel>
                      Краткое название{" "}
                      <span className="text-muted-foreground font-normal">
                        (необязательно)
                      </span>
                    </FormLabel>
                    <FormControl>
                      <Input placeholder="ОЦ Знание" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <div className="grid grid-cols-2 gap-4">
                <FormField
                  control={form.control}
                  name="legalForm"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>
                        Правовая форма{" "}
                        <span className="text-destructive">*</span>
                      </FormLabel>
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
                            Object.entries(LEGAL_FORM_LABELS) as [
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
                  name="registrationDate"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>
                        Дата регистрации{" "}
                        <span className="text-destructive">*</span>
                      </FormLabel>
                      <FormControl>
                        <Input type="date" {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
              </div>

              <FormField
                control={form.control}
                name="isLegalEntity"
                render={({ field }) => (
                  <FormItem className="flex flex-row items-center gap-3 space-y-0">
                    <FormControl>
                      <Checkbox
                        checked={field.value}
                        onCheckedChange={field.onChange}
                      />
                    </FormControl>
                    <FormLabel className="cursor-pointer font-normal">
                      Юридическое лицо
                    </FormLabel>
                  </FormItem>
                )}
              />
            </div>

            {/* Тип организации */}
            <div className="space-y-4 border-t pt-4">
              <p className="text-muted-foreground text-xs font-medium tracking-wide uppercase">
                Тип организации
              </p>

              <FormField
                control={form.control}
                name="organizationType"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>
                      Тип <span className="text-destructive">*</span>
                    </FormLabel>
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

            {/* Основной контакт */}
            <div className="space-y-4 border-t pt-4">
              <p className="text-muted-foreground text-xs font-medium tracking-wide uppercase">
                Основной контакт
              </p>

              <div className="grid grid-cols-2 gap-4">
                <FormField
                  control={form.control}
                  name="primaryContactType"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>
                        Тип <span className="text-destructive">*</span>
                      </FormLabel>
                      <Select
                        value={String(field.value)}
                        onValueChange={(v) =>
                          field.onChange(Number(v) as ContactType)
                        }
                      >
                        <FormControl>
                          <SelectTrigger>
                            <SelectValue placeholder="Выберите тип" />
                          </SelectTrigger>
                        </FormControl>
                        <SelectContent>
                          {(
                            Object.entries(CONTACT_TYPE_LABELS) as [
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
                  name="primaryContactValue"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>
                        Значение <span className="text-destructive">*</span>
                      </FormLabel>
                      <FormControl>
                        <Input placeholder="info@example.by" {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
              </div>

              <FormField
                control={form.control}
                name="primaryContactDescription"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>
                      Описание контакта{" "}
                      <span className="text-destructive">*</span>
                    </FormLabel>
                    <FormControl>
                      <Input
                        placeholder="Основная почта для связи"
                        {...field}
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>

            <div className="flex justify-end gap-2 pt-2">
              <Button
                type="button"
                variant="outline"
                size="sm"
                onClick={() => router.back()}
              >
                Отмена
              </Button>
              <Button
                type="submit"
                size="sm"
                disabled={createMutation.isPending}
              >
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
