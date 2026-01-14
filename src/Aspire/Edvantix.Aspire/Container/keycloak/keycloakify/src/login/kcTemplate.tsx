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

export default function Template(props: TemplateProps<KcContext, I18n>) {
    const {
        displayInfo = false,
        displayMessage = true,
        displayRequiredFields = false,
        headerNode,
        socialProvidersNode = null,
        infoNode = null,
        documentTitle,
        bodyClassName,
        kcContext,
        i18n,
        // doUseDefaultCss,
        // classes,
        children,
    } = props;

    const { msg, msgStr, currentLanguage, enabledLanguages } = i18n;
    const { realm, message, isAppInitiatedAction } = kcContext;

    useEffect(() => {
        document.title = documentTitle ?? msgStr("loginTitle", kcContext.realm.displayName);
    }, [documentTitle, i18n, kcContext.realm.displayName, msgStr]);

    return (
        <div className={clsx("min-h-screen flex items-center justify-center p-4 bg-gradient-to-br from-background via-background to-muted/20", bodyClassName)}>
            {/* Background decoration */}
            <div className="fixed inset-0 -z-10 overflow-hidden">
                <div className="absolute -top-40 -right-40 w-80 h-80 bg-primary/5 rounded-full blur-3xl animate-pulse" />
                <div className="absolute -bottom-40 -left-40 w-80 h-80 bg-primary/5 rounded-full blur-3xl animate-pulse delay-1000" />
            </div>

            <div className="w-full max-w-md">
                {/* Locale Selector */}
                {realm.internationalizationEnabled && enabledLanguages.length > 1 && (
                    <div className="flex justify-end mb-4 animate-in fade-in slide-in-from-top-2 duration-500">
                        <DropdownMenu>
                            <DropdownMenuTrigger asChild>
                                <Button
                                    variant="outline"
                                    size="sm"
                                    className="gap-2 border-2 hover:border-primary/50 hover:bg-primary/5 transition-all duration-300 hover:shadow-lg hover:shadow-primary/10 group"
                                >
                                    <Globe className="h-4 w-4 transition-transform group-hover:rotate-12" />
                                    <span className="font-medium">
                    {enabledLanguages.find(lang => lang.languageTag === currentLanguage.languageTag)?.label}
                  </span>
                                </Button>
                            </DropdownMenuTrigger>
                            <DropdownMenuContent
                                align="end"
                                className="w-48 border-2 shadow-xl animate-in fade-in slide-in-from-top-2 duration-200"
                            >
                                {enabledLanguages.map((lang) => (
                                    <DropdownMenuItem
                                        key={lang.languageTag}
                                        asChild
                                    >
                                        <a
                                            href={lang.href}
                                            className={clsx(
                                                "cursor-pointer transition-colors duration-200",
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

                {/* Main Card */}
                <div className="relative flex h-auto min-h-screen items-center justify-center overflow-x-hidden px-4 py-10 sm:px-6 lg:px-8">
                    <div className='absolute'>
                        {/*<AuthBackgroundShape />*/}
                    </div>

                    {/* Header */}
                    <div className="space-y-4">
                        {headerNode}

                        {/* Messages */}
                        {displayMessage && message !== undefined && (
                            <div
                                className={clsx(
                                    "rounded-lg p-4 border-2 flex items-start gap-3 animate-in fade-in slide-in-from-top-2 duration-500",
                                    message.type === "success" && "bg-green-50 dark:bg-green-950/20 border-green-200 dark:border-green-800 text-green-800 dark:text-green-200",
                                    message.type === "warning" && "bg-yellow-50 dark:bg-yellow-950/20 border-yellow-200 dark:border-yellow-800 text-yellow-800 dark:text-yellow-200",
                                    message.type === "error" && "bg-red-50 dark:bg-red-950/20 border-red-200 dark:border-red-800 text-red-800 dark:text-red-200",
                                    message.type === "info" && "bg-blue-50 dark:bg-blue-950/20 border-blue-200 dark:border-blue-800 text-blue-800 dark:text-blue-200"
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
                    </div>

                    {/* Required fields notice */}
                    {displayRequiredFields && (
                        <div className="text-sm text-muted-foreground">
                            <span className="text-destructive">*</span> {msg("requiredFields")}
                        </div>
                    )}

                    {/* Main content */}
                    <div className="space-y-6">
                        {children}

                        {/* Social providers */}
                        {socialProvidersNode}

                        {/* Info section */}
                        {displayInfo && infoNode}
                    </div>

                    {/* App initiated action */}
                    {isAppInitiatedAction && (
                        <div className="pt-4 border-t animate-in fade-in slide-in-from-bottom-2 duration-500 delay-300">
                            <form action={kcContext.url.loginAction} method="post">
                                <input type="hidden" name="cancel-aia" value="true" />
                                <Button
                                    type="submit"
                                    variant="outline"
                                    className="w-full border-2 hover:border-primary/50 hover:bg-primary/5 transition-all duration-300"
                                >
                                    {msg("doCancel")}
                                </Button>
                            </form>
                        </div>
                    )}
                </div>

                {/* Footer branding (optional) */}
                <div className="mt-8 text-center text-sm text-muted-foreground animate-in fade-in duration-1000 delay-500">
                    <p>Powered by <span className="font-semibold text-foreground">Edvzntix</span></p>
                </div>
            </div>
        </div>
    );
}