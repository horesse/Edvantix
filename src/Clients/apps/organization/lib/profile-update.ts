import type {
  ContactRequest,
  EducationRequest,
  EmploymentHistoryRequest,
  OwnProfileDetails,
  UpdateProfileRequest,
} from "@workspace/types/profile";

type ProfileUpdateOverrides = Partial<
  Pick<
    UpdateProfileRequest,
    | "firstName"
    | "lastName"
    | "middleName"
    | "birthDate"
    | "contacts"
    | "educations"
    | "employmentHistories"
    | "avatar"
  >
>;

export function buildProfileUpdateRequest(
  profile: OwnProfileDetails,
  overrides: ProfileUpdateOverrides = {},
): UpdateProfileRequest {
  const contacts: ContactRequest[] =
    overrides.contacts ??
    profile.contacts.map((contact) => ({
      type: contact.type,
      value: contact.value,
      description: contact.description ?? null,
    }));

  const educations: EducationRequest[] =
    overrides.educations ??
    profile.educations.map((education) => ({
      dateStart: education.dateStart,
      dateEnd: education.dateEnd ?? null,
      institution: education.institution,
      specialty: education.specialty ?? null,
      level: education.educationLevel,
    }));

  const employmentHistories: EmploymentHistoryRequest[] =
    overrides.employmentHistories ??
    profile.employmentHistories.map((employment) => ({
      workplace: employment.workplace,
      position: employment.position,
      startDate: employment.startDate,
      endDate: employment.endDate ?? null,
      description: employment.description ?? null,
    }));

  return {
    firstName: overrides.firstName ?? profile.firstName,
    lastName: overrides.lastName ?? profile.lastName,
    middleName:
      overrides.middleName !== undefined
        ? overrides.middleName
        : profile.middleName ?? null,
    birthDate: overrides.birthDate ?? profile.birthDate,
    contacts,
    educations,
    employmentHistories,
    avatar: overrides.avatar,
  };
}
