"use client";

import { useEffect, useState } from "react";

import { useRouter } from "next/navigation";

import { zodResolver } from "@hookform/resolvers/zod";
import { useForm, useWatch } from "react-hook-form";
import { toast } from "sonner";

import useCreateOrganization from "@workspace/api-hooks/company/useCreateOrganization";
import {
  ContactType,
  LegalForm,
  OrganizationType,
} from "@workspace/types/company";
import {
  type CreateOrganizationInput,
  createOrganizationSchema,
} from "@workspace/validations/company";

import { LEGAL_FORM_DATA, STEP_FIELDS, WIZARD_STEPS } from "./constants";

const INITIAL_VALUES: CreateOrganizationInput = {
  fullLegalName: "",
  shortName: "",
  isLegalEntity: true,
  registrationDate: "",
  legalForm: undefined as unknown as LegalForm,
  organizationType: undefined as unknown as OrganizationType,
  primaryContactType: ContactType.Email,
  primaryContactValue: "",
  primaryContactDescription: "",
};

export type UseCreateWizardReturn = ReturnType<typeof useCreateWizard>;

export function useCreateWizard() {
  const router = useRouter();
  const [step, setStep] = useState(0);
  const [done, setDone] = useState(false);
  const [completed, setCompleted] = useState(new Set<number>());

  const form = useForm<CreateOrganizationInput>({
    resolver: zodResolver(createOrganizationSchema),
    defaultValues: INITIAL_VALUES,
  });

  const legalFormValue = useWatch({ control: form.control, name: "legalForm" });

  // Auto-derive isLegalEntity from legalForm selection
  useEffect(() => {
    if (legalFormValue !== undefined) {
      const entry = LEGAL_FORM_DATA.find((e) => e.value === legalFormValue);
      form.setValue("isLegalEntity", entry?.isLegalEntity ?? true);
    }
  }, [legalFormValue, form]);

  const mutation = useCreateOrganization({
    onSuccess: () => setDone(true),
    onError: () => toast.error("Не удалось создать организацию"),
  });

  async function handleNext() {
    const fields = STEP_FIELDS[step];
    if (fields) {
      const isValid = await form.trigger(fields);
      if (!isValid) return;
    }

    if (step === WIZARD_STEPS.length - 1) {
      await form.handleSubmit((data) => {
        mutation.mutate({
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
      })();
    } else {
      setCompleted((prev) => new Set(prev).add(step));
      setStep((s) => s + 1);
    }
  }

  function handleBack() {
    if (step > 0) setStep((s) => s - 1);
  }

  function jumpTo(i: number) {
    if (completed.has(i) || i === step) setStep(i);
  }

  function handleCancel() {
    router.back();
  }

  return {
    form,
    step,
    completed,
    done,
    legalFormValue,
    isPending: mutation.isPending,
    isLastStep: step === WIZARD_STEPS.length - 1,
    handleNext,
    handleBack,
    jumpTo,
    handleCancel,
  };
}
