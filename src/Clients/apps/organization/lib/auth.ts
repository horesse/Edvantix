import NextAuth from "next-auth";
import Keycloak from "next-auth/providers/keycloak";

import { env } from "@/env.mjs";

export const { handlers, signOut } = NextAuth({
  providers: [
    Keycloak({
      clientId: env.KEYCLOAK_CLIENT_ID!,
      clientSecret: env.KEYCLOAK_CLIENT_SECRET || "",
      issuer: `${env.KEYCLOAK_URL}/realms/${env.KEYCLOAK_REALM}`,
      redirectProxyUrl: `${env.NEXT_PUBLIC_APP_URL}/api/auth`,
      authorization: {
        params: {
          scope: "openid profile email profile_read profile_write",
        },
      },
    }),
  ],
  secret: env.AUTH_SECRET,
  basePath: "/api/auth",
  trustHost: true,
  // debug: process.env.NODE_ENV === "development",
  callbacks: {
    async jwt({ token, account, profile }) {
      if (account && profile) {
        token.accessToken = account.access_token;
        token.idToken = account.id_token;
        token.refreshToken = account.refresh_token;
        token.expiresAt = account.expires_at;
      }

      return token;
    },
    async session({ session, token }) {
      return {
        ...session,
        accessToken: token.accessToken as string,
        idToken: token.idToken as string,
        error: token.error as string | undefined,
      };
    },
  },
});
