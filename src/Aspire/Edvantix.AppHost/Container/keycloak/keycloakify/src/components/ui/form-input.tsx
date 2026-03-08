import * as React from "react";
import { useReducer, useId } from "react";
import { AlertCircle, Eye, EyeOff, Check, X } from "lucide-react";
import { cn } from "@/lib/utils";
import { Input } from "./input";
import { Label } from "./label";

/** Label with optional required asterisk, shared by FormInput and PasswordInput. */
function FormFieldLabel({
  htmlFor,
  required,
  children,
}: {
  htmlFor: string;
  required?: boolean;
  children: React.ReactNode;
}) {
  return (
    <Label
      htmlFor={htmlFor}
      className="flex items-center gap-1 text-sm font-medium"
    >
      {children}
      {required && (
        <span className="text-destructive" aria-hidden="true">
          *
        </span>
      )}
    </Label>
  );
}

/** Inline error message with icon, shared by FormInput and PasswordInput. */
function FormFieldError({ id, error }: { id: string; error: string }) {
  return (
    <div
      id={id}
      role="alert"
      className="flex items-start gap-2 text-destructive animate-in fade-in-0 slide-in-from-top-1 duration-200"
    >
      <AlertCircle className="h-4 w-4 mt-0.5 shrink-0" aria-hidden="true" />
      <span className="text-sm">{error}</span>
    </div>
  );
}

export type FormInputProps = {
  label: string;
  name: string;
  type?: "text" | "email" | "password";
  required?: boolean;
  error?: string | null;
  touched?: boolean;
  autoComplete?: string;
  autoFocus?: boolean;
  placeholder?: string;
  defaultValue?: string;
  value?: string;
  onChange?: (e: React.ChangeEvent<HTMLInputElement>) => void;
  onBlur?: () => void;
  className?: string;
  inputClassName?: string;
  disabled?: boolean;
};

export function FormInput({
  label,
  name,
  type = "text",
  required = false,
  error,
  touched = false,
  autoComplete,
  autoFocus,
  placeholder,
  defaultValue,
  value,
  onChange,
  onBlur,
  className,
  inputClassName,
  disabled,
}: FormInputProps) {
  const id = useId();
  const hasError = touched && error;

  return (
    <div className={cn("space-y-2", className)}>
      <FormFieldLabel htmlFor={id} required={required}>
        {label}
      </FormFieldLabel>
      <Input
        id={id}
        name={name}
        type={type}
        autoComplete={autoComplete}
        autoFocus={autoFocus}
        placeholder={placeholder}
        defaultValue={defaultValue}
        value={value}
        onChange={onChange}
        onBlur={onBlur}
        disabled={disabled}
        aria-invalid={!!hasError}
        aria-describedby={hasError ? `${id}-error` : undefined}
        className={cn(
          "transition-colors duration-200",
          hasError && "border-destructive focus-visible:ring-destructive/50",
          inputClassName,
        )}
      />
      {hasError && <FormFieldError id={`${id}-error`} error={error} />}
    </div>
  );
}

export type PasswordStrength = "weak" | "fair" | "good" | "strong";

export type PasswordInputProps = Omit<FormInputProps, "type"> & {
  showStrengthIndicator?: boolean;
  showRequirements?: boolean;
  i18n?: {
    showPassword?: string;
    hidePassword?: string;
    passwordStrength?: {
      weak?: string;
      fair?: string;
      good?: string;
      strong?: string;
    };
    requirements?: {
      minLength?: string;
      uppercase?: string;
      lowercase?: string;
      number?: string;
      special?: string;
    };
  };
};

const defaultI18n = {
  showPassword: "Показать пароль",
  hidePassword: "Скрыть пароль",
  passwordStrength: {
    weak: "Слабый",
    fair: "Средний",
    good: "Хороший",
    strong: "Надёжный",
  },
  requirements: {
    minLength: "Минимум 8 символов",
    uppercase: "Одна заглавная буква",
    lowercase: "Одна строчная буква",
    number: "Одна цифра",
    special: "Один спецсимвол",
  },
};

export function calculatePasswordStrength(password: string): {
  strength: PasswordStrength;
  score: number;
  requirements: {
    minLength: boolean;
    uppercase: boolean;
    lowercase: boolean;
    number: boolean;
    special: boolean;
  };
} {
  const requirements = {
    minLength: password.length >= 8,
    uppercase: /[A-Z]/.test(password),
    lowercase: /[a-z]/.test(password),
    number: /[0-9]/.test(password),
    special: /[!@#$%^&*(),.?":{}|<>]/.test(password),
  };

  const score = Object.values(requirements).filter(Boolean).length;

  let strength: PasswordStrength;
  if (score <= 2) strength = "weak";
  else if (score === 3) strength = "fair";
  else if (score === 4) strength = "good";
  else strength = "strong";

  return { strength, score, requirements };
}

export function PasswordInput({
  label,
  name,
  required = false,
  error,
  touched = false,
  autoComplete,
  autoFocus,
  placeholder,
  defaultValue,
  value,
  onChange,
  onBlur,
  className,
  inputClassName,
  disabled,
  showStrengthIndicator = false,
  showRequirements = false,
  i18n: customI18n,
}: PasswordInputProps) {
  const id = useId();
  const hasError = touched && error;
  const [isRevealed, toggleRevealed] = useReducer(
    (state: boolean) => !state,
    false,
  );
  const i18n = { ...defaultI18n, ...customI18n };

  const passwordValue = value ?? "";
  const { strength, requirements } = calculatePasswordStrength(passwordValue);

  const strengthColors: Record<PasswordStrength, string> = {
    weak: "bg-destructive",
    fair: "bg-yellow-500",
    good: "bg-blue-500",
    strong: "bg-green-500",
  };

  const strengthWidths: Record<PasswordStrength, string> = {
    weak: "w-1/4",
    fair: "w-2/4",
    good: "w-3/4",
    strong: "w-full",
  };

  return (
    <div className={cn("space-y-2", className)}>
      <FormFieldLabel htmlFor={id} required={required}>
        {label}
      </FormFieldLabel>
      <div className="relative">
        <Input
          id={id}
          name={name}
          type={isRevealed ? "text" : "password"}
          autoComplete={autoComplete}
          autoFocus={autoFocus}
          placeholder={placeholder}
          defaultValue={defaultValue}
          value={value}
          onChange={onChange}
          onBlur={onBlur}
          disabled={disabled}
          aria-invalid={!!hasError}
          aria-describedby={hasError ? `${id}-error` : undefined}
          className={cn(
            "pr-10 transition-colors duration-200",
            hasError && "border-destructive focus-visible:ring-destructive/50",
            inputClassName,
          )}
        />
        <button
          type="button"
          onClick={toggleRevealed}
          className={cn(
            "absolute right-1 top-1/2 -translate-y-1/2 h-7 w-7",
            "flex items-center justify-center rounded-md",
            "text-muted-foreground hover:text-foreground",
            "transition-colors duration-200",
            "focus:outline-none focus-visible:ring-2 focus-visible:ring-ring",
          )}
          aria-label={isRevealed ? i18n.hidePassword : i18n.showPassword}
          aria-controls={id}
          tabIndex={-1}
        >
          {isRevealed ? (
            <EyeOff className="h-4 w-4" aria-hidden="true" />
          ) : (
            <Eye className="h-4 w-4" aria-hidden="true" />
          )}
        </button>
      </div>

      {showStrengthIndicator && passwordValue.length > 0 && (
        <div className="space-y-1.5 animate-in fade-in-0 slide-in-from-top-1 duration-200">
          <div className="flex items-center justify-between">
            <span className="text-xs text-muted-foreground">
              Надёжность пароля
            </span>
            <span
              className={cn(
                "text-xs font-medium",
                strength === "weak" && "text-destructive",
                strength === "fair" && "text-yellow-600 dark:text-yellow-500",
                strength === "good" && "text-blue-600 dark:text-blue-500",
                strength === "strong" && "text-green-600 dark:text-green-500",
              )}
            >
              {i18n.passwordStrength?.[strength]}
            </span>
          </div>
          <div className="h-1.5 w-full rounded-full bg-muted overflow-hidden">
            <div
              className={cn(
                "h-full rounded-full transition-all duration-300 ease-out",
                strengthColors[strength],
                strengthWidths[strength],
              )}
            />
          </div>
        </div>
      )}

      {showRequirements && passwordValue.length > 0 && (
        <ul className="space-y-1 text-xs animate-in fade-in-0 slide-in-from-top-1 duration-200">
          {Object.entries(requirements).map(([key, met]) => (
            <li
              key={key}
              className={cn(
                "flex items-center gap-1.5 transition-colors duration-200",
                met
                  ? "text-green-600 dark:text-green-500"
                  : "text-muted-foreground",
              )}
            >
              {met ? (
                <Check className="h-3 w-3" aria-hidden="true" />
              ) : (
                <X className="h-3 w-3" aria-hidden="true" />
              )}
              <span>
                {i18n.requirements?.[key as keyof typeof i18n.requirements]}
              </span>
            </li>
          ))}
        </ul>
      )}

      {hasError && <FormFieldError id={`${id}-error`} error={error} />}
    </div>
  );
}
