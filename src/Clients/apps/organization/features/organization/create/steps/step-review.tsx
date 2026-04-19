import { Shield } from "lucide-react";

import { ORGANIZATION_TYPE_LABELS } from "@workspace/types/company";
import type { CreateOrganizationInput } from "@workspace/validations/company";

import { InfoCallout } from "../components/info-callout";
import { ReviewRow, ReviewSection } from "../components/review-section";
import { StepHeader } from "../components/step-header";
import { CONTACT_TYPE_DATA, LEGAL_FORM_DATA } from "../constants";

function formatDate(iso: string): string {
  if (!iso) return "—";
  try {
    return new Date(iso).toLocaleDateString("ru-RU", {
      day: "numeric",
      month: "long",
      year: "numeric",
    });
  } catch {
    return iso;
  }
}

interface StepReviewProps {
  data: CreateOrganizationInput;
  goTo: (step: number) => void;
}

export function StepReview({ data, goTo }: StepReviewProps) {
  const lf = LEGAL_FORM_DATA.find((e) => e.value === data.legalForm);
  const ct = CONTACT_TYPE_DATA.find((c) => c.value === data.primaryContactType);

  return (
    <div className="flex flex-col gap-4">
      <StepHeader
        eyebrow="Шаг 4 из 4"
        title="Проверьте данные"
        subtitle="Проверьте введённую информацию. После подтверждения школа появится в вашем кабинете."
      />

      <ReviewSection title="Форма собственности" onEdit={() => goTo(0)}>
        <ReviewRow label="Форма" value={lf ? `${lf.tag} — ${lf.label}` : "—"} />
        <ReviewRow
          label="Регистрируется как"
          value={data.isLegalEntity ? "Юридическое лицо" : "Физическое лицо"}
        />
      </ReviewSection>

      <ReviewSection title="Об организации" onEdit={() => goTo(1)}>
        <ReviewRow label="Полное наименование" value={data.fullLegalName} />
        <ReviewRow
          label="Краткое название"
          value={data.shortName || undefined}
          empty="не указано"
        />
        <ReviewRow
          label="Дата регистрации"
          value={formatDate(data.registrationDate)}
        />
        <ReviewRow
          label="Тип организации"
          value={
            data.organizationType !== undefined
              ? ORGANIZATION_TYPE_LABELS[data.organizationType]
              : undefined
          }
        />
      </ReviewSection>

      <ReviewSection title="Основной контакт" onEdit={() => goTo(2)}>
        <ReviewRow
          label="Канал"
          value={
            ct ? (
              <span className="inline-flex items-center gap-1.5">
                <ct.Icon className="text-brand-600 size-3.5" />
                {ct.label}
              </span>
            ) : undefined
          }
        />
        <ReviewRow label="Значение" value={data.primaryContactValue} />
        <ReviewRow
          label="Комментарий"
          value={data.primaryContactDescription || undefined}
          empty="не указан"
        />
      </ReviewSection>

      <InfoCallout
        variant="success"
        icon={Shield}
        title="Данные защищены"
        description="Информация передаётся по защищённому каналу."
      />
    </div>
  );
}
