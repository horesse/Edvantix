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
import { Eye, EyeOff } from "lucide-react";
import { Card, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";

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
    const [isFocused, setIsFocused] = useState<string | null>(null);

    return (
        <Template
            kcContext={kcContext}
            i18n={i18n}
            doUseDefaultCss={false}
            classes={classes}
            displayMessage={!messagesPerField.existsError("username", "password")}
            displayInfo={
                realm.password && realm.registrationAllowed && !registrationDisabled
            }
            infoNode={
                <p className='text-muted-foreground text-center'>
                    {msg("noAccount")}{" "}
                    <a href={url.registrationUrl} className='text-card-foreground hover:underline'>
                        {msg("doRegister")}
                    </a>
                </p>
            }
            socialProvidersNode={
                <>
                    {realm.password &&
                        social?.providers !== undefined &&
                        social.providers.length !== 0 && (
                            <>
                                <div className='flex items-center gap-4'>
                                    <Separator className='flex-1' />
                                    <p>{msg("identity-provider-login-label")}</p>
                                    <Separator className='flex-1' />
                                </div>

                                {social.providers.map((p) => (
                                    <Button
                                        key={p.alias}
                                        variant="ghost"
                                        className="w-full"
                                        asChild
                                    >
                                        <a
                                            id={`social-${p.alias}`}
                                            href={p.loginUrl}
                                        >
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
                            </>
                        )}
                </>
            }
        >
            <Card className='w-full border-none shadow-md'>
                <CardHeader className='gap-6'>
                    <div>
                        <CardTitle className='mb-1.5 text-2xl'>{msg("loginAccountTitle")}</CardTitle>
                        <CardDescription className='text-base'>Менеджмент онлайн-школы</CardDescription>
                    </div>
                </CardHeader>

                {realm.password && (
                    <form
                        id="kc-form-login"
                        onSubmit={() => {
                            setIsLoginButtonDisabled(true);
                            return true;
                        }}
                        action={url.loginAction}
                        method="post"
                        className="px-6 pb-6 space-y-4"
                    >
                        {!usernameHidden && (
                            <div className="space-y-2">
                                <Label htmlFor="username">
                                    {(() => {
                                        if (!realm.loginWithEmailAllowed) {
                                            return msg("username");
                                        }
                                        if (!realm.registrationEmailAsUsername) {
                                            return msg("usernameOrEmail");
                                        }
                                        return msg("email");
                                    })()}
                                </Label>
                                <Input
                                    id="username"
                                    name="username"
                                    defaultValue={login.username ?? ""}
                                    type="text"
                                    autoFocus
                                    autoComplete="username"
                                    aria-invalid={messagesPerField.existsError(
                                        "username",
                                        "password"
                                    )}
                                />
                                {messagesPerField.existsError("username", "password") && (
                                    <p className="text-sm text-destructive">
                                        <span
                                            dangerouslySetInnerHTML={{
                                                __html: kcSanitize(
                                                    messagesPerField.getFirstError("username", "password")
                                                ),
                                            }}
                                        />
                                    </p>
                                )}
                            </div>
                        )}

                        <div className='space-y-2'>
                            <Label htmlFor="password">
                                {msg("password")}
                            </Label>
                            <PasswordInput
                                i18n={i18n}
                                passwordInputId="password"
                                hasError={messagesPerField.existsError("username", "password")}
                                isFocused={isFocused === "password"}
                                onFocus={() => setIsFocused("password")}
                                onBlur={() => setIsFocused(null)}
                            />
                            {usernameHidden &&
                                messagesPerField.existsError("username", "password") && (
                                    <p className="text-sm text-destructive">
                                        <span
                                            dangerouslySetInnerHTML={{
                                                __html: kcSanitize(
                                                    messagesPerField.getFirstError("username", "password")
                                                ),
                                            }}
                                        />
                                    </p>
                                )}
                        </div>

                        <div className="flex items-center justify-between">
                            {realm.rememberMe && !usernameHidden && (
                                <div className="flex items-center space-x-2">
                                    <Checkbox
                                        id="rememberMe"
                                        name="rememberMe"
                                        defaultChecked={!!login.rememberMe}
                                    />
                                    <label
                                        htmlFor="rememberMe"
                                        className="text-sm font-medium leading-none cursor-pointer select-none"
                                    >
                                        {msg("rememberMe")}
                                    </label>
                                </div>
                            )}
                            {realm.resetPasswordAllowed && (
                                <a
                                    href={url.loginResetCredentialsUrl}
                                    className="text-sm text-card-foreground hover:underline ml-auto"
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
                            className="w-full"
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
            </Card>
        </Template>
    );
}

function PasswordInput(props: Readonly<{
    i18n: I18n;
    passwordInputId: string;
    hasError: boolean;
    isFocused: boolean;
    onFocus: () => void;
    onBlur: () => void;
}>) {
    const { i18n, passwordInputId, hasError, onFocus, onBlur } = props;
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
                name="password"
                type="password"
                autoComplete="current-password"
                aria-invalid={hasError}
                onFocus={onFocus}
                onBlur={onBlur}
                className="pr-10"
            />
            <Button
                type="button"
                variant="ghost"
                size="sm"
                className="absolute right-1 top-1/2 -translate-y-1/2 h-7 w-7 px-0"
                aria-label={msgStr(isPasswordRevealed ? "hidePassword" : "showPassword")}
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