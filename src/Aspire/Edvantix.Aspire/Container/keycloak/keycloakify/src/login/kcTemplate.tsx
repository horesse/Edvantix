import { useEffect } from "react";
import { clsx } from "keycloakify/tools/clsx";
import { kcSanitize } from "keycloakify/lib/kcSanitize";
import type { TemplateProps } from "keycloakify/login/TemplateProps";
import type { KcContext } from "./KcContext";
import type { I18n } from "./i18n";
import { Button } from "@/components/ui/button";
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Globe, AlertCircle, CheckCircle2, Info } from "lucide-react";
import AuthBackgroundShape from "@/login/assets/img/AuthBackgroundShape.tsx";

export default function Template(props: TemplateProps<KcContext, I18n>) {
    const {
        displayInfo = false,
        displayMessage = true,
        displayRequiredFields = false,
        socialProvidersNode = null,
        infoNode = null,
        documentTitle,
        bodyClassName,
        kcContext,
        i18n,
        children,
    } = props;

    const { msg, msgStr, currentLanguage, enabledLanguages } = i18n;
    const { realm, message, isAppInitiatedAction } = kcContext;

    useEffect(() => {
        document.title = documentTitle ?? msgStr("loginTitle", kcContext.realm.displayName);
    }, [documentTitle, i18n, kcContext.realm.displayName, msgStr]);

    return (
        <div className={clsx("relative flex h-auto min-h-screen items-center justify-center overflow-x-hidden px-4 py-10 sm:px-6 lg:px-8", bodyClassName)}>
            {/* Background decoration */}
            <div className="absolute">
                <AuthBackgroundShape />
            </div>

            <div className="w-full max-w-lg z-10 space-y-4">
                {/* Locale Selector */}
                {realm.internationalizationEnabled && enabledLanguages.length > 1 && (
                    <div className="flex justify-end">
                        <DropdownMenu>
                            <DropdownMenuTrigger asChild>
                                <Button
                                    variant="outline"
                                    size="sm"
                                    className="gap-2"
                                >
                                    <Globe className="h-4 w-4" />
                                    <span className="font-medium">
                                        {enabledLanguages.find(lang => lang.languageTag === currentLanguage.languageTag)?.label}
                                    </span>
                                </Button>
                            </DropdownMenuTrigger>
                            <DropdownMenuContent align="end" className="w-48">
                                {enabledLanguages.map((lang) => (
                                    <DropdownMenuItem key={lang.languageTag} asChild>
                                        <a
                                            href={lang.href}
                                            className={clsx(
                                                "cursor-pointer",
                                                lang.languageTag === currentLanguage.languageTag && "bg-primary/10 font-semibold"
                                            )}
                                        >
                                            <span className="flex items-center justify-between w-full">
                                                {lang.label}
                                                {lang.languageTag === currentLanguage.languageTag && (
                                                    <CheckCircle2 className="h-4 w-4 text-primary" />
                                                )}
                                            </span>
                                        </a>
                                    </DropdownMenuItem>
                                ))}
                            </DropdownMenuContent>
                        </DropdownMenu>
                    </div>
                )}

                {/* Messages */}
                {displayMessage && message !== undefined && (
                    <div
                        className={clsx(
                            "rounded-lg p-4 flex items-start gap-3",
                            message.type === "success" && "bg-green-50 dark:bg-green-950/20 border border-green-200 dark:border-green-800 text-green-800 dark:text-green-200",
                            message.type === "warning" && "bg-yellow-50 dark:bg-yellow-950/20 border border-yellow-200 dark:border-yellow-800 text-yellow-800 dark:text-yellow-200",
                            message.type === "error" && "bg-red-50 dark:bg-red-950/20 border border-red-200 dark:border-red-800 text-red-800 dark:text-red-200",
                            message.type === "info" && "bg-blue-50 dark:bg-blue-950/20 border border-blue-200 dark:border-blue-800 text-blue-800 dark:text-blue-200"
                        )}
                    >
                        {message.type === "success" && <CheckCircle2 className="h-5 w-5 mt-0.5 flex-shrink-0" />}
                        {message.type === "warning" && <AlertCircle className="h-5 w-5 mt-0.5 flex-shrink-0" />}
                        {message.type === "error" && <AlertCircle className="h-5 w-5 mt-0.5 flex-shrink-0" />}
                        {message.type === "info" && <Info className="h-5 w-5 mt-0.5 flex-shrink-0" />}
                        <span
                            className="text-sm font-medium flex-1"
                            dangerouslySetInnerHTML={{
                                __html: kcSanitize(message.summary),
                            }}
                        />
                    </div>
                )}

                {/* Required fields notice */}
                {displayRequiredFields && (
                    <div className="text-sm text-muted-foreground">
                        <span className="text-destructive">*</span> {msg("requiredFields")}
                    </div>
                )}

                {/* Main content */}
                {children}

                {/* Social providers */}
                {socialProvidersNode}

                {/* Info section */}
                {displayInfo && infoNode}

                {/* App initiated action */}
                {isAppInitiatedAction && (
                    <div className="mt-6">
                        <form action={kcContext.url.loginAction} method="post">
                            <input type="hidden" name="cancel-aia" value="true" />
                            <Button
                                type="submit"
                                variant="outline"
                                className="w-full"
                            >
                                {msg("doCancel")}
                            </Button>
                        </form>
                    </div>
                )}
            </div>
        </div>
    );
}