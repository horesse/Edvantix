import { useCallback, useState } from "react";

export type ValidationRule = {
  validate: (value: string) => boolean;
  message: string;
};

export type FieldValidation = {
  value: string;
  error: string | null;
  touched: boolean;
  isValid: boolean;
};

export type FormValidationState = Record<string, FieldValidation>;

export type ValidationRules = Record<string, ValidationRule[]>;

export const commonValidationRules = {
  required: (message = "Обязательное поле"): ValidationRule => ({
    validate: (value: string) => value.trim().length > 0,
    message,
  }),
  email: (message = "Некорректный формат email"): ValidationRule => ({
    validate: (value: string) =>
      /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value) || value.trim().length === 0,
    message,
  }),
  minLength: (min: number, message?: string): ValidationRule => ({
    validate: (value: string) =>
      value.trim().length >= min || value.trim().length === 0,
    message: message ?? `Минимум ${min} символов`,
  }),
  maxLength: (max: number, message?: string): ValidationRule => ({
    validate: (value: string) => value.trim().length <= max,
    message: message ?? `Максимум ${max} символов`,
  }),
  pattern: (regex: RegExp, message = "Неверный формат"): ValidationRule => ({
    validate: (value: string) => regex.test(value) || value.trim().length === 0,
    message,
  }),
  matchField: (
    getFieldValue: () => string,
    message = "Значения не совпадают",
  ): ValidationRule => ({
    validate: (value: string) => value === getFieldValue(),
    message,
  }),
};

export function useFormValidation(rules: ValidationRules) {
  const initialState: FormValidationState = Object.keys(rules).reduce(
    (acc, fieldName) => ({
      ...acc,
      [fieldName]: {
        value: "",
        error: null,
        touched: false,
        isValid: true,
      },
    }),
    {},
  );

  const [fields, setFields] = useState<FormValidationState>(initialState);

  const validateField = useCallback(
    (fieldName: string, value: string): string | null => {
      const fieldRules = rules[fieldName];
      if (!fieldRules) return null;

      for (const rule of fieldRules) {
        if (!rule.validate(value)) {
          return rule.message;
        }
      }
      return null;
    },
    [rules],
  );

  const setValue = useCallback(
    (fieldName: string, value: string) => {
      const error = validateField(fieldName, value);
      setFields((prev) => ({
        ...prev,
        [fieldName]: {
          value,
          error: prev[fieldName]?.touched ? error : null,
          touched: prev[fieldName]?.touched ?? false,
          isValid: error === null,
        },
      }));
    },
    [validateField],
  );

  const setTouched = useCallback(
    (fieldName: string) => {
      setFields((prev) => {
        const field = prev[fieldName];
        if (!field) return prev;
        const error = validateField(fieldName, field.value);
        return {
          ...prev,
          [fieldName]: {
            ...field,
            touched: true,
            error,
            isValid: error === null,
          },
        };
      });
    },
    [validateField],
  );

  const validateAll = useCallback((): boolean => {
    let allValid = true;
    const newFields: FormValidationState = {};

    for (const fieldName of Object.keys(rules)) {
      const field = fields[fieldName];
      const error = validateField(fieldName, field?.value ?? "");
      newFields[fieldName] = {
        value: field?.value ?? "",
        error,
        touched: true,
        isValid: error === null,
      };
      if (error !== null) {
        allValid = false;
      }
    }

    setFields(newFields);
    return allValid;
  }, [fields, rules, validateField]);

  const reset = useCallback(() => {
    setFields(initialState);
  }, [initialState]);

  const getFieldProps = useCallback(
    (fieldName: string) => {
      const field = fields[fieldName];
      return {
        value: field?.value ?? "",
        onChange: (e: React.ChangeEvent<HTMLInputElement>) =>
          setValue(fieldName, e.target.value),
        onBlur: () => setTouched(fieldName),
        "aria-invalid": field?.touched && !field?.isValid,
      };
    },
    [fields, setValue, setTouched],
  );

  return {
    fields,
    setValue,
    setTouched,
    validateField,
    validateAll,
    reset,
    getFieldProps,
  };
}
