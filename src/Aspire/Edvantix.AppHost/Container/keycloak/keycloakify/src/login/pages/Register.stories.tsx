import type { Meta, StoryObj } from "@storybook/react-vite";
import { createKcPageStory } from "../KcPageStory";

const { KcPageStory } = createKcPageStory({ pageId: "register.ftl" });

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
            return fieldNames.has("email") || fieldNames.has("password");
          },
          get: (fieldName: string) => {
            if (fieldName === "email") {
              return "Invalid email format.";
            }
            if (fieldName === "password") {
              return "Password must be at least 8 characters.";
            }
            return "";
          },
          getFirstError: (fieldName: string) => {
            if (fieldName === "email") {
              return "Invalid email format.";
            }
            if (fieldName === "password") {
              return "Password must be at least 8 characters.";
            }
            return "";
          },
          exists: (fieldName: string) => {
            return fieldName === "email" || fieldName === "password";
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
          getFirstError: (fieldName: string) => {
            if (fieldName === "password-confirm") {
              return "Passwords do not match.";
            }
            return "";
          },
          exists: () => false,
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
          getFirstError: (fieldName: string) => {
            if (fieldName === "termsAccepted") {
              return "You must accept the terms and conditions.";
            }
            return "";
          },
          exists: () => false,
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
          getFirstError: () => "",
          exists: (fieldName: string) => fieldName === "global",
        },
      }}
    />
  ),
};

// Note: Social providers are conditionally rendered based on realm configuration
// and would need to be configured at the Keycloak level for the register page
