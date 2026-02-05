"use client";

import {
  signIn as nextAuthSignIn,
  signOut as nextAuthSignOut,
} from "next-auth/react";

export const signIn = async () => {
  await nextAuthSignIn("keycloak", {
    callbackUrl: window.location.origin,
  });
};

export const signOut = async () => {
  await nextAuthSignOut({
    callbackUrl: window.location.origin,
  });
};

export { useSession } from "next-auth/react";
