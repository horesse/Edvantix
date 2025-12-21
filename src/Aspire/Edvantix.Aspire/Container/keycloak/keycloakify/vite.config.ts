import react from "@vitejs/plugin-react";
import {keycloakify} from "keycloakify/vite-plugin";
import {defineConfig} from "vite";
import path from "path"
import tailwindcss from '@tailwindcss/vite'

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [
        react(),
        tailwindcss(),
        keycloakify({
            themeName: "edvantix",
            themeVersion: "1.0.0",
            groupId: "com.edvantix.edvantix.keycloak",
            artifactId: "keycloak-theme-edvantix",
            accountThemeImplementation: "none",
            keycloakifyBuildDirPath: "../themes",
        }),
    ],
    resolve: {
        alias: {
            "@": path.resolve(__dirname, "./src"),
        },
    },
});
