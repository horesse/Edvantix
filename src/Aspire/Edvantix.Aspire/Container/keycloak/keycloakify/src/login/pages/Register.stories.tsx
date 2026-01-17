import type { Meta, StoryObj } from "@storybook/react-vite";
import { createKcPageStory } from "../KcPageStory";

const { KcPageStory } = createKcPageStory({ pageId: "register.ftl" });

// Common social providers to avoid duplication
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
