import { kcSanitize } from "keycloakify/lib/kcSanitize";
import type { PageProps } from "keycloakify/login/pages/PageProps";
import { useState, useCallback } from "react";
import type { I18n } from "../i18n";
import type { KcContext } from "../KcContext";
import { Button } from "@/components/ui/button";
import { Checkbox } from "@/components/ui/checkbox";
import { Separator } from "@/components/ui/separator";
import { FormInput, PasswordInput } from "@/components/ui/form-input";
import { AlertCircle } from "lucide-react";

export default function Login(
  props: Readonly<PageProps<Extract<KcContext, { pageId: "login.ftl" }>, I18n>>
) {
  const { kcContext, i18n, Template, classes } = props;

  const {
    social,
    realm,
    url,
    usernameHidden,
    login,
    auth,
    registrationDisabled,
    messagesPerField,
  } = kcContext;

  const { msg, msgStr } = i18n;

  const [isLoginButtonDisabled, setIsLoginButtonDisabled] = useState(false);
  const [formState, setFormState] = useState({
    username: {
      value: login.username ?? "",
      touched: false,
      error: null as string | null,
    },
    password: {
      value: "",
      touched: false,
      error: null as string | null,
    },
  });

  const validateUsername = useCallback(
    (value: string): string | null => {
      if (!value.trim()) {
        return "Обязательное поле";
      }
      if (realm.loginWithEmailAllowed && realm.registrationEmailAsUsername) {
        if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value)) {
          return "Некорректный формат email";
        }
      }
      return null;
    },
    [realm.loginWithEmailAllowed, realm.registrationEmailAsUsername]
  );

  const validatePassword = useCallback((value: string): string | null => {
    if (!value) {
      return "Обязательное поле";
    }
    return null;
  }, []);

  const handleUsernameChange = useCallback(
    (e: React.ChangeEvent<HTMLInputElement>) => {
      const value = e.target.value;
      setFormState(prev => ({
        ...prev,
        username: {
          value,
          touched: prev.username.touched,
          error: prev.username.touched ? validateUsername(value) : null,
        },
      }));
    },
    [validateUsername]
  );

  const handleUsernameBlur = useCallback(() => {
    setFormState(prev => ({
      ...prev,
      username: {
        ...prev.username,
        touched: true,
        error: validateUsername(prev.username.value),
      },
    }));
  }, [validateUsername]);

  const handlePasswordChange = useCallback(
    (e: React.ChangeEvent<HTMLInputElement>) => {
      const value = e.target.value;
      setFormState(prev => ({
        ...prev,
        password: {
          value,
          touched: prev.password.touched,
          error: prev.password.touched ? validatePassword(value) : null,
        },
      }));
    },
    [validatePassword]
  );

  const handlePasswordBlur = useCallback(() => {
    setFormState(prev => ({
      ...prev,
      password: {
        ...prev.password,
        touched: true,
        error: validatePassword(prev.password.value),
      },
    }));
  }, [validatePassword]);

  const getUsernameLabel = () => {
    if (!realm.loginWithEmailAllowed) {
      return msgStr("username");
    }
    if (!realm.registrationEmailAsUsername) {
      return msgStr("usernameOrEmail");
    }
    return msgStr("email");
  };

  const hasServerError = messagesPerField.existsError("username", "password");

  return (
    <Template
      kcContext={kcContext}
      headerNode={""}
      i18n={i18n}
      doUseDefaultCss={false}
      classes={classes}
      displayMessage={!hasServerError}
      displayInfo={
        realm.password && realm.registrationAllowed && !registrationDisabled
      }
      infoNode={
        <p className="text-muted-foreground text-center text-sm">
          {msg("noAccount")}{" "}
          <a
            href={url.registrationUrl}
            className="text-primary font-medium hover:underline focus:outline-none focus-visible:underline"
          >
            {msg("doRegister")}
          </a>
        </p>
      }
      socialProvidersNode={
        realm.password &&
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
              {social.providers.map(p => (
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
          {msg("loginAccountTitle")}
        </h1>
        <p className="text-muted-foreground">
          Войдите для доступа к платформе
        </p>
      </div>

      {realm.password && (
        <form
          id="kc-form-login"
          onSubmit={() => {
            setIsLoginButtonDisabled(true);
            return true;
          }}
          action={url.loginAction}
          method="post"
          className="space-y-5"
          noValidate
        >
          {hasServerError && (
            <div
              role="alert"
              className="flex items-start gap-3 p-4 rounded-xl bg-destructive/10 border border-destructive/20 text-destructive animate-in fade-in-0 slide-in-from-top-2 duration-300"
            >
              <AlertCircle
                className="h-5 w-5 mt-0.5 flex-shrink-0"
                aria-hidden="true"
              />
              <span
                className="text-sm"
                dangerouslySetInnerHTML={{
                  __html: kcSanitize(
                    messagesPerField.getFirstError("username", "password")
                  ),
                }}
              />
            </div>
          )}

          {!usernameHidden && (
            <FormInput
              label={getUsernameLabel()}
              name="username"
              type={realm.registrationEmailAsUsername ? "email" : "text"}
              required
              autoComplete="username"
              autoFocus
              value={formState.username.value}
              onChange={handleUsernameChange}
              onBlur={handleUsernameBlur}
              touched={formState.username.touched}
              error={hasServerError ? null : formState.username.error}
            />
          )}

          <PasswordInput
            label={msgStr("password")}
            name="password"
            required
            autoComplete="current-password"
            value={formState.password.value}
            onChange={handlePasswordChange}
            onBlur={handlePasswordBlur}
            touched={formState.password.touched}
            error={hasServerError ? null : formState.password.error}
            i18n={{
              showPassword: msgStr("showPassword"),
              hidePassword: msgStr("hidePassword"),
            }}
          />

          <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-3">
            {realm.rememberMe && !usernameHidden && (
              <div className="flex items-center space-x-2">
                <Checkbox
                  id="rememberMe"
                  name="rememberMe"
                  defaultChecked={!!login.rememberMe}
                  className="data-[state=checked]:bg-primary data-[state=checked]:border-primary"
                />
                <label
                  htmlFor="rememberMe"
                  className="text-sm font-medium leading-none cursor-pointer select-none text-muted-foreground hover:text-foreground transition-colors"
                >
                  {msg("rememberMe")}
                </label>
              </div>
            )}
            {realm.resetPasswordAllowed && (
              <a
                href={url.loginResetCredentialsUrl}
                className="text-sm text-primary hover:underline focus:outline-none focus-visible:underline sm:ml-auto"
              >
                {msg("doForgotPassword")}
              </a>
            )}
          </div>

          <input
            type="hidden"
            id="id-hidden-input"
            name="credentialId"
            value={auth.selectedCredential}
          />

          <Button
            disabled={isLoginButtonDisabled}
            className="w-full h-11 text-base font-medium"
            name="login"
            id="kc-login"
            type="submit"
          >
            {isLoginButtonDisabled ? (
              <span className="flex items-center gap-2">
                <span className="inline-block w-4 h-4 border-2 border-primary-foreground/30 border-t-primary-foreground rounded-full animate-spin" />
                {msgStr("doLogIn")}
              </span>
            ) : (
              msgStr("doLogIn")
            )}
          </Button>
        </form>
      )}
    </Template>
  );
}
