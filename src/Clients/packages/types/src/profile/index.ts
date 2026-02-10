export type OwnProfile = {
  id: string;
  name: string;
  userName: string;
};

export enum Gender {
  Male = 1,
  Female = 2,
  None = 3,
}

export type RegisterProfileRequest = {
  birthDate: string;
  firstName: string;
  lastName: string;
  middleName?: string | null;
  gender: Gender;
  avatar?: File | null;
};
