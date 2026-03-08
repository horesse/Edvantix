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
import {
  Globe,
  AlertCircle,
  CheckCircle2,
  Info,
  Check,
  GraduationCap,
  Shield,
  Sparkles,
  Users,
} from "lucide-react";

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
    document.title =
      documentTitle ?? msgStr("loginTitle", kcContext.realm.displayName);
  }, [documentTitle, i18n, kcContext.realm.displayName, msgStr]);

  return (
    <div className={clsx("relative flex min-h-screen w-full", bodyClassName)}>
      {/* Left Panel - Branding (hidden on mobile) */}
      <div className="hidden lg:flex lg:w-1/2 xl:w-[55%] relative overflow-hidden bg-gradient-to-br from-primary/90 via-primary to-primary/80">
        {/* Decorative elements */}
        <div className="absolute inset-0">
          <div className="absolute top-0 left-0 w-full h-full bg-[radial-gradient(ellipse_at_top_left,_rgba(255,255,255,0.15)_0%,_transparent_50%)]" />
          <div className="absolute bottom-0 right-0 w-full h-full bg-[radial-gradient(ellipse_at_bottom_right,_rgba(0,0,0,0.1)_0%,_transparent_50%)]" />

          {/* Animated circles */}
          <div className="absolute top-20 left-20 w-64 h-64 bg-white/5 rounded-full blur-3xl animate-pulse" />
          <div className="absolute bottom-40 right-20 w-96 h-96 bg-white/5 rounded-full blur-3xl animate-pulse delay-1000" />
          <div className="absolute top-1/2 left-1/3 w-48 h-48 bg-white/5 rounded-full blur-2xl animate-pulse delay-500" />

          {/* Grid pattern */}
          <div
            className="absolute inset-0 opacity-[0.03]"
            style={{
              backgroundImage: `url("data:image/svg+xml,%3Csvg width='60' height='60' viewBox='0 0 60 60' xmlns='http://www.w3.org/2000/svg'%3E%3Cg fill='none' fill-rule='evenodd'%3E%3Cg fill='%23ffffff' fill-opacity='1'%3E%3Cpath d='M36 34v-4h-2v4h-4v2h4v4h2v-4h4v-2h-4zm0-30V0h-2v4h-4v2h4v4h2V6h4V4h-4zM6 34v-4H4v4H0v2h4v4h2v-4h4v-2H6zM6 4V0H4v4H0v2h4v4h2V6h4V4H6z'/%3E%3C/g%3E%3C/g%3E%3C/svg%3E")`,
            }}
          />
        </div>

        {/* Content */}
        <div className="relative z-10 flex flex-col justify-between w-full p-12 xl:p-16 text-white">
          {/* Logo */}
          <div className="flex items-center gap-3">
            <div className="flex items-center justify-center w-12 h-12 rounded-xl bg-white/10 backdrop-blur-sm">
              <GraduationCap className="w-7 h-7" />
            </div>
            <span className="text-2xl font-bold tracking-tight">Edvantix</span>
          </div>

          {/* Main content */}
          <div className="space-y-8">
            <div className="space-y-4">
              <h1 className="text-4xl xl:text-5xl font-bold leading-tight">
                Управляйте онлайн-школой
                <br />
                <span className="text-white/80">эффективно</span>
              </h1>
              <p className="text-lg xl:text-xl text-white/70 max-w-md leading-relaxed">
                Современная платформа для создания и управления образовательными
                курсами
              </p>
            </div>

            {/* Features */}
            <div className="space-y-4">
              <Feature
                icon={<Sparkles className="w-5 h-5" />}
                text="Интуитивный конструктор курсов"
              />
              <Feature
                icon={<Users className="w-5 h-5" />}
                text="Управление учениками и преподавателями"
              />
              <Feature
                icon={<Shield className="w-5 h-5" />}
                text="Безопасное хранение данных"
              />
            </div>
          </div>

          {/* Footer */}
          <div className="text-sm text-white/50">
            © {new Date().getFullYear()} Edvantix. Все права защищены.
          </div>
        </div>
      </div>

      {/* Right Panel - Form */}
      <div className="flex-1 flex flex-col min-h-screen bg-background">
        {/* Mobile header */}
        <div className="lg:hidden flex items-center justify-between p-4 border-b border-border/50">
          <div className="flex items-center gap-2">
            <div className="flex items-center justify-center w-9 h-9 rounded-lg bg-primary/10">
              <GraduationCap className="w-5 h-5 text-primary" />
            </div>
            <span className="text-lg font-bold">Edvantix</span>
          </div>

          {realm.internationalizationEnabled && enabledLanguages.length > 1 && (
            <LanguageSelector
              currentLanguage={currentLanguage}
              enabledLanguages={enabledLanguages}
            />
          )}
        </div>

        {/* Desktop language selector */}
        {realm.internationalizationEnabled && enabledLanguages.length > 1 && (
          <div className="hidden lg:flex justify-end p-6">
            <LanguageSelector
              currentLanguage={currentLanguage}
              enabledLanguages={enabledLanguages}
            />
          </div>
        )}

        {/* Form container */}
        <div className="flex-1 flex items-center justify-center p-4 sm:p-6 lg:p-8">
          <div className="w-full max-w-[420px] space-y-6">
            {/* Messages */}
            {displayMessage && message !== undefined && (
              <MessageAlert message={message} />
            )}

            {/* Required fields notice */}
            {displayRequiredFields && (
              <div className="text-sm text-muted-foreground">
                <span className="text-destructive" aria-hidden="true">
                  *
                </span>{" "}
                {msg("requiredFields")}
              </div>
            )}

            {/* Main content */}
            <div className="animate-in fade-in-0 slide-in-from-bottom-4 duration-500">
              {children}
            </div>

            {/* Social providers */}
            {socialProvidersNode && (
              <div className="animate-in fade-in-0 duration-500 delay-100">
                {socialProvidersNode}
              </div>
            )}

            {/* Info section */}
            {displayInfo && infoNode && (
              <div className="animate-in fade-in-0 duration-500 delay-150">
                {infoNode}
              </div>
            )}

            {/* App initiated action */}
            {isAppInitiatedAction && (
              <div className="pt-4">
                <form action={kcContext.url.loginAction} method="post">
                  <input type="hidden" name="cancel-aia" value="true" />
                  <Button type="submit" variant="outline" className="w-full">
                    {msg("doCancel")}
                  </Button>
                </form>
              </div>
            )}
          </div>
        </div>

        {/* Mobile footer */}
        <div className="lg:hidden text-center text-xs text-muted-foreground p-4 border-t border-border/50">
          © {new Date().getFullYear()} Edvantix
        </div>
      </div>
    </div>
  );
}

function Feature({ icon, text }: { icon: React.ReactNode; text: string }) {
  return (
    <div className="flex items-center gap-3">
      <div className="flex items-center justify-center w-10 h-10 rounded-lg bg-white/10 backdrop-blur-sm">
        {icon}
      </div>
      <span className="text-white/90">{text}</span>
    </div>
  );
}

function LanguageSelector({
  currentLanguage,
  enabledLanguages,
}: {
  currentLanguage: { languageTag: string };
  enabledLanguages: Array<{ languageTag: string; label: string; href: string }>;
}) {
  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <Button
          variant="ghost"
          size="sm"
          className="gap-2 text-muted-foreground hover:text-foreground"
        >
          <Globe className="h-4 w-4" />
          <span className="text-sm">
            {
              enabledLanguages.find(
                (lang) => lang.languageTag === currentLanguage.languageTag,
              )?.label
            }
          </span>
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end" className="w-40">
        {enabledLanguages.map((lang) => (
          <DropdownMenuItem key={lang.languageTag} asChild>
            <a
              href={lang.href}
              className={clsx(
                "cursor-pointer flex items-center justify-between",
                lang.languageTag === currentLanguage.languageTag &&
                  "font-medium",
              )}
            >
              <span>{lang.label}</span>
              {lang.languageTag === currentLanguage.languageTag && (
                <Check className="h-4 w-4 text-primary" />
              )}
            </a>
          </DropdownMenuItem>
        ))}
      </DropdownMenuContent>
    </DropdownMenu>
  );
}

function MessageAlert({
  message,
}: {
  message: { type: string; summary: string };
}) {
  return (
    <div
      className={clsx(
        "rounded-xl p-4 flex items-start gap-3",
        "animate-in fade-in-0 slide-in-from-top-2 duration-300",
        "border shadow-sm",
        message.type === "success" &&
          "bg-green-50 dark:bg-green-950/30 border-green-200 dark:border-green-800/50 text-green-800 dark:text-green-300",
        message.type === "warning" &&
          "bg-amber-50 dark:bg-amber-950/30 border-amber-200 dark:border-amber-800/50 text-amber-800 dark:text-amber-300",
        message.type === "error" &&
          "bg-red-50 dark:bg-red-950/30 border-red-200 dark:border-red-800/50 text-red-800 dark:text-red-300",
        message.type === "info" &&
          "bg-blue-50 dark:bg-blue-950/30 border-blue-200 dark:border-blue-800/50 text-blue-800 dark:text-blue-300",
      )}
      role="alert"
    >
      {message.type === "success" && (
        <CheckCircle2
          className="h-5 w-5 mt-0.5 flex-shrink-0"
          aria-hidden="true"
        />
      )}
      {message.type === "warning" && (
        <AlertCircle
          className="h-5 w-5 mt-0.5 flex-shrink-0"
          aria-hidden="true"
        />
      )}
      {message.type === "error" && (
        <AlertCircle
          className="h-5 w-5 mt-0.5 flex-shrink-0"
          aria-hidden="true"
        />
      )}
      {message.type === "info" && (
        <Info className="h-5 w-5 mt-0.5 flex-shrink-0" aria-hidden="true" />
      )}
      <span
        className="text-sm font-medium flex-1"
        dangerouslySetInnerHTML={{
          __html: kcSanitize(message.summary),
        }}
      />
    </div>
  );
}
