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
import {Card, CardDescription, CardHeader, CardTitle} from "@/components/ui/card.tsx";

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
            headerNode={
                <Card className='z-1 w-full border-none shadow-md sm:max-w-lg'>
                    <CardHeader className='gap-6'>
                        <div>
                            <CardTitle className='mb-1.5 text-2xl'>{msg("loginAccountTitle")}</CardTitle>
                            <CardDescription className='text-base'>Менеджмент онлайн-школы</CardDescription>
                        </div>
                    </CardHeader>
                </Card>
            }
            displayInfo={
                realm.password && realm.registrationAllowed && !registrationDisabled
            }
            infoNode={
                <div className="text-center mt-8 animate-in fade-in slide-in-from-bottom-4 duration-700 delay-150">
                    <p className="text-sm text-muted-foreground">
                        {msg("noAccount")}{" "}
                        <a
                            href={url.registrationUrl}
                            className="font-semibold text-primary underline-offset-4 hover:underline transition-all hover:text-primary/80 inline-flex items-center gap-1 group"
                        >
                            {msg("doRegister")}
                            <span className="inline-block transition-transform group-hover:translate-x-0.5">→</span>
                        </a>
                    </p>
                </div>
            }
            socialProvidersNode={
                <>
                    {realm.password &&
                        social?.providers !== undefined &&
                        social.providers.length !== 0 && (
                            <div className="space-y-6 animate-in fade-in slide-in-from-bottom-4 duration-700 delay-100">
                                <div className="relative">
                                    <div className="absolute inset-0 flex items-center">
                                        <Separator className="w-full" />
                                    </div>
                                    <div className="relative flex justify-center text-xs uppercase">
                    <span className="bg-background px-3 py-1 text-muted-foreground font-medium tracking-wider">
                      {msg("identity-provider-login-label")}
                    </span>
                                    </div>
                                </div>

                                <div className={`grid gap-3 ${social.providers.length > 3 ? 'grid-cols-2' : 'grid-cols-1'}`}>
                                    {social.providers.map((p) => (
                                        <Button
                                            key={p.alias}
                                            variant="outline"
                                            className="w-full h-12 border-2 hover:border-primary/50 hover:bg-primary/5 transition-all duration-300 hover:shadow-lg hover:shadow-primary/10 hover:-translate-y-0.5 group"
                                            asChild
                                        >
                                            <a
                                                id={`social-${p.alias}`}
                                                href={p.loginUrl}
                                            >
                                                {p.iconClasses && (
                                                    <i
                                                        className={`${p.iconClasses} mr-2 text-lg transition-transform group-hover:scale-110`}
                                                        aria-hidden="true"
                                                    />
                                                )}
                                                <span
                                                    className="font-medium"
                                                    dangerouslySetInnerHTML={{
                                                        __html: kcSanitize(p.displayName),
                                                    }}
                                                />
                                            </a>
                                        </Button>
                                    ))}
                                </div>
                            </div>
                        )}
                </>
            }
        >
            {realm.password && (
                <form
                    id="kc-form-login"
                    onSubmit={() => {
                        setIsLoginButtonDisabled(true);
                        return true;
                    }}
                    action={url.loginAction}
                    method="post"
                    className="space-y-4"
                >
                    {!usernameHidden && (
                        <div className="space-y-2 group">
                            <Label
                                htmlFor="username"
                                className="leading-5"
                            >
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
                            <div className="relative">
                                <Input
                                    id="username"
                                    name="username"
                                    defaultValue={login.username ?? ""}
                                    type="text"
                                    autoFocus
                                    autoComplete="username"
                                    onFocus={() => setIsFocused("username")}
                                    onBlur={() => setIsFocused(null)}
                                    aria-invalid={messagesPerField.existsError(
                                        "username",
                                        "password"
                                    )}
                                />
                            </div>
                            {messagesPerField.existsError("username", "password") && (
                                <p
                                    className="text-sm font-medium text-destructive animate-in fade-in slide-in-from-top-1 duration-300 flex items-center gap-1.5"
                                    aria-live="polite"
                                >
                                    <span className="inline-block w-1 h-1 rounded-full bg-destructive animate-pulse" />
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

                    <div className='w-full space-y-1'>
                        <Label
                            htmlFor="password"
                        >
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
                                <p
                                    className="text-destructive animate-in fade-in slide-in-from-top-1 duration-300 flex items-center gap-1.5"
                                    aria-live="polite"
                                >
                                    <span className="inline-block w-1 h-1 rounded-full bg-destructive animate-pulse" />
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

                    <div className="flex items-center justify-between pt-2">
                        <div className="flex items-center space-x-2">
                            {realm.rememberMe && !usernameHidden && (
                                <>
                                    <Checkbox
                                        id="rememberMe"
                                        name="rememberMe"
                                        defaultChecked={!!login.rememberMe}
                                        className="transition-all duration-300 data-[state=checked]:bg-primary data-[state=checked]:scale-110"
                                    />
                                    <label
                                        htmlFor="rememberMe"
                                        className="text-sm font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70 cursor-pointer select-none transition-colors hover:text-foreground/80"
                                    >
                                        {msg("rememberMe")}
                                    </label>
                                </>
                            )}
                        </div>
                        {realm.resetPasswordAllowed && (
                            <a
                                href={url.loginResetCredentialsUrl}
                                className="text-sm font-semibold text-primary underline-offset-4 hover:underline transition-all hover:text-primary/80 inline-flex items-center gap-1 group"
                            >
                                {msg("doForgotPassword")}
                                <span className="inline-block transition-transform group-hover:translate-x-0.5">→</span>
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
                        className="w-full text-base shadow-lg shadow-primary/20 hover:shadow-xl hover:shadow-primary/30 transition-all duration-300 hover:scale-[1.02] active:scale-[0.98] disabled:opacity-50 disabled:cursor-not-allowed disabled:hover:scale-100"
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

function PasswordInput(props: Readonly<{
    i18n: I18n;
    passwordInputId: string;
    hasError: boolean;
    isFocused: boolean;
    onFocus: () => void;
    onBlur: () => void;
}>) {
    const { i18n, passwordInputId, hasError, isFocused, onFocus, onBlur } = props;
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
                className={`pr-12 transition-all duration-300`}
            />
            {isFocused && !hasError && (
                <div className="absolute -bottom-1 left-0 right-0 h-0.5 bg-gradient-to-r from-transparent via-primary to-transparent animate-in fade-in duration-300" />
            )}
            <Button
                type="button"
                variant="ghost"
                size="sm"
                className="absolute right-1 top-1 h-7 w-10 px-0 hover:bg-primary/10 transition-all duration-300 group rounded-md"
                aria-label={msgStr(isPasswordRevealed ? "hidePassword" : "showPassword")}
                aria-controls={passwordInputId}
                onClick={toggleIsPasswordRevealed}
            >
                {isPasswordRevealed ? (
                    <EyeOff className="h-4 w-4 text-muted-foreground transition-all duration-300 group-hover:text-primary group-hover:scale-110" />
                ) : (
                    <Eye className="h-4 w-4 text-muted-foreground transition-all duration-300 group-hover:text-primary group-hover:scale-110" />
                )}
            </Button>
        </div>
    );
}