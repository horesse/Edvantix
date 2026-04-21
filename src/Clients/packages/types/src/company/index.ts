// --- Enums ---

/** Тип контакта организации. */
export enum ContactType {
  Email = 0,
  MobilePhone = 1,
  Telegram = 2,
  WhatsApp = 3,
  Viber = 4,
}

/** Организационно-правовая форма. */
export enum LegalForm {
  Llc = 0,
  Ojsc = 1,
  Cjsc = 2,
  Ue = 3,
  Pue = 4,
  IndividualEntrepreneur = 5,
  ProductionCooperative = 6,
  StateEducationalInstitution = 7,
  PrivateEducationalInstitution = 8,
  EducationalInstitution = 9,
}

export const LEGAL_FORM_LABELS: Record<LegalForm, string> = {
  [LegalForm.Llc]: "ООО",
  [LegalForm.Ojsc]: "ОАО",
  [LegalForm.Cjsc]: "ЗАО",
  [LegalForm.Ue]: "УП",
  [LegalForm.Pue]: "ЧУП",
  [LegalForm.IndividualEntrepreneur]: "ИП",
  [LegalForm.ProductionCooperative]: "Кооператив",
  [LegalForm.StateEducationalInstitution]: "ГУО",
  [LegalForm.PrivateEducationalInstitution]: "ЧУО",
  [LegalForm.EducationalInstitution]: "Образовательное учреждение",
};

/** Статус организационной сущности. */
export enum OrganizationStatus {
  Active = 0,
  Archived = 1,
  Deleted = 2,
}

/** Тип образовательного или бизнес-учреждения. */
export enum OrganizationType {
  EducationalInstitution = 0,
  GeneralEducationSchool = 1,
  Lyceum = 2,
  Gymnasium = 3,
  College = 4,
  VocationalSchool = 5,
  University = 6,
  AdditionalEducation = 7,
  Preschool = 8,
  PrivateEducationalCenter = 9,
  TrainingCompany = 10,
  LlcEducation = 11,
  IndividualEntrepreneur = 12,
  LanguageSchool = 13,
  ItSchool = 14,
  TutoringCenter = 15,
  OnlinePlatform = 16,
}

export const ORGANIZATION_TYPE_LABELS: Record<OrganizationType, string> = {
  [OrganizationType.EducationalInstitution]: "Учреждение образования",
  [OrganizationType.GeneralEducationSchool]:
    "Учреждение общего среднего образования",
  [OrganizationType.Lyceum]: "Лицей",
  [OrganizationType.Gymnasium]: "Гимназия",
  [OrganizationType.College]: "Колледж",
  [OrganizationType.VocationalSchool]: "Профессионально-техническое училище",
  [OrganizationType.University]: "Университет, институт",
  [OrganizationType.AdditionalEducation]:
    "Учреждение дополнительного образования детей и молодёжи",
  [OrganizationType.Preschool]: "Дошкольное учреждение образования",
  [OrganizationType.PrivateEducationalCenter]: "Частный образовательный центр",
  [OrganizationType.TrainingCompany]: "Учебный центр, обучающая компания",
  [OrganizationType.LlcEducation]: "ООО в сфере образования",
  [OrganizationType.IndividualEntrepreneur]: "Индивидуальный предприниматель",
  [OrganizationType.LanguageSchool]: "Языковая школа",
  [OrganizationType.ItSchool]: "IT-школа, школа программирования",
  [OrganizationType.TutoringCenter]: "Репетиторский центр",
  [OrganizationType.OnlinePlatform]: "Онлайн-платформа",
};

// --- DTOs ---

export type ContactDto = {
  readonly id: string;
  readonly value: string;
  readonly description: string;
  readonly contactType: ContactType;
  readonly isPrimary: boolean;
};

/** Краткая сводка организации (используется в списке). */
export type OrganizationDto = {
  readonly id: string;
  readonly fullLegalName: string;
  readonly shortName: string | null;
  readonly organizationType: OrganizationType;
  readonly status: OrganizationStatus;
  readonly isLegalEntity: boolean;
};

/** Полные данные организации (используется на странице деталей/настроек). */
export type OrganizationDetailDto = {
  readonly id: string;
  readonly fullLegalName: string;
  readonly shortName: string | null;
  readonly isLegalEntity: boolean;
  readonly registrationDate: string;
  readonly legalForm: LegalForm;
  readonly countryId: string;
  readonly currencyId: string;
  readonly organizationType: OrganizationType;
  readonly status: OrganizationStatus;
  readonly contacts: readonly ContactDto[];
};

/** Организация с ролью текущего пользователя в ней (эндпоинт /organizations/mine). */
export type OrganizationWithRoleDto = OrganizationDto & {
  readonly roleCode: string;
  readonly roleDescription: string | null;
};

/** Участник организации. */
export type OrganizationMemberDto = {
  readonly id: string;
  readonly organizationId: string;
  readonly profileId: string;
  readonly organizationMemberRoleId: string;
  readonly status: OrganizationStatus;
  readonly startDate: string;
  readonly endDate: string | null;
};

// --- Request types ---

export type CreateOrganizationRequest = {
  readonly fullLegalName: string;
  readonly shortName?: string | null;
  readonly isLegalEntity: boolean;
  readonly registrationDate: string;
  readonly legalForm: LegalForm;
  readonly organizationType: OrganizationType;
  readonly primaryContactValue: string;
  readonly primaryContactType: ContactType;
  readonly primaryContactDescription: string;
};

export type UpdateOrganizationRequest = {
  readonly fullLegalName: string;
  readonly shortName?: string | null;
  readonly organizationType: OrganizationType;
  readonly legalForm: LegalForm;
};

export type OrganizationsQuery = {
  readonly pageIndex?: number;
  readonly pageSize?: number;
  readonly search?: string;
  readonly status?: OrganizationStatus;
  readonly organizationType?: OrganizationType;
};

export type CreateOrganizationMemberRequest = {
  readonly profileId: string;
  readonly organizationMemberRoleId: string;
  readonly startDate: string;
  readonly endDate?: string | null;
};

export type UpdateOrganizationMemberRequest = {
  readonly organizationMemberRoleId: string;
};

export type OrganizationMembersQuery = {
  readonly pageIndex?: number;
  readonly pageSize?: number;
  readonly status?: OrganizationStatus;
};
