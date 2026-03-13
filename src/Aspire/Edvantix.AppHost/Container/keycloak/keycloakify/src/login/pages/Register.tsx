import { kcSanitize } from "keycloakify/lib/kcSanitize";
import type { PageProps } from "keycloakify/login/pages/PageProps";
import { useState, useCallback, useMemo } from "react";
import type { I18n } from "../i18n";
import type { KcContext } from "../KcContext";
import { Button } from "@/components/ui/button";
import { FormInput, PasswordInput } from "@/components/ui/form-input";

/** Иконка Google для кнопки социального входа */
function GoogleIcon() {
  return (
    <svg width="18" height="18" viewBox="0 0 24 24" aria-hidden="true">
      <path
        fill="#4285F4"
        d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z"
      />
      <path
        fill="#34A853"
        d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z"
      />
      <path
        fill="#FBBC05"
        d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l3.66-2.84z"
      />
      <path
        fill="#EA4335"
        d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z"
      />
    </svg>
  );
}

/** Логотип Edvantix для мобильного header */
function LayersIcon() {
  return (
    <svg
      width="18"
      height="18"
      fill="none"
      viewBox="0 0 24 24"
      aria-hidden="true"
    >
      <path
        d="M12 2L2 7l10 5 10-5-10-5zM2 17l10 5 10-5M2 12l10 5 10-5"
        stroke="white"
        strokeWidth={2}
        strokeLinecap="round"
        strokeLinejoin="round"
      />
    </svg>
  );
}

/** Иконка предупреждения для инлайн-ошибок */
function WarningIcon() {
  return (
    <svg
      width="12"
      height="12"
      fill="none"
      viewBox="0 0 24 24"
      stroke="currentColor"
      aria-hidden="true"
    >
      <path
        strokeLinecap="round"
        strokeLinejoin="round"
        strokeWidth={2}
        d="M12 9v4m0 4h.01M10.29 3.86L1.82 18a2 2 0 001.71 3h16.94a2 2 0 001.71-3L13.71 3.86a2 2 0 00-3.42 0z"
      />
    </svg>
  );
}

/** Инлайн-сообщение об ошибке под полем */
function FieldError({ message }: { message: string }) {
  return (
    <p
      className="mt-1.5 flex items-center gap-1 text-xs"
      style={{ color: "#ef4444" }}
      aria-live="polite"
    >
      <WarningIcon />
      <span dangerouslySetInnerHTML={{ __html: kcSanitize(message) }} />
    </p>
  );
}

type FieldState = { value: string; touched: boolean; error: string | null };
type FormState = {
  email: FieldState;
  username: FieldState;
  password: FieldState;
  passwordConfirm: FieldState;
};

export default function Register(
  props: Readonly<
    PageProps<Extract<KcContext, { pageId: "register.ftl" }>, I18n>
  >,
) {
  const { kcContext, i18n, Template, classes } = props;

  const { url, messagesPerField, realm } = kcContext;
  const { msgStr } = i18n;

  // Расширенные поля контекста регистрации
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

  const [isFormSubmitting, setIsFormSubmitting] = useState(false);

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
    password: { value: "", touched: false, error: null },
    passwordConfirm: { value: "", touched: false, error: null },
  });

  // Валидаторы полей
  const validateEmail = useCallback((value: string): string | null => {
    if (!value.trim()) return "Обязательное поле";
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value))
      return "Некорректный формат email";
    return null;
  }, []);

  const validateUsername = useCallback((value: string): string | null => {
    if (!value.trim()) return "Обязательное поле";
    if (value.length < 3) return "Минимум 3 символа";
    return null;
  }, []);

  const validatePassword = useCallback((value: string): string | null => {
    if (!value) return "Обязательное поле";
    if (value.length < 8) return "Минимум 8 символов";
    return null;
  }, []);

  const validatePasswordConfirm = useCallback(
    (value: string, password: string): string | null => {
      if (!value) return "Обязательное поле";
      if (value !== password) return "Пароли не совпадают";
      return null;
    },
    [],
  );

  // Обработчики изменений для простых полей
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

  // Обработчик пароля обновляет и поле подтверждения при изменении
  const passwordHandlers = useMemo(
    () => ({
      onChange: (e: React.ChangeEvent<HTMLInputElement>) => {
        const value = e.target.value;
        setFormState((prev) => ({
          ...prev,
          password: {
            value,
            touched: prev.password.touched,
            error: prev.password.touched ? validatePassword(value) : null,
          },
          passwordConfirm: {
            ...prev.passwordConfirm,
            error:
              prev.passwordConfirm.touched && prev.passwordConfirm.value
                ? validatePasswordConfirm(prev.passwordConfirm.value, value)
                : null,
          },
        }));
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

  // Разделяем Google от прочих провайдеров
  const googleProvider = social?.providers?.find(
    (p) =>
      p.alias === "google" || p.displayName?.toLowerCase().includes("google"),
  );
  const otherProviders =
    social?.providers?.filter(
      (p) =>
        p.alias !== "google" &&
        !p.displayName?.toLowerCase().includes("google"),
    ) ?? [];

  const hasSocialProviders =
    social?.providers !== undefined && social.providers.length > 0;

  return (
    <Template
      kcContext={kcContext}
      i18n={i18n}
      doUseDefaultCss={false}
      classes={classes}
      displayMessage={hasGlobalError}
      headerNode={null}
      displayInfo={false}
    >
      {/* Мобильный логотип */}
      <div className="lg:hidden flex items-center gap-2 mb-8">
        <div
          className="w-8 h-8 rounded-lg flex items-center justify-center"
          style={{ background: "#4f46e5" }}
        >
          <LayersIcon />
        </div>
        <span
          className="font-bold text-base tracking-tight"
          style={{ color: "#0f172a" }}
        >
          Edvantix
        </span>
      </div>

      {/* Заголовок */}
      <div>
        <h1
          className="text-2xl font-bold tracking-tight"
          style={{ color: "#0f172a" }}
        >
          Создать аккаунт
        </h1>
        <p className="text-sm mt-1.5" style={{ color: "#64748b" }}>
          Заполните данные для регистрации
        </p>
      </div>

      <form
        id="kc-register-form"
        action={url.registrationAction}
        method="post"
        className="mt-8 space-y-5"
        noValidate
        onSubmit={() => {
          setIsFormSubmitting(true);
          return true;
        }}
      >
        {/* Поле Email */}
        <FormInput
          label={msgStr("email")}
          name="email"
          type="email"
          required
          autoComplete="email"
          autoFocus
          placeholder="example@mail.ru"
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

        {/* Поле Username (если email не используется как логин) */}
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

        {/* Поля пароля */}
        {passwordRequired && (
          <>
            <PasswordInput
              label={msgStr("password")}
              name="password"
              required
              autoComplete="new-password"
              placeholder="Минимум 8 символов"
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
              i18n={{
                showPassword: msgStr("showPassword"),
                hidePassword: msgStr("hidePassword"),
                passwordStrength: {
                  weak: "Слабый",
                  fair: "Средний",
                  good: "Хороший",
                  strong: "Надёжный",
                },
              }}
            />

            <PasswordInput
              label={msgStr("passwordConfirm")}
              name="password-confirm"
              required
              autoComplete="new-password"
              placeholder="Повторите пароль"
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

        {/* Чекбокс принятия условий.
                    Используем нативный <input type="checkbox">, так как Radix Checkbox
                    не передаёт значение в POST-форму Keycloak. */}
        {termsAcceptanceRequired && (
          <div className="space-y-1.5">
            <label className="flex items-start gap-2.5 cursor-pointer select-none">
              <input
                id="termsAccepted"
                name="termsAccepted"
                type="checkbox"
                className="mt-0.5 h-4 w-4 shrink-0 rounded cursor-pointer"
                style={{ accentColor: "#4f46e5" }}
                aria-invalid={messagesPerField.existsError("termsAccepted")}
              />
              <span
                className="text-sm leading-relaxed"
                style={{ color: "#4b5563" }}
              >
                Я принимаю{" "}
                <a
                  href={url.loginAction}
                  className="font-medium"
                  style={{ color: "#4f46e5" }}
                >
                  Условия использования
                </a>{" "}
                и{" "}
                <a
                  href={url.loginAction}
                  className="font-medium"
                  style={{ color: "#4f46e5" }}
                >
                  Политику конфиденциальности
                </a>
              </span>
            </label>
            {messagesPerField.existsError("termsAccepted") && (
              <FieldError
                message={messagesPerField.getFirstError("termsAccepted")}
              />
            )}
          </div>
        )}

        {/* reCAPTCHA */}
        {recaptchaRequired && recaptchaSiteKey !== undefined && (
          <div
            className="g-recaptcha flex justify-center"
            data-size="normal"
            data-sitekey={recaptchaSiteKey}
            data-action={recaptchaAction}
          />
        )}

        {/* Кнопка создания аккаунта */}
        <Button type="submit" className="w-full" disabled={isFormSubmitting}>
          {isFormSubmitting ? (
            <span className="flex items-center gap-2">
              <span className="inline-block w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin" />
              Создание аккаунта...
            </span>
          ) : (
            "Создать аккаунт"
          )}
        </Button>
      </form>

      {/* Социальные провайдеры */}
      {hasSocialProviders && (
        <div className="mt-5 space-y-3">
          {/* Разделитель «или» */}
          <div className="relative">
            <div className="absolute inset-0 flex items-center">
              <div
                className="w-full border-t"
                style={{ borderColor: "#e2e8f0" }}
              />
            </div>
            <div className="relative flex justify-center">
              <span
                className="px-3 text-xs font-medium"
                style={{ background: "#f8fafc", color: "#94a3b8" }}
              >
                или
              </span>
            </div>
          </div>

          {/* Google */}
          {googleProvider !== undefined && (
            <a
              id={`social-${googleProvider.alias}`}
              href={googleProvider.loginUrl}
              className="w-full flex items-center justify-center gap-2.5 py-2.5 px-4 rounded-lg border text-sm font-medium transition-all duration-150 bg-white hover:bg-slate-50"
              style={{ borderColor: "#d1d5db", color: "#374151" }}
            >
              <GoogleIcon />
              Зарегистрироваться через Google
            </a>
          )}

          {/* Прочие провайдеры */}
          {otherProviders.map((provider) => (
            <a
              key={provider.alias}
              id={`social-${provider.alias}`}
              href={provider.loginUrl}
              className="w-full flex items-center justify-center gap-2.5 py-2.5 px-4 rounded-lg border text-sm font-medium transition-all duration-150 bg-white hover:bg-slate-50"
              style={{ borderColor: "#d1d5db", color: "#374151" }}
            >
              {provider.iconClasses !== undefined &&
                provider.iconClasses !== "" && (
                  <i className={provider.iconClasses} aria-hidden="true" />
                )}
              <span
                dangerouslySetInnerHTML={{
                  __html: kcSanitize(provider.displayName),
                }}
              />
            </a>
          ))}
        </div>
      )}

      {/* Ссылка на вход */}
      <p className="mt-6 text-center text-sm" style={{ color: "#64748b" }}>
        Уже есть аккаунт?{" "}
        <a
          href={url.loginUrl}
          className="font-semibold transition-colors"
          style={{ color: "#4f46e5" }}
        >
          Войти
        </a>
      </p>
    </Template>
  );
}
