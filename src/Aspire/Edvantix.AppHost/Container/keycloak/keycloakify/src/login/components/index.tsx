import { kcSanitize } from "keycloakify/lib/kcSanitize";
import { Button, type ButtonProps } from "@/components/ui/button";
import { Separator } from "@/components/ui/separator";

type SocialProvider = {
  alias: string;
  loginUrl: string;
  displayName: string;
  iconClasses?: string;
};

type SocialProvidersListProps = {
  providers: SocialProvider[];
  label: string;
};

/** Renders the list of identity provider login buttons with a labeled separator. */
export function SocialProvidersList({
  providers,
  label,
}: Readonly<SocialProvidersListProps>) {
  return (
    <div className="space-y-4">
      <div className="flex items-center gap-4">
        <Separator className="flex-1" />
        <p className="text-sm text-muted-foreground whitespace-nowrap">
          {label}
        </p>
        <Separator className="flex-1" />
      </div>

      <div className="grid gap-2">
        {providers.map((p) => (
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
  );
}

type FormSubmitButtonProps = ButtonProps & {
  isLoading: boolean;
  label: string;
};

/** Submit button that shows an inline spinner while the form is submitting. */
export function FormSubmitButton({
  isLoading,
  label,
  className,
  ...props
}: Readonly<FormSubmitButtonProps>) {
  return (
    <Button
      disabled={isLoading}
      className={className ?? "w-full h-11 text-base font-medium"}
      type="submit"
      {...props}
    >
      {isLoading ? (
        <span className="flex items-center gap-2">
          <span className="inline-block w-4 h-4 border-2 border-primary-foreground/30 border-t-primary-foreground rounded-full animate-spin" />
          {label}
        </span>
      ) : (
        label
      )}
    </Button>
  );
}
