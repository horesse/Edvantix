"use client";

import { useRouter } from "next/navigation";

import { Form } from "@workspace/ui/components/form";

import { SuccessScreen } from "./components/success-screen";
import { WIZARD_STEPS } from "./constants";
import { StepperSidebar } from "./stepper-sidebar";
import { StepAbout } from "./steps/step-about";
import { StepContact } from "./steps/step-contact";
import { StepLegalForm } from "./steps/step-legal-form";
import { StepReview } from "./steps/step-review";
import { useCreateWizard } from "./use-create-wizard";
import { WizardLayout } from "./wizard-layout";

/**
 * 4-step onboarding wizard for creating an organization.
 *
 * Step 1: Legal form (card-radio grid) → auto-derives isLegalEntity
 * Step 2: Organization details (name, date, type)
 * Step 3: Primary contact (segmented channel + value + comment)
 * Step 4: Review & submit
 */
export function CreateOrganizationPage() {
  const router = useRouter();
  const wizard = useCreateWizard();
  const { form, step, completed, done, isPending, isLastStep } = wizard;

  if (done) {
    const values = form.getValues();
    return (
      <div className="-mx-4 -my-4 flex h-screen overflow-hidden lg:-mx-6 lg:-my-6">
        <StepperSidebar
          current={WIZARD_STEPS.length}
          completed={new Set([0, 1, 2, 3])}
        />
        <div className="flex flex-1 items-center justify-center p-8">
          <SuccessScreen
            name={values.shortName || values.fullLegalName}
            onDashboard={() => router.push("/")}
          />
        </div>
      </div>
    );
  }

  return (
    <WizardLayout
      step={step}
      completed={completed}
      isLast={isLastStep}
      isPending={isPending}
      onBack={wizard.handleBack}
      onNext={wizard.handleNext}
      onCancel={wizard.handleCancel}
      onJump={wizard.jumpTo}
    >
      <Form {...form}>
        <form>
          {step === 0 && <StepLegalForm control={form.control} />}
          {step === 1 && <StepAbout control={form.control} />}
          {step === 2 && <StepContact control={form.control} />}
          {step === 3 && (
            <StepReview data={form.getValues()} goTo={wizard.jumpTo} />
          )}
        </form>
      </Form>
    </WizardLayout>
  );
}
