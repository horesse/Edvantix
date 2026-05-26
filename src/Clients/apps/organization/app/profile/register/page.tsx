"use client";

import { useRouter } from "next/navigation";

import useRegisterProfile from "@workspace/api-hooks/profiles/useRegisterProfile";
import { useTokenRefresh } from "@workspace/auth/use-token-refresh";

import { ProfileSetupPage } from "@/features/profile/setup/profile-setup-page";
import type { ProfileSetupValues } from "@/features/profile/setup/schema";
import { genderOptions } from "@/lib/profile-options";

/**
 * Maps the wizard gender value ("male" | "female" | "other") to the
 * numeric value expected by the API, using the project's genderOptions map.
 */
function mapGender(gender: ProfileSetupValues["gender"]): number {
  const map: Record<string, number> = {};
  for (const opt of genderOptions) {
    // genderOptions use labels; map by lower-case label matching
    map[opt.label.toLowerCase()] = opt.value;
  }
  // Fallback: male=1, female=2, other=0
  const fallback: Record<string, number> = { male: 1, female: 2, other: 0 };
  return map[gender] ?? fallback[gender] ?? 0;
}

/**
 * Profile registration page.
 *
 * Uses the new multi-step ProfileSetupPage wizard for the UI, and wires in
 * the real `useRegisterProfile` API mutation at this page boundary.
 *
 * Sits outside the (main) layout — no sidebar, minimal auth header only.
 */
export default function ProfileRegisterPage() {
  const router = useRouter();

  const { mutateAsync } = useRegisterProfile();
  const refreshToken = useTokenRefresh();

  async function handleSubmit(
    values: ProfileSetupValues,
    avatarFile: File | null,
  ) {
    await mutateAsync({
      lastName: values.lastName,
      firstName: values.firstName,
      middleName: values.patronymic ?? null,
      birthDate: values.birthDate,
      gender: mapGender(values.gender),
      avatar: avatarFile,
    });

    // After profile creation Keycloak updates the user's claims.
    // Force a silent token refresh so the new profile_id claim is included.
    await refreshToken();

    router.push("/");
  }

  return <ProfileSetupPage onSubmit={handleSubmit} />;
}
