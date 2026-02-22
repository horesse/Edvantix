import type {
  ContactRequest,
  EducationRequest,
  EmploymentHistoryRequest,
  OwnProfile,
  OwnProfileDetails,
  RegisterProfileRequest,
  UpdateProfileRequest,
} from "@workspace/types/profile";

import { apiClient } from "../client";
import type ApiClient from "../client";

class ProfileApiClient {
  private readonly client: ApiClient;

  constructor() {
    this.client = apiClient;
  }

  public async getProfile(): Promise<OwnProfile> {
    const response = await this.client.get<OwnProfile>(
      `/persona/api/v1/profile`,
    );
    return response.data;
  }

  public async getProfileDetails(): Promise<OwnProfileDetails> {
    const response = await this.client.get<OwnProfileDetails>(
      `/persona/api/v1/profile/details`,
    );
    return response.data;
  }

  public async updateProfile(
    request: UpdateProfileRequest,
  ): Promise<OwnProfile> {
    const formData = new FormData();
    formData.append("firstName", request.firstName);
    formData.append("lastName", request.lastName);
    formData.append("birthDate", request.birthDate);

    if (request.middleName) {
      formData.append("middleName", request.middleName);
    }

    appendContacts(formData, request.contacts);
    appendEducations(formData, request.educations);
    appendEmploymentHistories(formData, request.employmentHistories);

    if (request.avatar) {
      formData.append("avatar", request.avatar);
    }

    const response = await this.client.put<OwnProfile>(
      `/persona/api/v1/profile`,
      formData,
      { headers: { "Content-Type": "multipart/form-data" } },
    );
    return response.data;
  }

  public async registerProfile(
    request: RegisterProfileRequest,
  ): Promise<string> {
    const formData = new FormData();
    formData.append("firstName", request.firstName);
    formData.append("lastName", request.lastName);
    formData.append("birthDate", request.birthDate);
    formData.append("gender", String(request.gender));

    if (request.middleName) {
      formData.append("middleName", request.middleName);
    }

    if (request.avatar) {
      formData.append("avatar", request.avatar);
    }

    const response = await this.client.post<string>(
      `/persona/api/v1/profile/registration`,
      formData,
      { headers: { "Content-Type": "multipart/form-data" } },
    );
    return response.data;
  }
}

export default new ProfileApiClient();

function appendContacts(formData: FormData, contacts: ContactRequest[]) {
  contacts.forEach((contact, index) => {
    const prefix = `contacts[${index}]`;
    formData.append(`${prefix}.type`, String(contact.type));
    formData.append(`${prefix}.value`, contact.value);
    if (contact.description) {
      formData.append(`${prefix}.description`, contact.description);
    }
  });
}

function appendEducations(formData: FormData, educations: EducationRequest[]) {
  educations.forEach((education, index) => {
    const prefix = `educations[${index}]`;
    formData.append(`${prefix}.dateStart`, education.dateStart);
    formData.append(`${prefix}.institution`, education.institution);
    formData.append(`${prefix}.level`, String(education.level));
    if (education.specialty) {
      formData.append(`${prefix}.specialty`, education.specialty);
    }
    if (education.dateEnd) {
      formData.append(`${prefix}.dateEnd`, education.dateEnd);
    }
  });
}

function appendEmploymentHistories(
  formData: FormData,
  employmentHistories: EmploymentHistoryRequest[],
) {
  employmentHistories.forEach((employment, index) => {
    const prefix = `employmentHistories[${index}]`;
    formData.append(`${prefix}.workplace`, employment.workplace);
    formData.append(`${prefix}.position`, employment.position);
    formData.append(`${prefix}.startDate`, employment.startDate);
    if (employment.endDate) {
      formData.append(`${prefix}.endDate`, employment.endDate);
    }
    if (employment.description) {
      formData.append(`${prefix}.description`, employment.description);
    }
  });
}
