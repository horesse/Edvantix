export type OwnProfile = {
  id: string;
  name: string;
  userName: string;
};

export enum Gender {
  Male = 0,
  Female = 1,
  Other = 2,
}

export type ProfileModel = {
  birthDate: string;
  firstName: string;
  lastName: string;
  middleName?: string | null;
};

export type RegisterProfileRequest = {
  gender: Gender;
  profile: ProfileModel;
};
