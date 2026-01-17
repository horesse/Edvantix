import { kcSanitize } from "keycloakify/lib/kcSanitize";
import type { PageProps } from "keycloakify/login/pages/PageProps";
import { useEffect, useReducer, useState } from "react";
import type { I18n } from "../i18n";
import type { KcContext } from "../KcContext";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Checkbox } from "@/components/ui/checkbox";
import { Separator } from "@/components/ui/separator";
import { Eye, EyeOff, GraduationCap, AlertCircle } from "lucide-react";
import {
  Card,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";

export default function Register(
  props: Readonly<
    PageProps<Extract<KcContext, { pageId: "register.ftl" }>, I18n>
  >
) {
  const { kcContext, i18n, Template, classes } = props;

  const { url, messagesPerField, realm } = kcContext;

  const {
    passwordRequired,
    recaptchaRequired,
    recaptchaSiteKey,
    recaptchaAction,
    termsAcceptanceRequired,
  } = kcContext as any;

  const social = (kcContext as any).social;
  const register = (kcContext as any).register || { formData: {} };

  const { msg, msgStr } = i18n;

  const [isFormSubmitting, setIsFormSubmitting] = useState(false);
  const [termsAccepted, setTermsAccepted] = useState(false);

  return (
    <Template
      kcContext={kcContext}
      headerNode={""}
      i18n={i18n}
      doUseDefaultCss={false}
      classes={classes}
      displayMessage={messagesPerField.exists("global")}
      displayRequiredFields={true}
      socialProvidersNode={
        <>
          {social?.providers !== undefined && social.providers.length !== 0 && (
              <>
                <div className="flex items-center gap-4">
                  <Separator className="flex-1" />
                  <p className="text-sm text-muted-foreground">
                    {msg("identity-provider-login-label")}
                  </p>
                  <Separator className="flex-1" />
                </div>

                <div className="space-y-2">
                  {social.providers.map((p: any) => (
                    <Button
                      key={p.alias}
                      variant="outline"
                      className="w-full"
                      asChild
                    >
                      <a id={`social-${p.alias}`} href={p.loginUrl}>
                        {p.iconClasses && (
                          <i
                            className={`${p.iconClasses} mr-2`}
                            aria-hidden="true"
                          />
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
              </>
            )}
        </>
      }
    >
      <Card className="w-full border-none shadow-md">
        <CardHeader className="gap-6">
          <div className="flex items-center gap-3">
            <div className="flex items-center justify-center w-12 h-12 rounded-lg bg-primary/10">
              <GraduationCap className="w-6 h-6 text-primary" />
            </div>
            <span className="text-2xl font-bold">Edvantix</span>
          </div>

          <div>
            <CardTitle className="mb-1.5 text-2xl">
              {msg("registerTitle")}
            </CardTitle>
            <CardDescription className="text-base">
              Создайте аккаунт для начала работы
            </CardDescription>
          </div>
        </CardHeader>

        <form
          id="kc-register-form"
          action={url.registrationAction}
          method="post"
          onSubmit={() => {
            setIsFormSubmitting(true);
            return true;
          }}
          className="px-6 pb-6 space-y-4"
        >
          {/* First Name & Last Name */}
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label htmlFor="firstName" className="flex items-center gap-1">
                {msg("firstName")}
                <span className="text-destructive">*</span>
              </Label>
              <Input
                id="firstName"
                name="firstName"
                defaultValue={register.formData.firstName ?? ""}
                type="text"
                autoComplete="given-name"
                aria-invalid={messagesPerField.existsError("firstName")}
                className={
                  messagesPerField.existsError("firstName")
                    ? "border-destructive focus-visible:ring-destructive"
                    : ""
                }
              />
              {messagesPerField.existsError("firstName") && (
                <div className="flex items-start gap-2 text-destructive">
                  <AlertCircle className="h-4 w-4 mt-0.5 flex-shrink-0" />
                  <span
                    className="text-sm"
                    dangerouslySetInnerHTML={{
                      __html: kcSanitize(
                        messagesPerField.getFirstError("firstName")
                      ),
                    }}
                  />
                </div>
              )}
            </div>

            <div className="space-y-2">
              <Label htmlFor="lastName" className="flex items-center gap-1">
                {msg("lastName")}
                <span className="text-destructive">*</span>
              </Label>
              <Input
                id="lastName"
                name="lastName"
                defaultValue={register.formData.lastName ?? ""}
                type="text"
                autoComplete="family-name"
                aria-invalid={messagesPerField.existsError("lastName")}
                className={
                  messagesPerField.existsError("lastName")
                    ? "border-destructive focus-visible:ring-destructive"
                    : ""
                }
              />
              {messagesPerField.existsError("lastName") && (
                <div className="flex items-start gap-2 text-destructive">
                  <AlertCircle className="h-4 w-4 mt-0.5 flex-shrink-0" />
                  <span
                    className="text-sm"
                    dangerouslySetInnerHTML={{
                      __html: kcSanitize(
                        messagesPerField.getFirstError("lastName")
                      ),
                    }}
                  />
                </div>
              )}
            </div>
          </div>

          {/* Email */}
          <div className="space-y-2">
            <Label htmlFor="email" className="flex items-center gap-1">
              {msg("email")}
              <span className="text-destructive">*</span>
            </Label>
            <Input
              id="email"
              name="email"
              defaultValue={register.formData.email ?? ""}
              type="email"
              autoComplete="email"
              aria-invalid={messagesPerField.existsError("email")}
              className={
                messagesPerField.existsError("email")
                  ? "border-destructive focus-visible:ring-destructive"
                  : ""
              }
            />
            {messagesPerField.existsError("email") && (
              <div className="flex items-start gap-2 text-destructive">
                <AlertCircle className="h-4 w-4 mt-0.5 flex-shrink-0" />
                <span
                  className="text-sm"
                  dangerouslySetInnerHTML={{
                    __html: kcSanitize(messagesPerField.getFirstError("email")),
                  }}
                />
              </div>
            )}
          </div>

          {/* Username (if email is not username) */}
          {!realm.registrationEmailAsUsername && (
            <div className="space-y-2">
              <Label htmlFor="username" className="flex items-center gap-1">
                {msg("username")}
                <span className="text-destructive">*</span>
              </Label>
              <Input
                id="username"
                name="username"
                defaultValue={register.formData.username ?? ""}
                type="text"
                autoComplete="username"
                aria-invalid={messagesPerField.existsError("username")}
                className={
                  messagesPerField.existsError("username")
                    ? "border-destructive focus-visible:ring-destructive"
                    : ""
                }
              />
              {messagesPerField.existsError("username") && (
                <div className="flex items-start gap-2 text-destructive">
                  <AlertCircle className="h-4 w-4 mt-0.5 flex-shrink-0" />
                  <span
                    className="text-sm"
                    dangerouslySetInnerHTML={{
                      __html: kcSanitize(
                        messagesPerField.getFirstError("username")
                      ),
                    }}
                  />
                </div>
              )}
            </div>
          )}

          {/* Password */}
          {passwordRequired && (
            <>
              <div className="space-y-2">
                <Label htmlFor="password" className="flex items-center gap-1">
                  {msg("password")}
                  <span className="text-destructive">*</span>
                </Label>
                <PasswordInput
                  i18n={i18n}
                  passwordInputId="password"
                  hasError={messagesPerField.existsError("password")}
                />
                {messagesPerField.existsError("password") && (
                  <div className="flex items-start gap-2 text-destructive">
                    <AlertCircle className="h-4 w-4 mt-0.5 flex-shrink-0" />
                    <span
                      className="text-sm"
                      dangerouslySetInnerHTML={{
                        __html: kcSanitize(
                          messagesPerField.getFirstError("password")
                        ),
                      }}
                    />
                  </div>
                )}
              </div>

              <div className="space-y-2">
                <Label
                  htmlFor="password-confirm"
                  className="flex items-center gap-1"
                >
                  {msg("passwordConfirm")}
                  <span className="text-destructive">*</span>
                </Label>
                <PasswordInput
                  i18n={i18n}
                  passwordInputId="password-confirm"
                  hasError={messagesPerField.existsError("password-confirm")}
                />
                {messagesPerField.existsError("password-confirm") && (
                  <div className="flex items-start gap-2 text-destructive">
                    <AlertCircle className="h-4 w-4 mt-0.5 flex-shrink-0" />
                    <span
                      className="text-sm"
                      dangerouslySetInnerHTML={{
                        __html: kcSanitize(
                          messagesPerField.getFirstError("password-confirm")
                        ),
                      }}
                    />
                  </div>
                )}
              </div>
            </>
          )}

          {/* Terms and Conditions */}
          {termsAcceptanceRequired && (
            <div className="space-y-2">
              <div className="flex items-start space-x-2">
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
                  className="text-sm leading-relaxed cursor-pointer select-none"
                >
                  {msg("termsText")}{" "}
                  <span className="text-destructive">*</span>
                </label>
              </div>
              {messagesPerField.existsError("termsAccepted") && (
                <div className="flex items-start gap-2 text-destructive">
                  <AlertCircle className="h-4 w-4 mt-0.5 flex-shrink-0" />
                  <span
                    className="text-sm"
                    dangerouslySetInnerHTML={{
                      __html: kcSanitize(
                        messagesPerField.getFirstError("termsAccepted")
                      ),
                    }}
                  />
                </div>
              )}
            </div>
          )}

          {/* reCAPTCHA */}
          {recaptchaRequired && (
            <div className="g-recaptcha" data-size="compact" data-sitekey={recaptchaSiteKey} data-action={recaptchaAction} />
          )}

          {/* Submit Button */}
          <Button
            disabled={isFormSubmitting}
            className="w-full"
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

          {/* Login link */}
          <div className="text-center">
            <p className="text-muted-foreground text-sm">
              Уже есть аккаунт?{" "}
              <a
                href={url.loginUrl}
                className="text-card-foreground hover:underline font-medium"
              >
                {msg("doLogIn")}
              </a>
            </p>
          </div>
        </form>
      </Card>
    </Template>
  );
}

function PasswordInput(
  props: Readonly<{
    i18n: I18n;
    passwordInputId: string;
    hasError: boolean;
  }>
) {
  const { i18n, passwordInputId, hasError } = props;
  const { msgStr } = i18n;

  const [isPasswordRevealed, toggleIsPasswordRevealed] = useReducer(
    (isPasswordRevealed: boolean) => !isPasswordRevealed,
    false
  );

  useEffect(() => {
    const passwordInputElement = document.getElementById(passwordInputId);
    if (passwordInputElement instanceof HTMLInputElement) {
      passwordInputElement.type = isPasswordRevealed ? "text" : "password";
    }
  }, [isPasswordRevealed, passwordInputId]);

  return (
    <div className="relative">
      <Input
        id={passwordInputId}
        name={passwordInputId}
        type="password"
        autoComplete={
          passwordInputId === "password" ? "new-password" : "new-password"
        }
        aria-invalid={hasError}
        className={`pr-10 ${hasError ? "border-destructive focus-visible:ring-destructive" : ""}`}
      />
      <Button
        type="button"
        variant="ghost"
        size="sm"
        className="absolute right-1 top-1/2 -translate-y-1/2 h-7 w-7 px-0"
        aria-label={msgStr(
          isPasswordRevealed ? "hidePassword" : "showPassword"
        )}
        aria-controls={passwordInputId}
        onClick={toggleIsPasswordRevealed}
      >
        {isPasswordRevealed ? (
          <EyeOff className="h-4 w-4" />
        ) : (
          <Eye className="h-4 w-4" />
        )}
      </Button>
    </div>
  );
}
