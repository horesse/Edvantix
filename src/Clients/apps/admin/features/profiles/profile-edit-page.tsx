"use client";

import type React from "react";
import { useEffect, useState } from "react";

import {
  AlertTriangle,
  ArrowLeft,
  BookOpen,
  Briefcase,
  Calendar,
  CheckCircle2,
  Loader2,
  Mail,
  Phone,
  Plus,
  Save,
  ShieldAlert,
  ShieldCheck,
  Sparkles,
  Trash2,
  User,
  X,
} from "lucide-react";
import Link from "next/link";
import { toast } from "sonner";

import useAdminProfile from "@workspace/api-hooks/admin/useAdminProfile";
import useBlockProfile from "@workspace/api-hooks/admin/useBlockProfile";
import useUnblockProfile from "@workspace/api-hooks/admin/useUnblockProfile";
import useUpdateAdminProfile from "@workspace/api-hooks/admin/useUpdateAdminProfile";
import type { AdminProfileDetailDto } from "@workspace/types/admin";
import {
  ContactType,
  type ContactRequest,
  type EducationRequest,
  EducationLevel,
  type EmploymentHistoryRequest,
} from "@workspace/types/profile";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@workspace/ui/components/alert-dialog";
import {
  Avatar,
  AvatarFallback,
  AvatarImage,
} from "@workspace/ui/components/avatar";
import { Badge } from "@workspace/ui/components/badge";
import {
  Breadcrumb,
  BreadcrumbItem,
  BreadcrumbLink,
  BreadcrumbList,
  BreadcrumbPage,
  BreadcrumbSeparator,
} from "@workspace/ui/components/breadcrumb";
import { Button } from "@workspace/ui/components/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@workspace/ui/components/card";
import { Input } from "@workspace/ui/components/input";
import { Label } from "@workspace/ui/components/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@workspace/ui/components/select";
import { Separator } from "@workspace/ui/components/separator";
import { Skeleton } from "@workspace/ui/components/skeleton";
import { Textarea } from "@workspace/ui/components/textarea";

// ── Constants ────────────────────────────────────────────────────────────────

const CONTACT_TYPE_LABELS: Record<number, { label: string; icon: typeof Mail }> = {
  [ContactType.Email]: { label: "Email", icon: Mail },
  [ContactType.Phone]: { label: "Телефон", icon: Phone },
  [ContactType.Uri]: { label: "Ссылка", icon: Sparkles },
  [ContactType.Other]: { label: "Другое", icon: User },
};

const EDUCATION_LEVEL_LABELS: Record<number, string> = {
  [EducationLevel.Preschool]: "Дошкольное",
  [EducationLevel.GeneralSecondary]: "Общее среднее",
  [EducationLevel.VocationalTechnical]: "Профессионально-техническое",
  [EducationLevel.SecondarySpecialized]: "Среднее специальное",
  [EducationLevel.HigherBachelor]: "Высшее (бакалавр)",
  [EducationLevel.HigherMaster]: "Высшее (магистр)",
  [EducationLevel.Postgraduate]: "Послевузовское",
  [EducationLevel.AdditionalChildren]: "Доп. образование (дети)",
  [EducationLevel.AdditionalAdults]: "Доп. образование (взрослые)",
  [EducationLevel.Special]: "Специальное",
};

// ── Loading skeleton ─────────────────────────────────────────────────────────

function PageSkeleton() {
  return (
    <div className="space-y-6">
      <Skeleton className="h-5 w-64" />
      <div className="flex items-center gap-4">
        <Skeleton className="size-16 rounded-full" />
        <div className="space-y-2">
          <Skeleton className="h-6 w-48" />
          <Skeleton className="h-4 w-32" />
        </div>
      </div>
      <div className="grid gap-6 lg:grid-cols-3">
        <div className="space-y-4 lg:col-span-2">
          <Skeleton className="h-[400px] w-full rounded-2xl" />
          <Skeleton className="h-[200px] w-full rounded-2xl" />
        </div>
        <Skeleton className="h-[300px] w-full rounded-2xl" />
      </div>
    </div>
  );
}

// ── Profile header ───────────────────────────────────────────────────────────

function ProfileHeader({ profile }: { profile: AdminProfileDetailDto }) {
  const initials = `${profile.lastName[0]}${profile.firstName[0]}`.toUpperCase();
  const lastLogin = profile.lastLoginAt
    ? new Date(profile.lastLoginAt).toLocaleString("ru-RU", {
        day: "2-digit",
        month: "long",
        year: "numeric",
        hour: "2-digit",
        minute: "2-digit",
      })
    : null;

  return (
    <div className="flex items-start gap-5">
      <Avatar className="size-16">
        {profile.avatarUrl ? (
          <AvatarImage src={profile.avatarUrl} alt={profile.firstName} />
        ) : null}
        <AvatarFallback className="from-primary/60 to-primary bg-gradient-to-br text-lg font-bold text-white">
          {initials}
        </AvatarFallback>
      </Avatar>
      <div className="min-w-0 flex-1">
        <div className="flex flex-wrap items-center gap-3">
          <h1 className="text-foreground text-2xl font-bold tracking-tight">
            {profile.lastName} {profile.firstName}{" "}
            {profile.middleName && (
              <span className="text-muted-foreground font-normal">
                {profile.middleName}
              </span>
            )}
          </h1>
          {profile.isBlocked ? (
            <Badge variant="destructive" className="gap-1">
              <ShieldAlert className="size-3" />
              Заблокирован
            </Badge>
          ) : (
            <Badge className="gap-1 border-transparent bg-emerald-100 text-emerald-700">
              <CheckCircle2 className="size-3" />
              Активен
            </Badge>
          )}
        </div>
        <div className="text-muted-foreground mt-1 flex flex-wrap items-center gap-x-4 gap-y-1 text-sm">
          <span>@{profile.userName}</span>
          <span className="text-border">|</span>
          <span className="font-mono text-xs">
            {profile.accountId.slice(0, 8)}…
          </span>
          {lastLogin && (
            <>
              <span className="text-border">|</span>
              <span className="flex items-center gap-1">
                <Calendar className="size-3" />
                Последний вход: {lastLogin}
              </span>
            </>
          )}
        </div>
      </div>
    </div>
  );
}

// ── Inline list item wrapper ─────────────────────────────────────────────────

function ListItem({
  children,
  onRemove,
}: {
  children: React.ReactNode;
  onRemove: () => void;
}) {
  return (
    <div className="group bg-muted/30 relative rounded-xl border p-4 transition-colors hover:border-rose-200">
      <button
        type="button"
        onClick={onRemove}
        className="absolute top-3 right-3 flex size-7 items-center justify-center rounded-lg text-rose-400 opacity-0 transition-opacity hover:bg-rose-50 hover:text-rose-600 group-hover:opacity-100"
        title="Удалить"
      >
        <Trash2 className="size-3.5" />
      </button>
      {children}
    </div>
  );
}

// ── Main page ────────────────────────────────────────────────────────────────

export function ProfileEditPage({ profileId }: { profileId: string }) {
  const { data: profile, isLoading, error } = useAdminProfile(profileId);

  // Personal info
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [middleName, setMiddleName] = useState("");
  const [birthDate, setBirthDate] = useState("");
  const [bio, setBio] = useState("");

  // Collections
  const [contacts, setContacts] = useState<ContactRequest[]>([]);
  const [employmentHistories, setEmploymentHistories] = useState<
    EmploymentHistoryRequest[]
  >([]);
  const [educations, setEducations] = useState<EducationRequest[]>([]);
  const [skills, setSkills] = useState<string[]>([]);
  const [newSkill, setNewSkill] = useState("");

  // Reason
  const [reason, setReason] = useState("");

  // Block/Unblock dialogs
  const [showBlockDialog, setShowBlockDialog] = useState(false);
  const [showUnblockDialog, setShowUnblockDialog] = useState(false);

  useEffect(() => {
    if (profile) {
      setFirstName(profile.firstName);
      setLastName(profile.lastName);
      setMiddleName(profile.middleName ?? "");
      setBirthDate(profile.birthDate);
      setBio(profile.bio ?? "");
      setContacts(
        profile.contacts.map((c) => ({
          type: c.type,
          value: c.value,
          description: c.description,
        })),
      );
      setEmploymentHistories(
        profile.employmentHistories.map((e) => ({
          workplace: e.workplace,
          position: e.position,
          startDate: e.startDate.slice(0, 10),
          endDate: e.endDate?.slice(0, 10) ?? null,
          description: e.description,
        })),
      );
      setEducations(
        profile.educations.map((e) => ({
          dateStart: e.dateStart,
          institution: e.institution,
          level: e.educationLevel,
          specialty: e.specialty,
          dateEnd: e.dateEnd,
        })),
      );
      setSkills(profile.skills.map((s) => s.name));
      setReason("");
    }
  }, [profile]);

  const update = useUpdateAdminProfile({
    onSuccess: () => {
      toast.success("Профиль успешно обновлён");
      setReason("");
    },
    onError: () => toast.error("Не удалось обновить профиль"),
  });

  const blockMutation = useBlockProfile({
    onSuccess: () => {
      toast.success("Пользователь заблокирован");
      setShowBlockDialog(false);
    },
    onError: () => toast.error("Не удалось заблокировать"),
  });

  const unblockMutation = useUnblockProfile({
    onSuccess: () => {
      toast.success("Блокировка снята");
      setShowUnblockDialog(false);
    },
    onError: () => toast.error("Не удалось разблокировать"),
  });

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!profileId || !reason.trim()) return;
    update.mutate({
      profileId,
      request: {
        firstName,
        lastName,
        middleName: middleName || null,
        birthDate,
        bio: bio || null,
        contacts,
        employmentHistories,
        educations,
        skills,
        reason,
      },
    });
  }

  // ── Collection helpers ───────────────────────────────────────────────────

  function addContact() {
    setContacts((prev) => [
      ...prev,
      { type: ContactType.Email, value: "", description: null },
    ]);
  }

  function updateContact(index: number, patch: Partial<ContactRequest>) {
    setContacts((prev) =>
      prev.map((c, i) => (i === index ? { ...c, ...patch } : c)),
    );
  }

  function removeContact(index: number) {
    setContacts((prev) => prev.filter((_, i) => i !== index));
  }

  function addEmployment() {
    setEmploymentHistories((prev) => [
      ...prev,
      {
        workplace: "",
        position: "",
        startDate: "",
        endDate: null,
        description: null,
      },
    ]);
  }

  function updateEmployment(
    index: number,
    patch: Partial<EmploymentHistoryRequest>,
  ) {
    setEmploymentHistories((prev) =>
      prev.map((e, i) => (i === index ? { ...e, ...patch } : e)),
    );
  }

  function removeEmployment(index: number) {
    setEmploymentHistories((prev) => prev.filter((_, i) => i !== index));
  }

  function addEducation() {
    setEducations((prev) => [
      ...prev,
      {
        dateStart: "",
        institution: "",
        level: EducationLevel.HigherBachelor,
        specialty: null,
        dateEnd: null,
      },
    ]);
  }

  function updateEducation(index: number, patch: Partial<EducationRequest>) {
    setEducations((prev) =>
      prev.map((e, i) => (i === index ? { ...e, ...patch } : e)),
    );
  }

  function removeEducation(index: number) {
    setEducations((prev) => prev.filter((_, i) => i !== index));
  }

  function addSkill() {
    const trimmed = newSkill.trim();
    if (!trimmed) return;
    if (skills.some((s) => s.toLowerCase() === trimmed.toLowerCase())) return;
    setSkills((prev) => [...prev, trimmed]);
    setNewSkill("");
  }

  function removeSkill(index: number) {
    setSkills((prev) => prev.filter((_, i) => i !== index));
  }

  // ── Error state ──────────────────────────────────────────────────────────

  if (error) {
    return (
      <div className="flex flex-col items-center justify-center py-24">
        <AlertTriangle className="text-muted-foreground mb-4 size-12" />
        <h2 className="text-foreground text-lg font-semibold">
          Профиль не найден
        </h2>
        <p className="text-muted-foreground mt-1 text-sm">
          Запрашиваемый профиль не существует или был удалён.
        </p>
        <Button variant="outline" className="mt-6" asChild>
          <Link href="/profiles">Вернуться к списку</Link>
        </Button>
      </div>
    );
  }

  if (isLoading || !profile) {
    return <PageSkeleton />;
  }

  return (
    <div className="space-y-6">
      {/* Breadcrumbs */}
      <Breadcrumb>
        <BreadcrumbList>
          <BreadcrumbItem>
            <BreadcrumbLink asChild>
              <Link href="/profiles">Профили</Link>
            </BreadcrumbLink>
          </BreadcrumbItem>
          <BreadcrumbSeparator />
          <BreadcrumbItem>
            <BreadcrumbPage>
              {profile.lastName} {profile.firstName}
            </BreadcrumbPage>
          </BreadcrumbItem>
        </BreadcrumbList>
      </Breadcrumb>

      {/* Header */}
      <div className="flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between">
        <ProfileHeader profile={profile} />
        <div className="flex shrink-0 items-center gap-2">
          <Button variant="outline" size="sm" asChild>
            <Link href="/profiles">
              <ArrowLeft className="size-4" />
              Назад
            </Link>
          </Button>
          {profile.isBlocked ? (
            <Button
              variant="outline"
              size="sm"
              onClick={() => setShowUnblockDialog(true)}
            >
              <ShieldCheck className="size-4" />
              Разблокировать
            </Button>
          ) : (
            <Button
              variant="outline"
              size="sm"
              className="text-destructive hover:bg-destructive/10"
              onClick={() => setShowBlockDialog(true)}
            >
              <ShieldAlert className="size-4" />
              Заблокировать
            </Button>
          )}
        </div>
      </div>

      <Separator />

      {/* Content */}
      <form onSubmit={handleSubmit}>
        <div className="grid gap-6 lg:grid-cols-3">
          {/* ── Left column: Edit form ── */}
          <div className="space-y-6 lg:col-span-2">
            {/* Personal info */}
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <User className="size-4" />
                  Личные данные
                </CardTitle>
                <CardDescription>
                  ФИО, дата рождения и описание пользователя.
                </CardDescription>
              </CardHeader>
              <CardContent className="space-y-5">
                <div className="grid gap-4 sm:grid-cols-2">
                  <div className="space-y-2">
                    <Label htmlFor="edit-lastName">Фамилия</Label>
                    <Input
                      id="edit-lastName"
                      value={lastName}
                      onChange={(e) => setLastName(e.target.value)}
                      placeholder="Иванов"
                      required
                    />
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="edit-firstName">Имя</Label>
                    <Input
                      id="edit-firstName"
                      value={firstName}
                      onChange={(e) => setFirstName(e.target.value)}
                      placeholder="Иван"
                      required
                    />
                  </div>
                </div>
                <div className="grid gap-4 sm:grid-cols-2">
                  <div className="space-y-2">
                    <Label htmlFor="edit-middleName">Отчество</Label>
                    <Input
                      id="edit-middleName"
                      value={middleName}
                      onChange={(e) => setMiddleName(e.target.value)}
                      placeholder="Иванович"
                    />
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="edit-birthDate">Дата рождения</Label>
                    <Input
                      id="edit-birthDate"
                      type="date"
                      value={birthDate}
                      onChange={(e) => setBirthDate(e.target.value)}
                      required
                    />
                  </div>
                </div>
                <div className="space-y-2">
                  <div className="flex items-center justify-between">
                    <Label htmlFor="edit-bio">О себе</Label>
                    <span className="text-muted-foreground text-xs tabular-nums">
                      {bio.length}/600
                    </span>
                  </div>
                  <Textarea
                    id="edit-bio"
                    value={bio}
                    onChange={(e) => setBio(e.target.value)}
                    placeholder="Краткое описание профиля"
                    rows={3}
                    maxLength={600}
                    className="resize-none"
                  />
                </div>
              </CardContent>
            </Card>

            {/* Contacts */}
            <Card>
              <CardHeader>
                <div className="flex items-center justify-between">
                  <CardTitle className="flex items-center gap-2">
                    <Mail className="size-4" />
                    Контакты
                  </CardTitle>
                  <Button
                    type="button"
                    variant="outline"
                    size="sm"
                    onClick={addContact}
                  >
                    <Plus className="size-3.5" />
                    Добавить
                  </Button>
                </div>
              </CardHeader>
              <CardContent className="space-y-3">
                {contacts.length === 0 && (
                  <p className="text-muted-foreground py-4 text-center text-sm">
                    Нет контактных данных
                  </p>
                )}
                {contacts.map((contact, i) => (
                  <ListItem key={i} onRemove={() => removeContact(i)}>
                    <div className="grid gap-3 pr-8 sm:grid-cols-3">
                      <div className="space-y-1.5">
                        <Label className="text-xs">Тип</Label>
                        <Select
                          value={String(contact.type)}
                          onValueChange={(v) =>
                            updateContact(i, { type: Number(v) })
                          }
                        >
                          <SelectTrigger className="h-9">
                            <SelectValue />
                          </SelectTrigger>
                          <SelectContent>
                            {Object.entries(CONTACT_TYPE_LABELS).map(
                              ([val, { label }]) => (
                                <SelectItem key={val} value={val}>
                                  {label}
                                </SelectItem>
                              ),
                            )}
                          </SelectContent>
                        </Select>
                      </div>
                      <div className="space-y-1.5">
                        <Label className="text-xs">Значение</Label>
                        <Input
                          value={contact.value}
                          onChange={(e) =>
                            updateContact(i, { value: e.target.value })
                          }
                          placeholder="user@example.com"
                          className="h-9"
                          required
                        />
                      </div>
                      <div className="space-y-1.5">
                        <Label className="text-xs">Описание</Label>
                        <Input
                          value={contact.description ?? ""}
                          onChange={(e) =>
                            updateContact(i, {
                              description: e.target.value || null,
                            })
                          }
                          placeholder="Рабочий"
                          className="h-9"
                        />
                      </div>
                    </div>
                  </ListItem>
                ))}
              </CardContent>
            </Card>

            {/* Employment history */}
            <Card>
              <CardHeader>
                <div className="flex items-center justify-between">
                  <CardTitle className="flex items-center gap-2">
                    <Briefcase className="size-4" />
                    Опыт работы
                  </CardTitle>
                  <Button
                    type="button"
                    variant="outline"
                    size="sm"
                    onClick={addEmployment}
                  >
                    <Plus className="size-3.5" />
                    Добавить
                  </Button>
                </div>
              </CardHeader>
              <CardContent className="space-y-3">
                {employmentHistories.length === 0 && (
                  <p className="text-muted-foreground py-4 text-center text-sm">
                    Нет записей об опыте работы
                  </p>
                )}
                {employmentHistories.map((emp, i) => (
                  <ListItem key={i} onRemove={() => removeEmployment(i)}>
                    <div className="space-y-3 pr-8">
                      <div className="grid gap-3 sm:grid-cols-2">
                        <div className="space-y-1.5">
                          <Label className="text-xs">Компания</Label>
                          <Input
                            value={emp.workplace}
                            onChange={(e) =>
                              updateEmployment(i, {
                                workplace: e.target.value,
                              })
                            }
                            placeholder="ООО «Компания»"
                            className="h-9"
                            required
                          />
                        </div>
                        <div className="space-y-1.5">
                          <Label className="text-xs">Должность</Label>
                          <Input
                            value={emp.position}
                            onChange={(e) =>
                              updateEmployment(i, { position: e.target.value })
                            }
                            placeholder="Разработчик"
                            className="h-9"
                            required
                          />
                        </div>
                      </div>
                      <div className="grid gap-3 sm:grid-cols-2">
                        <div className="space-y-1.5">
                          <Label className="text-xs">Начало</Label>
                          <Input
                            type="date"
                            value={emp.startDate}
                            onChange={(e) =>
                              updateEmployment(i, {
                                startDate: e.target.value,
                              })
                            }
                            className="h-9"
                            required
                          />
                        </div>
                        <div className="space-y-1.5">
                          <Label className="text-xs">Окончание</Label>
                          <Input
                            type="date"
                            value={emp.endDate ?? ""}
                            onChange={(e) =>
                              updateEmployment(i, {
                                endDate: e.target.value || null,
                              })
                            }
                            className="h-9"
                          />
                        </div>
                      </div>
                      <div className="space-y-1.5">
                        <Label className="text-xs">Описание</Label>
                        <Textarea
                          value={emp.description ?? ""}
                          onChange={(e) =>
                            updateEmployment(i, {
                              description: e.target.value || null,
                            })
                          }
                          placeholder="Описание обязанностей"
                          rows={2}
                          className="resize-none"
                        />
                      </div>
                    </div>
                  </ListItem>
                ))}
              </CardContent>
            </Card>

            {/* Education */}
            <Card>
              <CardHeader>
                <div className="flex items-center justify-between">
                  <CardTitle className="flex items-center gap-2">
                    <BookOpen className="size-4" />
                    Образование
                  </CardTitle>
                  <Button
                    type="button"
                    variant="outline"
                    size="sm"
                    onClick={addEducation}
                  >
                    <Plus className="size-3.5" />
                    Добавить
                  </Button>
                </div>
              </CardHeader>
              <CardContent className="space-y-3">
                {educations.length === 0 && (
                  <p className="text-muted-foreground py-4 text-center text-sm">
                    Нет записей об образовании
                  </p>
                )}
                {educations.map((edu, i) => (
                  <ListItem key={i} onRemove={() => removeEducation(i)}>
                    <div className="space-y-3 pr-8">
                      <div className="grid gap-3 sm:grid-cols-2">
                        <div className="space-y-1.5">
                          <Label className="text-xs">Учебное заведение</Label>
                          <Input
                            value={edu.institution}
                            onChange={(e) =>
                              updateEducation(i, {
                                institution: e.target.value,
                              })
                            }
                            placeholder="Название учебного заведения"
                            className="h-9"
                            required
                          />
                        </div>
                        <div className="space-y-1.5">
                          <Label className="text-xs">Уровень</Label>
                          <Select
                            value={String(edu.level)}
                            onValueChange={(v) =>
                              updateEducation(i, { level: Number(v) })
                            }
                          >
                            <SelectTrigger className="h-9">
                              <SelectValue />
                            </SelectTrigger>
                            <SelectContent>
                              {Object.entries(EDUCATION_LEVEL_LABELS).map(
                                ([val, label]) => (
                                  <SelectItem key={val} value={val}>
                                    {label}
                                  </SelectItem>
                                ),
                              )}
                            </SelectContent>
                          </Select>
                        </div>
                      </div>
                      <div className="grid gap-3 sm:grid-cols-3">
                        <div className="space-y-1.5">
                          <Label className="text-xs">Специальность</Label>
                          <Input
                            value={edu.specialty ?? ""}
                            onChange={(e) =>
                              updateEducation(i, {
                                specialty: e.target.value || null,
                              })
                            }
                            placeholder="Информатика"
                            className="h-9"
                          />
                        </div>
                        <div className="space-y-1.5">
                          <Label className="text-xs">Начало</Label>
                          <Input
                            type="date"
                            value={edu.dateStart}
                            onChange={(e) =>
                              updateEducation(i, { dateStart: e.target.value })
                            }
                            className="h-9"
                            required
                          />
                        </div>
                        <div className="space-y-1.5">
                          <Label className="text-xs">Окончание</Label>
                          <Input
                            type="date"
                            value={edu.dateEnd ?? ""}
                            onChange={(e) =>
                              updateEducation(i, {
                                dateEnd: e.target.value || null,
                              })
                            }
                            className="h-9"
                          />
                        </div>
                      </div>
                    </div>
                  </ListItem>
                ))}
              </CardContent>
            </Card>

            {/* Skills */}
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <Sparkles className="size-4" />
                  Навыки
                </CardTitle>
                <CardDescription>
                  Максимум 20 навыков. Введите название и нажмите Enter или
                  кнопку.
                </CardDescription>
              </CardHeader>
              <CardContent className="space-y-4">
                <div className="flex gap-2">
                  <Input
                    value={newSkill}
                    onChange={(e) => setNewSkill(e.target.value)}
                    onKeyDown={(e) => {
                      if (e.key === "Enter") {
                        e.preventDefault();
                        addSkill();
                      }
                    }}
                    placeholder="Название навыка"
                    className="h-9"
                    disabled={skills.length >= 20}
                  />
                  <Button
                    type="button"
                    variant="outline"
                    size="sm"
                    onClick={addSkill}
                    disabled={!newSkill.trim() || skills.length >= 20}
                  >
                    <Plus className="size-3.5" />
                  </Button>
                </div>
                {skills.length > 0 && (
                  <div className="flex flex-wrap gap-2">
                    {skills.map((skill, i) => (
                      <Badge
                        key={i}
                        variant="tag"
                        className="gap-1.5 py-1.5 pr-1.5 pl-3"
                      >
                        {skill}
                        <button
                          type="button"
                          onClick={() => removeSkill(i)}
                          className="hover:bg-secondary-foreground/10 flex size-4 items-center justify-center rounded-full transition-colors"
                        >
                          <X className="size-3" />
                        </button>
                      </Badge>
                    ))}
                  </div>
                )}
                <p className="text-muted-foreground text-xs">
                  {skills.length}/20 навыков
                </p>
              </CardContent>
            </Card>

            {/* Reason + Save */}
            <Card className="border-amber-200 bg-amber-50/30">
              <CardHeader>
                <CardTitle className="flex items-center gap-2 text-amber-800">
                  <AlertTriangle className="size-4" />
                  Причина изменений
                </CardTitle>
                <CardDescription>
                  Укажите, почему вносятся изменения. Пользователь получит
                  уведомление с этой информацией.
                </CardDescription>
              </CardHeader>
              <CardContent className="space-y-4">
                <Textarea
                  id="edit-reason"
                  value={reason}
                  onChange={(e) => setReason(e.target.value)}
                  placeholder="Например: исправление ошибки в написании ФИО по обращению пользователя"
                  rows={3}
                  maxLength={500}
                  required
                  className="resize-none"
                />
                <div className="flex items-center justify-between">
                  <p className="text-muted-foreground text-xs">
                    {reason.length}/500 символов
                  </p>
                  <Button
                    type="submit"
                    disabled={!reason.trim() || update.isPending}
                    className="gap-2"
                  >
                    {update.isPending ? (
                      <Loader2 className="size-4 animate-spin" />
                    ) : (
                      <Save className="size-4" />
                    )}
                    Сохранить изменения
                  </Button>
                </div>
              </CardContent>
            </Card>
          </div>

          {/* ── Right column: Info sidebar ── */}
          <div className="space-y-6">
            <Card>
              <CardHeader>
                <CardTitle className="text-sm">Информация</CardTitle>
              </CardHeader>
              <CardContent>
                <dl className="space-y-3 text-sm">
                  <div>
                    <dt className="text-muted-foreground text-xs">
                      ID профиля
                    </dt>
                    <dd className="text-foreground mt-0.5 break-all font-mono text-xs">
                      {profile.id}
                    </dd>
                  </div>
                  <Separator />
                  <div>
                    <dt className="text-muted-foreground text-xs">
                      ID аккаунта (Keycloak)
                    </dt>
                    <dd className="text-foreground mt-0.5 break-all font-mono text-xs">
                      {profile.accountId}
                    </dd>
                  </div>
                  <Separator />
                  <div>
                    <dt className="text-muted-foreground text-xs">Логин</dt>
                    <dd className="text-foreground mt-0.5">
                      @{profile.userName}
                    </dd>
                  </div>
                  <Separator />
                  <div>
                    <dt className="text-muted-foreground text-xs">Статус</dt>
                    <dd className="mt-1">
                      {profile.isBlocked ? (
                        <Badge variant="destructive" className="gap-1">
                          <ShieldAlert className="size-3" />
                          Заблокирован
                        </Badge>
                      ) : (
                        <Badge className="gap-1 border-transparent bg-emerald-100 text-emerald-700">
                          <CheckCircle2 className="size-3" />
                          Активен
                        </Badge>
                      )}
                    </dd>
                  </div>
                  <Separator />
                  <div>
                    <dt className="text-muted-foreground text-xs">
                      Последний вход
                    </dt>
                    <dd className="text-foreground mt-0.5 text-xs">
                      {profile.lastLoginAt
                        ? new Date(profile.lastLoginAt).toLocaleString(
                            "ru-RU",
                            {
                              day: "2-digit",
                              month: "long",
                              year: "numeric",
                              hour: "2-digit",
                              minute: "2-digit",
                            },
                          )
                        : "Никогда"}
                    </dd>
                  </div>
                </dl>
              </CardContent>
            </Card>

            {/* Stats summary */}
            <Card>
              <CardHeader>
                <CardTitle className="text-sm">Сводка</CardTitle>
              </CardHeader>
              <CardContent>
                <dl className="space-y-2.5 text-sm">
                  <div className="flex items-center justify-between">
                    <dt className="text-muted-foreground flex items-center gap-1.5">
                      <Mail className="size-3" />
                      Контакты
                    </dt>
                    <dd className="tabular-nums font-medium">
                      {contacts.length}
                    </dd>
                  </div>
                  <div className="flex items-center justify-between">
                    <dt className="text-muted-foreground flex items-center gap-1.5">
                      <Briefcase className="size-3" />
                      Опыт работы
                    </dt>
                    <dd className="tabular-nums font-medium">
                      {employmentHistories.length}
                    </dd>
                  </div>
                  <div className="flex items-center justify-between">
                    <dt className="text-muted-foreground flex items-center gap-1.5">
                      <BookOpen className="size-3" />
                      Образование
                    </dt>
                    <dd className="tabular-nums font-medium">
                      {educations.length}
                    </dd>
                  </div>
                  <div className="flex items-center justify-between">
                    <dt className="text-muted-foreground flex items-center gap-1.5">
                      <Sparkles className="size-3" />
                      Навыки
                    </dt>
                    <dd className="tabular-nums font-medium">
                      {skills.length}/20
                    </dd>
                  </div>
                </dl>
              </CardContent>
            </Card>
          </div>
        </div>
      </form>

      {/* ── Block/Unblock dialogs ── */}
      <AlertDialog open={showBlockDialog} onOpenChange={setShowBlockDialog}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Заблокировать пользователя?</AlertDialogTitle>
            <AlertDialogDescription>
              Пользователь{" "}
              <strong>
                {profile.lastName} {profile.firstName}
              </strong>{" "}
              (@{profile.userName}) не сможет войти в систему.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Отмена</AlertDialogCancel>
            <AlertDialogAction
              onClick={() => blockMutation.mutate(profileId)}
              disabled={blockMutation.isPending}
              className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
            >
              {blockMutation.isPending && (
                <Loader2 className="mr-2 size-4 animate-spin" />
              )}
              Заблокировать
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>

      <AlertDialog open={showUnblockDialog} onOpenChange={setShowUnblockDialog}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Разблокировать пользователя?</AlertDialogTitle>
            <AlertDialogDescription>
              Пользователь{" "}
              <strong>
                {profile.lastName} {profile.firstName}
              </strong>{" "}
              (@{profile.userName}) снова сможет войти в систему.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Отмена</AlertDialogCancel>
            <AlertDialogAction
              onClick={() => unblockMutation.mutate(profileId)}
              disabled={unblockMutation.isPending}
            >
              {unblockMutation.isPending && (
                <Loader2 className="mr-2 size-4 animate-spin" />
              )}
              Разблокировать
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  );
}
