"use client";

import { useCallback, useEffect, useMemo, useState } from "react";

import { zodResolver } from "@hookform/resolvers/zod";
import { ChevronRight, CircleCheck, History } from "lucide-react";
import { useController, useForm } from "react-hook-form";
import { toast } from "sonner";

import useArchiveOrganization from "@workspace/api-hooks/organization/useArchiveOrganization";
import useUpdateOrganization from "@workspace/api-hooks/organization/useUpdateOrganization";
import {
  ContactType,
  LEGAL_FORM_LABELS,
  OrganizationType,
} from "@workspace/types/organization";
import type { OrganizationDetailDto } from "@workspace/types/organization";
import { Badge } from "@workspace/ui/components/badge";
import { Button } from "@workspace/ui/components/button";

import { ArchiveConfirmDialog } from "./dialogs/archive-confirm-dialog";
import { LegalFormConfirmDialog } from "./dialogs/legal-form-confirm-dialog";
import { ResetConfirmDialog } from "./dialogs/reset-confirm-dialog";
import { SaveBar } from "./save-bar";
import { editFormSchema, pluralRu } from "./schema";
import type { EditFormValues } from "./schema";
import { BasicInfoSection } from "./sections/basic-info-section";
import { ContactSection } from "./sections/contact-section";
import { DangerZoneSection } from "./sections/danger-zone-section";
import { LegalFormSection } from "./sections/legal-form-section";

interface OrgSettingsFormProps {
  org: OrganizationDetailDto;
}

export function OrgSettingsForm({ org }: Readonly<OrgSettingsFormProps>) {
  const buildDefaults = useCallback(
    (o: OrganizationDetailDto): EditFormValues => {
      const pc = o.contacts.find((c) => c.isPrimary) ?? null;
      return {
        fullLegalName: o.fullLegalName,
        shortName: o.shortName ?? "",
        legalForm: o.legalForm,
        organizationType: o.organizationType,
        registrationDate: o.registrationDate,
        contactType: pc?.contactType ?? ContactType.Email,
        contactValue: pc?.value ?? "",
        contactDescription: pc?.description ?? "",
      };
    },
    [],
  );

  const [defaults, setDefaults] = useState<EditFormValues>(() =>
    buildDefaults(org),
  );

  const {
    register,
    handleSubmit,
    reset,
    watch,
    control,
    formState: { errors },
  } = useForm<EditFormValues>({
    resolver: zodResolver(editFormSchema),
    defaultValues: defaults,
  });

  useEffect(() => {
    const d = buildDefaults(org);
    setDefaults(d);
    reset(d);
  }, [org, reset, buildDefaults]);

  const { field: legalFormField } = useController({
    control,
    name: "legalForm",
  });
  const { field: orgTypeField } = useController({
    control,
    name: "organizationType",
  });
  const { field: contactTypeField } = useController({
    control,
    name: "contactType",
  });
  const { field: contactDescField } = useController({
    control,
    name: "contactDescription",
  });

  const watched = watch();

  const changedOrg = useMemo(() => {
    const d: Partial<Record<keyof EditFormValues, boolean>> = {};
    if (watched.fullLegalName !== defaults.fullLegalName)
      d.fullLegalName = true;
    if ((watched.shortName ?? "") !== (defaults.shortName ?? ""))
      d.shortName = true;
    if (watched.legalForm !== defaults.legalForm) d.legalForm = true;
    if (watched.organizationType !== defaults.organizationType)
      d.organizationType = true;
    if (watched.registrationDate !== defaults.registrationDate)
      d.registrationDate = true;
    return d;
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [watched]);

  const changedContact = useMemo(() => {
    const d: Partial<Record<keyof EditFormValues, boolean>> = {};
    if (watched.contactType !== defaults.contactType) d.contactType = true;
    if (watched.contactValue !== defaults.contactValue) d.contactValue = true;
    if (watched.contactDescription !== defaults.contactDescription)
      d.contactDescription = true;
    return d;
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [watched]);

  const changedCount =
    Object.keys(changedOrg).length + Object.keys(changedContact).length;
  const hasChanges = changedCount > 0;

  const [savingState, setSavingState] = useState<"idle" | "saving" | "saved">(
    "idle",
  );
  const [showLegalFormConfirm, setShowLegalFormConfirm] = useState(false);
  const [showResetConfirm, setShowResetConfirm] = useState(false);
  const [showArchiveConfirm, setShowArchiveConfirm] = useState(false);
  const [submitAttempted, setSubmitAttempted] = useState(false);

  useEffect(() => {
    if (hasChanges && savingState === "saved") setSavingState("idle");
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [hasChanges]);

  useEffect(() => {
    const handler = (e: BeforeUnloadEvent) => {
      if (hasChanges) {
        e.preventDefault();
        e.returnValue = "";
      }
    };
    window.addEventListener("beforeunload", handler);
    return () => window.removeEventListener("beforeunload", handler);
  }, [hasChanges]);

  const updateOrgMutation = useUpdateOrganization();
  const archiveMutation = useArchiveOrganization({
    onSuccess: () => {
      toast.success("Организация архивирована");
      setShowArchiveConfirm(false);
    },
    onError: () => toast.error("Не удалось архивировать организацию"),
  });

  async function doSave(values: EditFormValues) {
    setSavingState("saving");
    try {
      await updateOrgMutation.mutateAsync({
        id: org.id,
        request: {
          fullLegalName: values.fullLegalName,
          shortName: values.shortName || null,
          organizationType: values.organizationType,
          legalForm: values.legalForm,
          registrationDate: values.registrationDate,
          contactType: values.contactType,
          contactValue: values.contactValue,
          contactDescription: values.contactDescription,
        },
      });
      setDefaults(values);
      reset(values);
      setSavingState("saved");
      toast.success("Изменения сохранены");
    } catch {
      setSavingState("idle");
      toast.error("Не удалось сохранить изменения");
    }
  }

  const onSaveClick = handleSubmit((values) => {
    setSubmitAttempted(true);
    if (changedOrg.legalForm) {
      setShowLegalFormConfirm(true);
    } else {
      void doSave(values);
    }
  });

  const onResetConfirm = () => {
    reset(defaults);
    setSubmitAttempted(false);
    setSavingState("idle");
    setShowResetConfirm(false);
  };

  const isSaving = savingState === "saving";
  const lastModifiedFormatted = org.lastModifiedAt
    ? new Date(org.lastModifiedAt).toLocaleDateString("ru-RU", {
        day: "numeric",
        month: "long",
        year: "numeric",
        hour: "2-digit",
        minute: "2-digit",
      })
    : null;

  return (
    <>
      <div className="mb-5 flex flex-wrap items-center justify-between gap-3">
        <nav className="flex items-center gap-1.5 text-sm text-slate-500">
          <span>Организация</span>
          <ChevronRight className="size-3.5 text-slate-300" />
          <span>{org.shortName ?? org.fullLegalName}</span>
          <ChevronRight className="size-3.5 text-slate-300" />
          <span className="font-medium text-slate-900">Настройки</span>
        </nav>
        <Button type="button" variant="outline" size="sm">
          <History className="size-3.5" />
          История изменений
        </Button>
      </div>

      <div className="border-border mb-5 flex flex-wrap items-center gap-4 rounded-2xl border bg-white px-6 py-4">
        <div
          className="flex size-14 shrink-0 items-center justify-center rounded-[14px] text-xl font-bold text-white"
          style={{
            background: "linear-gradient(135deg,#6366f1,#8b5cf6)",
            boxShadow: "0 4px 12px rgba(99,102,241,0.3)",
          }}
        >
          {(org.shortName ?? org.fullLegalName).trim().charAt(0)}
        </div>
        <div className="min-w-0 flex-1">
          <div className="flex flex-wrap items-center gap-2">
            <h1 className="text-xl font-bold tracking-tight text-slate-900">
              Редактирование организации
            </h1>
            <Badge variant="secondary" className="text-[11px]">
              {LEGAL_FORM_LABELS[watched.legalForm]}
            </Badge>
          </div>
          <p className="mt-0.5 text-[13px] text-slate-500">
            {hasChanges ? (
              <span className="font-medium text-amber-700">
                {changedCount}{" "}
                {pluralRu(
                  changedCount,
                  "несохранённое изменение",
                  "несохранённых изменения",
                  "несохранённых изменений",
                )}
              </span>
            ) : savingState === "saved" ? (
              <span className="inline-flex items-center gap-1.5 text-emerald-700">
                <CircleCheck className="size-3.5" />
                Сохранено только что
              </span>
            ) : lastModifiedFormatted ? (
              <span>Последнее изменение: {lastModifiedFormatted}</span>
            ) : (
              <span>
                Зарегистрировано:{" "}
                {new Date(org.registrationDate).toLocaleDateString("ru-RU")}
              </span>
            )}
          </p>
        </div>
      </div>

      <form className="space-y-5">
        <LegalFormSection
          value={legalFormField.value}
          onChange={legalFormField.onChange}
          changed={!!changedOrg.legalForm}
          error={errors.legalForm?.message}
          submitAttempted={submitAttempted}
        />

        <BasicInfoSection
          register={register}
          orgTypeValue={orgTypeField.value as OrganizationType}
          onOrgTypeChange={orgTypeField.onChange}
          changedFields={changedOrg}
          errors={errors}
          submitAttempted={submitAttempted}
        />

        <ContactSection
          register={register}
          contactTypeValue={contactTypeField.value as ContactType}
          onContactTypeChange={(v) => {
            contactTypeField.onChange(v);
          }}
          contactDescriptionValue={contactDescField.value ?? ""}
          onContactDescriptionChange={contactDescField.onChange}
          changedFields={changedContact}
          errors={errors}
          submitAttempted={submitAttempted}
        />

        <DangerZoneSection
          onArchive={() => setShowArchiveConfirm(true)}
          isArchiving={archiveMutation.isPending}
        />

        {(hasChanges || isSaving) && (
          <SaveBar
            changedCount={changedCount}
            isSaving={isSaving}
            onSave={() => void onSaveClick()}
            onReset={() => setShowResetConfirm(true)}
          />
        )}
      </form>

      <LegalFormConfirmDialog
        open={showLegalFormConfirm}
        onClose={() => setShowLegalFormConfirm(false)}
        onConfirm={() => {
          setShowLegalFormConfirm(false);
          void doSave(watch());
        }}
        newLegalForm={watched.legalForm}
      />

      <ResetConfirmDialog
        open={showResetConfirm}
        onClose={() => setShowResetConfirm(false)}
        onConfirm={onResetConfirm}
      />

      <ArchiveConfirmDialog
        open={showArchiveConfirm}
        onClose={() => setShowArchiveConfirm(false)}
        onConfirm={() => archiveMutation.mutate(org.id)}
        isLoading={archiveMutation.isPending}
      />
    </>
  );
}
