import type { Meta, StoryObj } from "@storybook/react-vite";
import { createKcPageStory } from "../KcPageStory";

const { KcPageStory } = createKcPageStory({ pageId: "register.ftl" });

// Common social providers to avoid duplication
const SOCIAL_PROVIDERS = [
  {
    loginUrl: "google",
    alias: "google",
    providerId: "google",
    displayName: "Google",
    iconClasses: "fa fa-google",
  },
  {
    loginUrl: "microsoft",
    alias: "microsoft",
    providerId: "microsoft",
    displayName: "Microsoft",
    iconClasses: "fa fa-windows",
  },
  {
    loginUrl: "facebook",
    alias: "facebook",
    providerId: "facebook",
    displayName: "Facebook",
    iconClasses: "fa fa-facebook",
  },
  {
    loginUrl: "github",
    alias: "github",
    providerId: "github",
    displayName: "Github",
    iconClasses: "fa fa-github",
  },
];

const meta = {
  title: "login/register.ftl",
  component: KcPageStory,
} satisfies Meta<typeof KcPageStory>;

export default meta;

type Story = StoryObj<typeof meta>;

export const Default: Story = {
  render: () => <KcPageStory />,
};

export const WithFieldErrors: Story = {
  render: () => (
    <KcPageStory
      kcContext={{
        messagesPerField: {
          existsError: (fieldName: string, ...otherFieldNames: string[]) => {
            const fieldNames = new Set([fieldName, ...otherFieldNames]);
            return (
              fieldNames.has("firstName") ||
              fieldNames.has("email") ||
              fieldNames.has("password")
            );
          },
          get: (fieldName: string) => {
            if (fieldName === "firstName") {
              return "First name is required.";
            }
            if (fieldName === "email") {
              return "Invalid email format.";
            }
            if (fieldName === "password") {
              return "Password must be at least 8 characters.";
            }
            return "";
          },
        },
      }}
    />
  ),
};

export const WithPasswordMismatch: Story = {
  render: () => (
    <KcPageStory
      kcContext={{
        messagesPerField: {
          existsError: (fieldName: string) => {
            return fieldName === "password-confirm";
          },
          get: (fieldName: string) => {
            if (fieldName === "password-confirm") {
              return "Passwords do not match.";
            }
            return "";
          },
        },
      }}
    />
  ),
};

export const WithPrefilledData: Story = {
  render: () => (
    <KcPageStory
      kcContext={{
        register: {
          formData: {
            firstName: "Иван",
            lastName: "Иванов",
            email: "ivan.ivanov@example.com",
            username: "ivan.ivanov",
          },
        },
      }}
    />
  ),
};

export const WithEmailAsUsername: Story = {
  render: () => (
    <KcPageStory
      kcContext={{
        realm: {
          registrationEmailAsUsername: true,
        },
      }}
    />
  ),
};

export const WithoutPassword: Story = {
  render: () => (
    <KcPageStory
      kcContext={{
        passwordRequired: false,
      }}
    />
  ),
};

export const WithTermsAndConditions: Story = {
  render: () => (
    <KcPageStory
      kcContext={{
        termsAcceptanceRequired: true,
      }}
    />
  ),
};

export const WithTermsError: Story = {
  render: () => (
    <KcPageStory
      kcContext={{
        termsAcceptanceRequired: true,
        messagesPerField: {
          existsError: (fieldName: string) => {
            return fieldName === "termsAccepted";
          },
          get: (fieldName: string) => {
            if (fieldName === "termsAccepted") {
              return "You must accept the terms and conditions.";
            }
            return "";
          },
        },
      }}
    />
  ),
};

export const WithSocialProviders: Story = {
  render: () => (
    <KcPageStory
      kcContext={{
        social: {
          displayInfo: true,
          providers: SOCIAL_PROVIDERS,
        },
      }}
    />
  ),
};

export const WithOneSocialProvider: Story = {
  render: () => (
    <KcPageStory
      kcContext={{
        social: {
          displayInfo: true,
          providers: [SOCIAL_PROVIDERS[0]], // Just Google
        },
      }}
    />
  ),
};

export const WithRecaptcha: Story = {
  render: () => (
    <KcPageStory
      kcContext={{
        recaptchaRequired: true,
        recaptchaSiteKey: "6LeIxAcTAAAAAJcZVRqyHh71UMIEGNQ_MXjiZKhI",
        recaptchaAction: "register",
      }}
    />
  ),
};

export const WithGlobalError: Story = {
  render: () => (
    <KcPageStory
      kcContext={{
        message: {
          summary: "Registration failed. Please try again later.",
          type: "error",
        },
        messagesPerField: {
          existsError: (fieldName: string) => {
            return fieldName === "global";
          },
          get: (fieldName: string) => {
            if (fieldName === "global") {
              return "Registration failed. Please try again later.";
            }
            return "";
          },
        },
      }}
    />
  ),
};

export const CompleteScenario: Story = {
  render: () => (
    <KcPageStory
      kcContext={{
        termsAcceptanceRequired: true,
        social: {
          displayInfo: true,
          providers: SOCIAL_PROVIDERS.slice(0, 2),
        },
        register: {
          formData: {
            firstName: "",
            lastName: "",
            email: "",
            username: "",
          },
        },
      }}
    />
  ),
};
