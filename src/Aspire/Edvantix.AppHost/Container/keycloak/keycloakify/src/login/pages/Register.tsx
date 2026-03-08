import { kcSanitize } from "keycloakify/lib/kcSanitize";
import type { PageProps } from "keycloakify/login/pages/PageProps";
import { useState, useCallback, useMemo } from "react";
import type { I18n } from "../i18n";
import type { KcContext } from "../KcContext";
import { Button } from "@/components/ui/button";
import { Checkbox } from "@/components/ui/checkbox";
import { Separator } from "@/components/ui/separator";
import { FormInput, PasswordInput } from "@/components/ui/form-input";
import { AlertCircle } from "lucide-react";

type FormField = {
  value: string;
  touched: boolean;
  error: string | null;
};

type FormState = {
  email: FormField;
  username: FormField;
  password: FormField;
  passwordConfirm: FormField;
};

export default function Register(
  props: Readonly<
    PageProps<Extract<KcContext, { pageId: "register.ftl" }>, I18n>
  >,
) {
  const { kcContext, i18n, Template, classes } = props;

  const { url, messagesPerField, realm } = kcContext;

  const {
    passwordRequired,
    recaptchaRequired,
    recaptchaSiteKey,
    recaptchaAction,
    termsAcceptanceRequired,
  } = kcContext as {
    passwordRequired?: boolean;
    recaptchaRequired?: boolean;
    recaptchaSiteKey?: string;
    recaptchaAction?: string;
    termsAcceptanceRequired?: boolean;
  };

  const social = (
    kcContext as {
      social?: {
        providers?: Array<{
          alias: string;
          loginUrl: string;
          displayName: string;
          iconClasses?: string;
        }>;
      };
    }
  ).social;
  const register = (
    kcContext as {
      register?: { formData: { email?: string; username?: string } };
    }
  ).register ?? { formData: {} };

  const { msg, msgStr } = i18n;

  const [isFormSubmitting, setIsFormSubmitting] = useState(false);
  const [termsAccepted, setTermsAccepted] = useState(false);

  const [formState, setFormState] = useState<FormState>({
    email: {
      value: register.formData.email ?? "",
      touched: false,
      error: null,
    },
    username: {
      value: register.formData.username ?? "",
      touched: false,
      error: null,
    },
    password: {
      value: "",
      touched: false,
      error: null,
    },
    passwordConfirm: {
      value: "",
      touched: false,
      error: null,
    },
  });

  const validateEmail = useCallback((value: string): string | null => {
    if (!value.trim()) {
      return "Обязательное поле";
    }
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value)) {
      return "Некорректный формат email";
    }
    return null;
  }, []);

  const validateUsername = useCallback((value: string): string | null => {
    if (!value.trim()) {
      return "Обязательное поле";
    }
    if (value.length < 3) {
      return "Минимум 3 символа";
    }
    return null;
  }, []);

  const validatePassword = useCallback((value: string): string | null => {
    if (!value) {
      return "Обязательное поле";
    }
    if (value.length < 8) {
      return "Минимум 8 символов";
    }
    return null;
  }, []);

  const validatePasswordConfirm = useCallback(
    (value: string, password: string): string | null => {
      if (!value) {
        return "Обязательное поле";
      }
      if (value !== password) {
        return "Пароли не совпадают";
      }
      return null;
    },
    [],
  );

  const createFieldHandler = useCallback(
    <K extends keyof FormState>(
      fieldName: K,
      validator: (value: string, ...args: string[]) => string | null,
      ...validatorArgs: string[]
    ) => ({
      onChange: (e: React.ChangeEvent<HTMLInputElement>) => {
        const value = e.target.value;
        setFormState((prev) => ({
          ...prev,
          [fieldName]: {
            value,
            touched: prev[fieldName].touched,
            error: prev[fieldName].touched
              ? validator(value, ...validatorArgs)
              : null,
          },
        }));
      },
      onBlur: () => {
        setFormState((prev) => ({
          ...prev,
          [fieldName]: {
            ...prev[fieldName],
            touched: true,
            error: validator(prev[fieldName].value, ...validatorArgs),
          },
        }));
      },
    }),
    [],
  );

  const emailHandlers = useMemo(
    () => createFieldHandler("email", validateEmail),
    [createFieldHandler, validateEmail],
  );

  const usernameHandlers = useMemo(
    () => createFieldHandler("username", validateUsername),
    [createFieldHandler, validateUsername],
  );

  const passwordHandlers = useMemo(
    () => ({
      onChange: (e: React.ChangeEvent<HTMLInputElement>) => {
        const value = e.target.value;
        setFormState((prev) => {
          const passwordError = prev.password.touched
            ? validatePassword(value)
            : null;
          const confirmError =
            prev.passwordConfirm.touched && prev.passwordConfirm.value
              ? validatePasswordConfirm(prev.passwordConfirm.value, value)
              : null;
          return {
            ...prev,
            password: {
              value,
              touched: prev.password.touched,
              error: passwordError,
            },
            passwordConfirm: {
              ...prev.passwordConfirm,
              error: confirmError,
            },
          };
        });
      },
      onBlur: () => {
        setFormState((prev) => ({
          ...prev,
          password: {
            ...prev.password,
            touched: true,
            error: validatePassword(prev.password.value),
          },
        }));
      },
    }),
    [validatePassword, validatePasswordConfirm],
  );

  const passwordConfirmHandlers = useMemo(
    () => ({
      onChange: (e: React.ChangeEvent<HTMLInputElement>) => {
        const value = e.target.value;
        setFormState((prev) => ({
          ...prev,
          passwordConfirm: {
            value,
            touched: prev.passwordConfirm.touched,
            error: prev.passwordConfirm.touched
              ? validatePasswordConfirm(value, prev.password.value)
              : null,
          },
        }));
      },
      onBlur: () => {
        setFormState((prev) => ({
          ...prev,
          passwordConfirm: {
            ...prev.passwordConfirm,
            touched: true,
            error: validatePasswordConfirm(
              prev.passwordConfirm.value,
              prev.password.value,
            ),
          },
        }));
      },
    }),
    [validatePasswordConfirm],
  );

  const hasGlobalError = messagesPerField.exists("global");

  return (
    <Template
      kcContext={kcContext}
      headerNode={""}
      i18n={i18n}
      doUseDefaultCss={false}
      classes={classes}
      displayMessage={hasGlobalError}
      displayRequiredFields={false}
      socialProvidersNode={
        social?.providers !== undefined &&
        social.providers.length !== 0 && (
          <div className="space-y-4">
            <div className="flex items-center gap-4">
              <Separator className="flex-1" />
              <p className="text-sm text-muted-foreground whitespace-nowrap">
                {msg("identity-provider-login-label")}
              </p>
              <Separator className="flex-1" />
            </div>

            <div className="grid gap-2">
              {social.providers.map((p) => (
                <Button
                  key={p.alias}
                  variant="outline"
                  className="w-full h-10 gap-2"
                  asChild
                >
                  <a id={`social-${p.alias}`} href={p.loginUrl}>
                    {p.iconClasses && (
                      <i className={p.iconClasses} aria-hidden="true" />
                    )}
                    <span
                      dangerouslySetInnerHTML={{
                        __html: kcSanitize(p.displayName),
                      }}
                    />
                  </a>
                </Button>
              ))}
            </div>
          </div>
        )
      }
    >
      {/* Header */}
      <div className="space-y-2 mb-6">
        <h1 className="text-2xl sm:text-3xl font-bold tracking-tight">
          {msg("registerTitle")}
        </h1>
        <p className="text-muted-foreground">
          Создайте аккаунт для начала работы
        </p>
      </div>

      <form
        id="kc-register-form"
        action={url.registrationAction}
        method="post"
        onSubmit={() => {
          setIsFormSubmitting(true);
          return true;
        }}
        className="space-y-5"
        noValidate
      >
        <FormInput
          label={msgStr("email")}
          name="email"
          type="email"
          required
          autoComplete="email"
          autoFocus
          value={formState.email.value}
          onChange={emailHandlers.onChange}
          onBlur={emailHandlers.onBlur}
          touched={formState.email.touched}
          error={
            messagesPerField.existsError("email")
              ? kcSanitize(messagesPerField.getFirstError("email"))
              : formState.email.error
          }
        />

        {!realm.registrationEmailAsUsername && (
          <FormInput
            label={msgStr("username")}
            name="username"
            type="text"
            required
            autoComplete="username"
            value={formState.username.value}
            onChange={usernameHandlers.onChange}
            onBlur={usernameHandlers.onBlur}
            touched={formState.username.touched}
            error={
              messagesPerField.existsError("username")
                ? kcSanitize(messagesPerField.getFirstError("username"))
                : formState.username.error
            }
          />
        )}

        {passwordRequired && (
          <>
            <PasswordInput
              label={msgStr("password")}
              name="password"
              required
              autoComplete="new-password"
              value={formState.password.value}
              onChange={passwordHandlers.onChange}
              onBlur={passwordHandlers.onBlur}
              touched={formState.password.touched}
              error={
                messagesPerField.existsError("password")
                  ? kcSanitize(messagesPerField.getFirstError("password"))
                  : formState.password.error
              }
              showStrengthIndicator
              showRequirements
              i18n={{
                showPassword: msgStr("showPassword"),
                hidePassword: msgStr("hidePassword"),
                passwordStrength: {
                  weak: "Слабый",
                  fair: "Средний",
                  good: "Хороший",
                  strong: "Надежный",
                },
                requirements: {
                  minLength: "Минимум 8 символов",
                  uppercase: "Одна заглавная буква",
                  lowercase: "Одна строчная буква",
                  number: "Одна цифра",
                  special: "Один спецсимвол",
                },
              }}
            />

            <PasswordInput
              label={msgStr("passwordConfirm")}
              name="password-confirm"
              required
              autoComplete="new-password"
              value={formState.passwordConfirm.value}
              onChange={passwordConfirmHandlers.onChange}
              onBlur={passwordConfirmHandlers.onBlur}
              touched={formState.passwordConfirm.touched}
              error={
                messagesPerField.existsError("password-confirm")
                  ? kcSanitize(
                      messagesPerField.getFirstError("password-confirm"),
                    )
                  : formState.passwordConfirm.error
              }
              i18n={{
                showPassword: msgStr("showPassword"),
                hidePassword: msgStr("hidePassword"),
              }}
            />
          </>
        )}

        {termsAcceptanceRequired && (
          <div className="space-y-2">
            <div className="flex items-start space-x-3">
              <Checkbox
                id="termsAccepted"
                name="termsAccepted"
                checked={termsAccepted}
                onCheckedChange={(checked) =>
                  setTermsAccepted(checked === true)
                }
                aria-invalid={messagesPerField.existsError("termsAccepted")}
                className={
                  messagesPerField.existsError("termsAccepted")
                    ? "border-destructive"
                    : ""
                }
              />
              <label
                htmlFor="termsAccepted"
                className="text-sm leading-relaxed cursor-pointer select-none text-muted-foreground"
              >
                {msg("termsText")}{" "}
                <span className="text-destructive" aria-hidden="true">
                  *
                </span>
              </label>
            </div>
            {messagesPerField.existsError("termsAccepted") && (
              <div
                role="alert"
                className="flex items-start gap-2 text-destructive animate-in fade-in-0 slide-in-from-top-1 duration-200"
              >
                <AlertCircle
                  className="h-4 w-4 mt-0.5 flex-shrink-0"
                  aria-hidden="true"
                />
                <span
                  className="text-sm"
                  dangerouslySetInnerHTML={{
                    __html: kcSanitize(
                      messagesPerField.getFirstError("termsAccepted"),
                    ),
                  }}
                />
              </div>
            )}
          </div>
        )}

        {recaptchaRequired && recaptchaSiteKey && (
          <div
            className="g-recaptcha flex justify-center"
            data-size="normal"
            data-sitekey={recaptchaSiteKey}
            data-action={recaptchaAction}
          />
        )}

        <Button
          disabled={isFormSubmitting}
          className="w-full h-11 text-base font-medium"
          type="submit"
          id="kc-register"
        >
          {isFormSubmitting ? (
            <span className="flex items-center gap-2">
              <span className="inline-block w-4 h-4 border-2 border-primary-foreground/30 border-t-primary-foreground rounded-full animate-spin" />
              {msgStr("doRegister")}
            </span>
          ) : (
            msgStr("doRegister")
          )}
        </Button>

        <div className="text-center">
          <p className="text-muted-foreground text-sm">
            Уже есть аккаунт?{" "}
            <a
              href={url.loginUrl}
              className="text-primary font-medium hover:underline focus:outline-none focus-visible:underline"
            >
              {msg("doLogIn")}
            </a>
          </p>
        </div>
      </form>
    </Template>
  );
}
