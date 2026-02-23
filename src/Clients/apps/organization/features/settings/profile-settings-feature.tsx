"use client";

import { useEffect, useRef, useState } from "react";

import { zodResolver } from "@hookform/resolvers/zod";
import {
  Briefcase,
  Camera,
  Globe,
  GraduationCap,
  Loader2,
  Mail,
  MoreHorizontal,
  Phone,
  Plus,
  Trash2,
  UserCircle,
} from "lucide-react";
import { useFieldArray, useForm } from "react-hook-form";
import { toast } from "sonner";
import { z } from "zod";

import useProfileDetails from "@workspace/api-hooks/profiles/useProfileDetails";
import useUpdateProfile from "@workspace/api-hooks/profiles/useUpdateProfile";
import type { OwnProfileDetails, UpdateProfileRequest } from "@workspace/types/profile";
import { ContactType, EducationLevel } from "@workspace/types/profile";
import {
  Avatar,
  AvatarFallback,
  AvatarImage,
} from "@workspace/ui/components/avatar";
import { Badge } from "@workspace/ui/components/badge";
import { Button } from "@workspace/ui/components/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@workspace/ui/components/dialog";
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
import { Textarea } from "@workspace/ui/components/textarea";
import { formatDateRange, getInitials } from "@workspace/utils/format";
import {
  ALLOWED_IMAGE_TYPES,
  MAX_AVATAR_SIZE,
  type ContactInput,
  type EducationInput,
  type EmploymentInput,
  contactSchema,
  educationSchema,
  employmentSchema,
  profileSettingsSchema,
} from "@workspace/validations/profile";

// ─── Schema ───────────────────────────────────────────────────────────────────

const profileFormSchema = profileSettingsSchema.extend({
  contacts: z.array(contactSchema).default([]),
  employmentHistories: z.array(employmentSchema).default([]),
  educations: z.array(educationSchema).default([]),
});

type ProfileFormValues = z.infer<typeof profileFormSchema>;

// ─── Constants ────────────────────────────────────────────────────────────────

const contactTypeLabels: Record<ContactType, string> = {
  [ContactType.Email]: "Email",
  [ContactType.Phone]: "Телефон",
  [ContactType.Uri]: "Веб-сайт",
  [ContactType.Other]: "Другое",
};

const contactTypeIcons: Record<ContactType, React.ReactNode> = {
  [ContactType.Email]: <Mail className="size-3.5" />,
  [ContactType.Phone]: <Phone className="size-3.5" />,
  [ContactType.Uri]: <Globe className="size-3.5" />,
  [ContactType.Other]: <MoreHorizontal className="size-3.5" />,
};

const educationLevelLabels: Record<EducationLevel, string> = {
  [EducationLevel.Preschool]: "Дошкольное",
  [EducationLevel.GeneralSecondary]: "Общее среднее",
  [EducationLevel.VocationalTechnical]: "Профессионально-техническое",
  [EducationLevel.SecondarySpecialized]: "Среднее специальное",
  [EducationLevel.HigherBachelor]: "Высшее (I ступень)",
  [EducationLevel.HigherMaster]: "Высшее (II ступень)",
  [EducationLevel.Postgraduate]: "Послевузовское",
  [EducationLevel.AdditionalChildren]: "Доп. образование детей",
  [EducationLevel.AdditionalAdults]: "Доп. образование взрослых",
  [EducationLevel.Special]: "Специальное",
};

// ─── Helpers ──────────────────────────────────────────────────────────────────

/** Extracts YYYY-MM-DD from either a date string or ISO datetime string. */
function toDateString(value: string | null | undefined): string {
  if (!value) return "";
  return value.slice(0, 10);
}

function getDefaultValues(profile: OwnProfileDetails): ProfileFormValues {
  return {
    lastName: profile.lastName,
    firstName: profile.firstName,
    middleName: profile.middleName ?? "",
    birthDate: profile.birthDate,
    contacts: profile.contacts.map((c) => ({
      type: c.type,
      value: c.value,
      description: c.description ?? "",
    })),
    employmentHistories: profile.employmentHistories.map((e) => ({
      workplace: e.workplace,
      position: e.position,
      startDate: toDateString(e.startDate),
      endDate: toDateString(e.endDate),
      description: e.description ?? "",
    })),
    educations: profile.educations.map((e) => ({
      institution: e.institution,
      specialty: e.specialty ?? "",
      dateStart: toDateString(e.dateStart),
      dateEnd: toDateString(e.dateEnd),
      level: e.educationLevel,
    })),
  };
}

function buildUpdateRequest(
  values: ProfileFormValues,
  avatar?: File,
): UpdateProfileRequest {
  return {
    firstName: values.firstName,
    lastName: values.lastName,
    middleName: values.middleName || null,
    birthDate: values.birthDate,
    contacts: values.contacts.map((c) => ({
      type: c.type,
      value: c.value,
      description: c.description || null,
    })),
    employmentHistories: values.employmentHistories.map((e) => ({
      workplace: e.workplace,
      position: e.position,
      startDate: e.startDate,
      endDate: e.endDate || null,
      description: e.description || null,
    })),
    educations: values.educations.map((e) => ({
      institution: e.institution,
      specialty: e.specialty || null,
      dateStart: e.dateStart,
      dateEnd: e.dateEnd || null,
      level: e.level,
    })),
    avatar,
  };
}

// ─── Skeleton ─────────────────────────────────────────────────────────────────

function ProfileSettingsSkeleton() {
  return (
    <div className="divide-y divide-border/40">
      <div className="pb-6">
        <div className="flex items-center gap-4">
          <Skeleton className="size-14 shrink-0 rounded-full" />
          <div className="space-y-2">
            <Skeleton className="h-4 w-36" />
            <Skeleton className="h-3 w-24" />
          </div>
        </div>
      </div>
      <div className="space-y-3 py-6">
        <Skeleton className="h-3 w-28" />
        <div className="grid gap-3 sm:grid-cols-2">
          {Array.from({ length: 4 }).map((_, i) => (
            <Skeleton key={i} className="h-9 w-full rounded-md" />
          ))}
        </div>
      </div>
      {[28, 24, 24].map((w, i) => (
        <div key={i} className="space-y-3 py-6">
          <div className="flex items-center justify-between">
            <Skeleton className={`h-3 w-${w}`} />
            <Skeleton className="h-7 w-24 rounded-md" />
          </div>
          <Skeleton className="h-12 w-full rounded-lg" />
        </div>
      ))}
    </div>
  );
}

// ─── Shared primitives ────────────────────────────────────────────────────────

function SectionLabel({ children }: { children: React.ReactNode }) {
  return (
    <p className="mb-4 text-[11px] font-semibold uppercase tracking-[0.08em] text-muted-foreground">
      {children}
    </p>
  );
}

function SectionHeader({
  label,
  action,
}: {
  label: string;
  action?: React.ReactNode;
}) {
  return (
    <div className="mb-4 flex items-center justify-between">
      <SectionLabel>{label}</SectionLabel>
      {action}
    </div>
  );
}

function EmptyState({
  icon,
  text,
  onAdd,
}: {
  icon: React.ReactNode;
  text: string;
  onAdd: () => void;
}) {
  return (
    <button
      type="button"
      onClick={onAdd}
      className="flex w-full flex-col items-center justify-center gap-2 rounded-lg border border-dashed border-border/50 py-7 transition-all hover:border-border hover:bg-muted/20"
    >
      <span className="text-muted-foreground/30">{icon}</span>
      <span className="text-xs text-muted-foreground">{text}</span>
    </button>
  );
}

// ─── Avatar block (immediate upload — lives outside the main form) ─────────────

function AvatarBlock({
  profile,
  getFormValues,
}: {
  profile: OwnProfileDetails;
  getFormValues: () => ProfileFormValues;
}) {
  const fileInputRef = useRef<HTMLInputElement>(null);
  const [preview, setPreview] = useState<string | null>(null);

  const uploadMutation = useUpdateProfile({
    onSuccess: () => {
      toast.success("Аватар обновлён");
      if (preview) {
        URL.revokeObjectURL(preview);
        setPreview(null);
      }
    },
    onError: () => {
      toast.error("Не удалось загрузить аватар");
      if (preview) {
        URL.revokeObjectURL(preview);
        setPreview(null);
      }
    },
  });

  function handleFileChange(file: File | null) {
    if (!file) return;

    if (file.size > MAX_AVATAR_SIZE) {
      toast.error("Размер файла не должен превышать 5 МБ");
      return;
    }

    if (!ALLOWED_IMAGE_TYPES.includes(file.type)) {
      toast.error("Допустимые форматы: JPEG, PNG, GIF, WebP");
      return;
    }

    if (preview) URL.revokeObjectURL(preview);
    setPreview(URL.createObjectURL(file));

    // Use current form values so unsaved changes are not overwritten by avatar upload.
    uploadMutation.mutate(buildUpdateRequest(getFormValues(), file));
  }

  const displayUrl = preview ?? profile.avatarUrl;
  const fullName = [profile.lastName, profile.firstName, profile.middleName]
    .filter(Boolean)
    .join(" ");

  return (
    <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:gap-5">
      <div className="relative shrink-0">
        <Avatar className="size-14 ring-2 ring-background">
          <AvatarImage src={displayUrl ?? undefined} alt={profile.firstName} />
          <AvatarFallback className="bg-muted text-base font-medium text-foreground">
            {fullName ? (
              getInitials(fullName)
            ) : (
              <UserCircle className="size-7" />
            )}
          </AvatarFallback>
        </Avatar>

        <button
          type="button"
          onClick={() => fileInputRef.current?.click()}
          disabled={uploadMutation.isPending}
          className="absolute -bottom-1 -right-1 flex size-6 items-center justify-center rounded-full border border-border/60 bg-background shadow-sm transition-all hover:scale-110 active:scale-95 disabled:opacity-50"
          aria-label="Изменить фото"
        >
          {uploadMutation.isPending ? (
            <Loader2 className="size-3 animate-spin text-muted-foreground" />
          ) : (
            <Camera className="size-3" />
          )}
        </button>
      </div>

      <div className="min-w-0">
        <p className="truncate text-sm font-medium">{fullName || "—"}</p>
        <p className="text-xs text-muted-foreground">{profile.login}</p>
        <button
          type="button"
          onClick={() => fileInputRef.current?.click()}
          disabled={uploadMutation.isPending}
          className="mt-1.5 text-xs text-muted-foreground transition-colors hover:text-foreground disabled:opacity-50"
        >
          {uploadMutation.isPending ? "Загружаю…" : "Изменить фото"}
        </button>
      </div>

      <Input
        ref={fileInputRef}
        type="file"
        accept="image/jpeg,image/png,image/gif,image/webp"
        className="hidden"
        onChange={(e) => {
          handleFileChange(e.target.files?.[0] ?? null);
          if (fileInputRef.current) fileInputRef.current.value = "";
        }}
      />
    </div>
  );
}

// ─── List item components ─────────────────────────────────────────────────────

function ContactRow({
  field,
  onRemove,
}: {
  field: ContactInput & { id: string };
  onRemove: () => void;
}) {
  return (
    <div className="group flex items-center gap-3 rounded-md px-2 py-2.5 transition-colors hover:bg-muted/30">
      <span className="shrink-0 text-muted-foreground">
        {contactTypeIcons[field.type]}
      </span>
      <div className="min-w-0 flex-1">
        <span className="truncate text-sm">{field.value}</span>
        {field.description && (
          <p className="truncate text-xs text-muted-foreground/60">
            {field.description}
          </p>
        )}
      </div>
      <Badge
        variant="secondary"
        className="shrink-0 rounded-sm px-1.5 py-0 text-[10px] font-normal"
      >
        {contactTypeLabels[field.type]}
      </Badge>
      <button
        type="button"
        onClick={onRemove}
        className="ml-0.5 shrink-0 text-transparent transition-colors group-hover:text-muted-foreground/40 hover:!text-destructive"
        aria-label="Удалить контакт"
      >
        <Trash2 className="size-3.5" />
      </button>
    </div>
  );
}

function EmploymentCard({
  field,
  onRemove,
}: {
  field: EmploymentInput & { id: string };
  onRemove: () => void;
}) {
  return (
    <div className="group relative rounded-lg border border-border/40 p-3.5 transition-colors hover:bg-muted/20">
      <div className="flex items-start justify-between gap-3">
        <div className="min-w-0">
          <p className="text-sm font-medium">{field.position}</p>
          <p className="mt-0.5 truncate text-xs text-muted-foreground">
            {field.workplace}
            {field.startDate && (
              <>
                {" · "}
                {formatDateRange(field.startDate, field.endDate || null)}
              </>
            )}
          </p>
          {field.description && (
            <p className="mt-2 text-xs leading-relaxed text-muted-foreground">
              {field.description}
            </p>
          )}
        </div>
        <button
          type="button"
          onClick={onRemove}
          className="shrink-0 text-transparent transition-colors group-hover:text-muted-foreground/40 hover:!text-destructive"
          aria-label="Удалить место работы"
        >
          <Trash2 className="size-3.5" />
        </button>
      </div>
    </div>
  );
}

function EducationCard({
  field,
  onRemove,
}: {
  field: EducationInput & { id: string };
  onRemove: () => void;
}) {
  const levelLabel = field.level
    ? (educationLevelLabels[field.level as EducationLevel] ?? "Не указано")
    : "Не указано";

  return (
    <div className="group relative rounded-lg border border-border/40 p-3.5 transition-colors hover:bg-muted/20">
      <div className="flex items-start justify-between gap-3">
        <div className="min-w-0">
          <p className="text-sm font-medium">{field.institution}</p>
          {field.specialty && (
            <p className="mt-0.5 text-xs text-muted-foreground">
              {field.specialty}
            </p>
          )}
          <div className="mt-1.5 flex flex-wrap items-center gap-2">
            <Badge
              variant="secondary"
              className="rounded-sm px-1.5 py-0 text-[10px] font-normal"
            >
              {levelLabel}
            </Badge>
            {field.dateStart && (
              <span className="text-xs text-muted-foreground/60">
                {formatDateRange(field.dateStart, field.dateEnd || null)}
              </span>
            )}
          </div>
        </div>
        <button
          type="button"
          onClick={onRemove}
          className="shrink-0 text-transparent transition-colors group-hover:text-muted-foreground/40 hover:!text-destructive"
          aria-label="Удалить образование"
        >
          <Trash2 className="size-3.5" />
        </button>
      </div>
    </div>
  );
}

// ─── Add dialogs (no API call — just append to form state) ────────────────────

function ContactDialog({
  open,
  onOpenChange,
  onAppend,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onAppend: (data: ContactInput) => void;
}) {
  const form = useForm<ContactInput>({
    resolver: zodResolver(contactSchema),
    defaultValues: { type: ContactType.Email, value: "", description: "" },
  });

  function handleSubmit(data: ContactInput) {
    onAppend(data);
    form.reset();
    onOpenChange(false);
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>Новый контакт</DialogTitle>
          <DialogDescription>Добавьте способ связи с вами</DialogDescription>
        </DialogHeader>
        <Form {...form}>
          <form
            onSubmit={form.handleSubmit(handleSubmit)}
            className="space-y-4"
          >
            <FormField
              control={form.control}
              name="type"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Тип</FormLabel>
                  <Select
                    onValueChange={(v) => field.onChange(Number(v))}
                    value={field.value?.toString()}
                  >
                    <FormControl>
                      <SelectTrigger className="w-full">
                        <SelectValue placeholder="Выберите тип" />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      {Object.entries(contactTypeLabels).map(([key, label]) => (
                        <SelectItem key={key} value={key}>
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
              name="value"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Значение</FormLabel>
                  <FormControl>
                    <Input placeholder="example@mail.com" {...field} />
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
                    <span className="font-normal text-muted-foreground">
                      (необязательно)
                    </span>
                  </FormLabel>
                  <FormControl>
                    <Input placeholder="Рабочий email" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <DialogFooter>
              <Button
                type="button"
                variant="outline"
                onClick={() => onOpenChange(false)}
              >
                Отмена
              </Button>
              <Button type="submit">Добавить</Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}

function EmploymentDialog({
  open,
  onOpenChange,
  onAppend,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onAppend: (data: EmploymentInput) => void;
}) {
  const form = useForm<EmploymentInput>({
    resolver: zodResolver(employmentSchema),
    defaultValues: {
      workplace: "",
      position: "",
      startDate: "",
      endDate: "",
      description: "",
    },
  });

  function handleSubmit(data: EmploymentInput) {
    onAppend(data);
    form.reset();
    onOpenChange(false);
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-h-[90vh] overflow-y-auto sm:max-w-lg">
        <DialogHeader>
          <DialogTitle>Новое место работы</DialogTitle>
          <DialogDescription>
            Укажите информацию о вашем опыте работы
          </DialogDescription>
        </DialogHeader>
        <Form {...form}>
          <form
            onSubmit={form.handleSubmit(handleSubmit)}
            className="space-y-4"
          >
            <FormField
              control={form.control}
              name="workplace"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Организация</FormLabel>
                  <FormControl>
                    <Input placeholder="ООО «Компания»" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="position"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Должность</FormLabel>
                  <FormControl>
                    <Input placeholder="Преподаватель" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <div className="grid gap-4 sm:grid-cols-2">
              <FormField
                control={form.control}
                name="startDate"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Дата начала</FormLabel>
                    <FormControl>
                      <Input type="date" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="endDate"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>
                      Дата окончания{" "}
                      <span className="font-normal text-muted-foreground">
                        (пусто = по н.в.)
                      </span>
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
              name="description"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>
                    Описание{" "}
                    <span className="font-normal text-muted-foreground">
                      (необязательно)
                    </span>
                  </FormLabel>
                  <FormControl>
                    <Textarea
                      placeholder="Опишите ваши обязанности и достижения…"
                      rows={3}
                      {...field}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <DialogFooter>
              <Button
                type="button"
                variant="outline"
                onClick={() => onOpenChange(false)}
              >
                Отмена
              </Button>
              <Button type="submit">Добавить</Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}

function EducationDialog({
  open,
  onOpenChange,
  onAppend,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onAppend: (data: EducationInput) => void;
}) {
  const form = useForm<EducationInput>({
    resolver: zodResolver(educationSchema),
    defaultValues: {
      institution: "",
      specialty: "",
      dateStart: "",
      dateEnd: "",
      level: undefined,
    },
  });

  function handleSubmit(data: EducationInput) {
    onAppend(data);
    form.reset();
    onOpenChange(false);
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-h-[90vh] overflow-y-auto sm:max-w-lg">
        <DialogHeader>
          <DialogTitle>Новое образование</DialogTitle>
          <DialogDescription>
            Укажите информацию о вашем образовании
          </DialogDescription>
        </DialogHeader>
        <Form {...form}>
          <form
            onSubmit={form.handleSubmit(handleSubmit)}
            className="space-y-4"
          >
            <FormField
              control={form.control}
              name="institution"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Учебное заведение</FormLabel>
                  <FormControl>
                    <Input
                      placeholder="Название учебного заведения"
                      {...field}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="specialty"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>
                    Специальность{" "}
                    <span className="font-normal text-muted-foreground">
                      (необязательно)
                    </span>
                  </FormLabel>
                  <FormControl>
                    <Input placeholder="Информатика" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="level"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Уровень образования</FormLabel>
                  <Select
                    onValueChange={(v) => field.onChange(Number(v))}
                    value={field.value?.toString()}
                  >
                    <FormControl>
                      <SelectTrigger className="w-full">
                        <SelectValue placeholder="Выберите уровень" />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      {Object.entries(educationLevelLabels).map(
                        ([key, label]) => (
                          <SelectItem key={key} value={key}>
                            {label}
                          </SelectItem>
                        ),
                      )}
                    </SelectContent>
                  </Select>
                  <FormMessage />
                </FormItem>
              )}
            />
            <div className="grid gap-4 sm:grid-cols-2">
              <FormField
                control={form.control}
                name="dateStart"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Дата начала</FormLabel>
                    <FormControl>
                      <Input type="date" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="dateEnd"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>
                      Дата окончания{" "}
                      <span className="font-normal text-muted-foreground">
                        (пусто = учусь)
                      </span>
                    </FormLabel>
                    <FormControl>
                      <Input type="date" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>
            <DialogFooter>
              <Button
                type="button"
                variant="outline"
                onClick={() => onOpenChange(false)}
              >
                Отмена
              </Button>
              <Button type="submit">Добавить</Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}

// ─── Main form ────────────────────────────────────────────────────────────────

function ProfileEditor({ profile }: { profile: OwnProfileDetails }) {
  const [contactDialog, setContactDialog] = useState(false);
  const [employmentDialog, setEmploymentDialog] = useState(false);
  const [educationDialog, setEducationDialog] = useState(false);

  const updateMutation = useUpdateProfile({
    onSuccess: () => {
      toast.success("Профиль сохранён");
      // Reset dirty state without changing values — server has accepted the data.
      form.reset(form.getValues());
    },
    onError: () => {
      toast.error("Не удалось сохранить профиль");
    },
  });

  const form = useForm<ProfileFormValues>({
    resolver: zodResolver(profileFormSchema),
    defaultValues: getDefaultValues(profile),
  });

  // Sync form with server state, but only when the form is clean to avoid wiping unsaved changes.
  useEffect(() => {
    if (!form.formState.isDirty) {
      form.reset(getDefaultValues(profile));
    }
  }, [profile]); // eslint-disable-line react-hooks/exhaustive-deps

  const contactsArray = useFieldArray({
    control: form.control,
    name: "contacts",
  });

  const employmentsArray = useFieldArray({
    control: form.control,
    name: "employmentHistories",
  });

  const educationsArray = useFieldArray({
    control: form.control,
    name: "educations",
  });

  function onSubmit(data: ProfileFormValues) {
    updateMutation.mutate(buildUpdateRequest(data));
  }

  const isDirty = form.formState.isDirty;
  const isPending = updateMutation.isPending;

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)}>
        {/* Avatar — lives outside the form fields, uploads immediately on file select. */}
        <div className="pb-6">
          <AvatarBlock profile={profile} getFormValues={form.getValues} />
        </div>

        <div className="divide-y divide-border/40">
          {/* ── Personal info ── */}
          <section className="py-6">
            <SectionLabel>Личная информация</SectionLabel>
            <div className="space-y-3">
              <div className="grid gap-3 sm:grid-cols-2">
                <FormField
                  control={form.control}
                  name="lastName"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel className="text-xs text-muted-foreground">
                        Фамилия
                      </FormLabel>
                      <FormControl>
                        <Input className="h-9" placeholder="Иванов" {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
                <FormField
                  control={form.control}
                  name="firstName"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel className="text-xs text-muted-foreground">
                        Имя
                      </FormLabel>
                      <FormControl>
                        <Input className="h-9" placeholder="Иван" {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
              </div>
              <div className="grid gap-3 sm:grid-cols-2">
                <FormField
                  control={form.control}
                  name="middleName"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel className="text-xs text-muted-foreground">
                        Отчество{" "}
                        <span className="text-muted-foreground/50">
                          (необяз.)
                        </span>
                      </FormLabel>
                      <FormControl>
                        <Input
                          className="h-9"
                          placeholder="Иванович"
                          {...field}
                        />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
                <FormField
                  control={form.control}
                  name="birthDate"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel className="text-xs text-muted-foreground">
                        Дата рождения
                      </FormLabel>
                      <FormControl>
                        <Input className="h-9" type="date" {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
              </div>
            </div>
          </section>

          {/* ── Contacts ── */}
          <section className="py-6">
            <SectionHeader
              label="Контакты"
              action={
                <Button
                  type="button"
                  variant="ghost"
                  size="sm"
                  onClick={() => setContactDialog(true)}
                  className="h-7 gap-1.5 px-2.5 text-xs text-muted-foreground hover:text-foreground"
                >
                  <Plus className="size-3" />
                  Добавить
                </Button>
              }
            />
            {contactsArray.fields.length === 0 ? (
              <EmptyState
                icon={<Mail className="size-5" />}
                text="Способы связи не добавлены"
                onAdd={() => setContactDialog(true)}
              />
            ) : (
              <div className="space-y-0.5">
                {contactsArray.fields.map((field, index) => (
                  <ContactRow
                    key={field.id}
                    field={field}
                    onRemove={() => contactsArray.remove(index)}
                  />
                ))}
              </div>
            )}
            <ContactDialog
              open={contactDialog}
              onOpenChange={setContactDialog}
              onAppend={(data) => contactsArray.append(data)}
            />
          </section>

          {/* ── Employment ── */}
          <section className="py-6">
            <SectionHeader
              label="Опыт работы"
              action={
                <Button
                  type="button"
                  variant="ghost"
                  size="sm"
                  onClick={() => setEmploymentDialog(true)}
                  className="h-7 gap-1.5 px-2.5 text-xs text-muted-foreground hover:text-foreground"
                >
                  <Plus className="size-3" />
                  Добавить
                </Button>
              }
            />
            {employmentsArray.fields.length === 0 ? (
              <EmptyState
                icon={<Briefcase className="size-5" />}
                text="Места работы не добавлены"
                onAdd={() => setEmploymentDialog(true)}
              />
            ) : (
              <div className="space-y-2">
                {employmentsArray.fields.map((field, index) => (
                  <EmploymentCard
                    key={field.id}
                    field={field}
                    onRemove={() => employmentsArray.remove(index)}
                  />
                ))}
              </div>
            )}
            <EmploymentDialog
              open={employmentDialog}
              onOpenChange={setEmploymentDialog}
              onAppend={(data) => employmentsArray.append(data)}
            />
          </section>

          {/* ── Education ── */}
          <section className="py-6">
            <SectionHeader
              label="Образование"
              action={
                <Button
                  type="button"
                  variant="ghost"
                  size="sm"
                  onClick={() => setEducationDialog(true)}
                  className="h-7 gap-1.5 px-2.5 text-xs text-muted-foreground hover:text-foreground"
                >
                  <Plus className="size-3" />
                  Добавить
                </Button>
              }
            />
            {educationsArray.fields.length === 0 ? (
              <EmptyState
                icon={<GraduationCap className="size-5" />}
                text="Образование не добавлено"
                onAdd={() => setEducationDialog(true)}
              />
            ) : (
              <div className="space-y-2">
                {educationsArray.fields.map((field, index) => (
                  <EducationCard
                    key={field.id}
                    field={field}
                    onRemove={() => educationsArray.remove(index)}
                  />
                ))}
              </div>
            )}
            <EducationDialog
              open={educationDialog}
              onOpenChange={setEducationDialog}
              onAppend={(data) => educationsArray.append(data)}
            />
          </section>
        </div>

        {/* ── Save bar ── */}
        <div className="flex items-center justify-between border-t border-border/40 pt-6">
          {isDirty && !isPending ? (
            <p className="text-xs text-muted-foreground">
              Есть несохранённые изменения
            </p>
          ) : (
            <span />
          )}
          <Button type="submit" disabled={isPending || !isDirty} size="sm">
            {isPending && <Loader2 className="size-3.5 animate-spin" />}
            Сохранить изменения
          </Button>
        </div>
      </form>
    </Form>
  );
}

// ─── Public export ────────────────────────────────────────────────────────────

export function ProfileSettingsFeature() {
  const { data: profile, isLoading } = useProfileDetails();

  if (isLoading) {
    return <ProfileSettingsSkeleton />;
  }

  if (!profile) {
    return null;
  }

  return <ProfileEditor profile={profile} />;
}
