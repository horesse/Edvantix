import { kcSanitize } from "keycloakify/lib/kcSanitize";
import type { PageProps } from "keycloakify/login/pages/PageProps";
import { useState } from "react";
import type { I18n } from "../i18n";
import type { KcContext } from "../KcContext";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";

/** Иконка открытого глаза для поля пароля */
function EyeIcon() {
    return (
        <svg width="18" height="18" fill="none" viewBox="0 0 24 24" stroke="currentColor" aria-hidden="true">
            <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={1.8}
                d="M2.036 12.322a1.012 1.012 0 010-.639C3.423 7.51 7.36 4.5 12 4.5c4.638 0 8.573 3.007 9.963 7.178.07.207.07.431 0 .639C20.577 16.49 16.64 19.5 12 19.5c-4.638 0-8.573-3.007-9.963-7.178z"
            />
            <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={1.8}
                d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"
            />
        </svg>
    );
}

/** Иконка закрытого глаза для поля пароля */
function EyeOffIcon() {
    return (
        <svg width="18" height="18" fill="none" viewBox="0 0 24 24" stroke="currentColor" aria-hidden="true">
            <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={1.8}
                d="M3.98 8.223A10.477 10.477 0 001.934 12C3.226 16.338 7.244 19.5 12 19.5c.993 0 1.953-.138 2.863-.395M6.228 6.228A10.45 10.45 0 0112 4.5c4.756 0 8.773 3.162 10.065 7.498a10.523 10.523 0 01-4.293 5.774M6.228 6.228L3 3m3.228 3.228l3.65 3.65m7.894 7.894L21 21m-3.228-3.228l-3.65-3.65m0 0a3 3 0 10-4.243-4.243m4.242 4.242L9.88 9.88"
            />
        </svg>
    );
}

/** Иконка предупреждения для инлайн-ошибок полей */
function WarningIcon() {
    return (
        <svg width="12" height="12" fill="none" viewBox="0 0 24 24" stroke="currentColor" aria-hidden="true">
            <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M12 9v4m0 4h.01M10.29 3.86L1.82 18a2 2 0 001.71 3h16.94a2 2 0 001.71-3L13.71 3.86a2 2 0 00-3.42 0z"
            />
        </svg>
    );
}

/** Логотип Edvantix для мобильного header */
function LayersIcon({ size = 18 }: { size?: number }) {
    return (
        <svg width={size} height={size} fill="none" viewBox="0 0 24 24" aria-hidden="true">
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

export default function Login(
    props: Readonly<PageProps<Extract<KcContext, { pageId: "login.ftl" }>, I18n>>
) {
    const { kcContext, i18n, Template, classes } = props;

    const { social, realm, url, usernameHidden, login, auth, registrationDisabled, messagesPerField } =
        kcContext;

    const { msgStr } = i18n;

    const [isLoginButtonDisabled, setIsLoginButtonDisabled] = useState(false);
    const [isPasswordRevealed, setIsPasswordRevealed] = useState(false);

    const hasUsernameError = messagesPerField.existsError("username");
    const hasPasswordError = messagesPerField.existsError("password");
    const hasAnyError = messagesPerField.existsError("username", "password");

    // Метка поля username зависит от настроек realm
    const usernameLabel = !realm.loginWithEmailAllowed
        ? "Имя пользователя"
        : !realm.registrationEmailAsUsername
          ? "Имя пользователя или Email"
          : "Email";

    // Разделяем Google от прочих провайдеров для специального отображения
    const googleProvider = social?.providers?.find(
        (p) => p.alias === "google" || p.displayName?.toLowerCase().includes("google")
    );
    const otherProviders =
        social?.providers?.filter(
            (p) => p.alias !== "google" && !p.displayName?.toLowerCase().includes("google")
        ) ?? [];

    const hasSocialProviders =
        realm.password && social?.providers !== undefined && social.providers.length > 0;

    return (
        <Template
            kcContext={kcContext}
            i18n={i18n}
            doUseDefaultCss={false}
            classes={classes}
            displayMessage={!hasAnyError}
            headerNode={null}
            displayInfo={false}
        >
            {/* Мобильный логотип (скрыт на десктопе, т.к. там левая панель) */}
            <div className="lg:hidden flex items-center gap-2 mb-8">
                <div
                    className="w-8 h-8 rounded-lg flex items-center justify-center"
                    style={{ background: "#4f46e5" }}
                >
                    <LayersIcon size={18} />
                </div>
                <span className="font-bold text-base tracking-tight" style={{ color: "#0f172a" }}>
                    Edvantix
                </span>
            </div>

            {/* Заголовок */}
            <div>
                <h1
                    className="text-2xl font-bold tracking-tight"
                    style={{ color: "#0f172a" }}
                >
                    Добро пожаловать
                </h1>
                <p className="text-sm mt-1.5" style={{ color: "#64748b" }}>
                    Войдите в свою учётную запись
                </p>
            </div>

            {realm.password && (
                <form
                    className="mt-8 space-y-5"
                    onSubmit={() => {
                        setIsLoginButtonDisabled(true);
                        return true;
                    }}
                    action={url.loginAction}
                    method="post"
                >
                    {/* Поле Email / Имя пользователя */}
                    {!usernameHidden && (
                        <div>
                            <label
                                htmlFor="username"
                                className="block text-sm font-medium mb-1.5"
                                style={{ color: "#374151" }}
                            >
                                {usernameLabel}
                            </label>
                            <Input
                                id="username"
                                name="username"
                                type="text"
                                autoFocus
                                autoComplete="username"
                                defaultValue={login.username ?? ""}
                                placeholder="example@mail.ru"
                                hasError={hasUsernameError}
                                aria-invalid={hasUsernameError}
                            />
                            {hasUsernameError && (
                                <FieldError
                                    message={messagesPerField.getFirstError("username")}
                                />
                            )}
                        </div>
                    )}

                    {/* Поле Пароль */}
                    <div>
                        <label
                            htmlFor="password"
                            className="block text-sm font-medium mb-1.5"
                            style={{ color: "#374151" }}
                        >
                            Пароль
                        </label>
                        <div className="relative">
                            <Input
                                id="password"
                                name="password"
                                type={isPasswordRevealed ? "text" : "password"}
                                autoComplete="current-password"
                                placeholder="Введите пароль"
                                hasError={
                                    hasPasswordError || (usernameHidden && hasAnyError)
                                }
                                aria-invalid={hasPasswordError || (usernameHidden && hasAnyError)}
                                className="pr-11"
                            />
                            <button
                                type="button"
                                onClick={() => setIsPasswordRevealed((v) => !v)}
                                className="absolute right-3 top-1/2 -translate-y-1/2 transition-colors"
                                style={{ color: "#94a3b8" }}
                                aria-label={
                                    isPasswordRevealed
                                        ? msgStr("hidePassword")
                                        : msgStr("showPassword")
                                }
                                aria-controls="password"
                            >
                                {isPasswordRevealed ? <EyeOffIcon /> : <EyeIcon />}
                            </button>
                        </div>
                        {(hasPasswordError || (usernameHidden && hasAnyError)) && (
                            <FieldError
                                message={messagesPerField.getFirstError(
                                    usernameHidden ? "username" : "password",
                                    "password"
                                )}
                            />
                        )}
                    </div>

                    {/* Запомнить меня + Забыли пароль */}
                    <div className="flex items-center justify-between">
                        {realm.rememberMe && !usernameHidden && (
                            <label
                                htmlFor="rememberMe"
                                className="flex items-center gap-2 cursor-pointer select-none"
                            >
                                <input
                                    id="rememberMe"
                                    name="rememberMe"
                                    type="checkbox"
                                    defaultChecked={!!login.rememberMe}
                                    className="h-4 w-4 rounded cursor-pointer"
                                    style={{ accentColor: "#4f46e5" }}
                                />
                                <span className="text-sm" style={{ color: "#4b5563" }}>
                                    Запомнить меня
                                </span>
                            </label>
                        )}
                        {realm.resetPasswordAllowed && (
                            <a
                                href={url.loginResetCredentialsUrl}
                                className="text-sm font-medium transition-colors ml-auto"
                                style={{ color: "#4f46e5" }}
                            >
                                Забыли пароль?
                            </a>
                        )}
                    </div>

                    {/* Скрытый id учётной записи */}
                    <input
                        type="hidden"
                        name="credentialId"
                        value={auth.selectedCredential ?? ""}
                    />

                    {/* Кнопка входа */}
                    <Button type="submit" className="w-full" disabled={isLoginButtonDisabled}>
                        Войти в аккаунт
                    </Button>
                </form>
            )}

            {/* Секция социальных провайдеров */}
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

                    {/* Кнопка Google (специальный стиль с G-иконкой) */}
                    {googleProvider !== undefined && (
                        <a
                            id={`social-${googleProvider.alias}`}
                            href={googleProvider.loginUrl}
                            className="w-full flex items-center justify-center gap-2.5 py-2.5 px-4 rounded-lg border text-sm font-medium transition-all duration-150 bg-white hover:bg-slate-50"
                            style={{ borderColor: "#d1d5db", color: "#374151" }}
                        >
                            <GoogleIcon />
                            Продолжить с Google
                        </a>
                    )}

                    {/* Остальные социальные провайдеры */}
                    {otherProviders.map((provider) => (
                        <a
                            key={provider.alias}
                            id={`social-${provider.alias}`}
                            href={provider.loginUrl}
                            className="w-full flex items-center justify-center gap-2.5 py-2.5 px-4 rounded-lg border text-sm font-medium transition-all duration-150 bg-white hover:bg-slate-50"
                            style={{ borderColor: "#d1d5db", color: "#374151" }}
                        >
                            {provider.iconClasses !== undefined && provider.iconClasses !== "" && (
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

            {/* Ссылка на регистрацию */}
            {realm.password && realm.registrationAllowed && !registrationDisabled && (
                <p className="mt-6 text-center text-sm" style={{ color: "#64748b" }}>
                    Нет учётной записи?{" "}
                    <a
                        href={url.registrationUrl}
                        className="font-semibold transition-colors"
                        style={{ color: "#4f46e5" }}
                    >
                        Зарегистрироваться
                    </a>
                </p>
            )}
        </Template>
    );
}
