import react from "@vitejs/plugin-react";
import { keycloakify } from "keycloakify/vite-plugin";
import { defineConfig } from "vite";

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [
    react(),
    keycloakify({
      themeName: "edvantix",
      themeVersion: "1.0.0",
      groupId: "com.edvantix.bookworm.keycloak",
      artifactId: "keycloak-theme-edvantix",
      accountThemeImplementation: "none",
      keycloakifyBuildDirPath: "../themes",
    }),
  ],
});
