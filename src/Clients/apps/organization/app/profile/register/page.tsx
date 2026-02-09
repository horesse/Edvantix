"use client";

import { useState } from "react";

import { useRouter } from "next/navigation";

import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";

import useRegisterProfile from "@workspace/api-hooks/profiles/useRegisterProfile";
import { Gender } from "@workspace/types/profile";
import { Button } from "@workspace/ui/components/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@workspace/ui/components/card";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@workspace/ui/components/form";
import { Input } from "@workspace/ui/components/input";
import { Label } from "@workspace/ui/components/label";
import {
  RadioGroup,
  RadioGroupItem,
} from "@workspace/ui/components/radio-group";
import {
  type RegistrationFormData,
  registrationSchema,
} from "@workspace/validations/profile/registration";

const genderOptions = [
  { value: Gender.Male, label: "Мужской" },
  { value: Gender.Female, label: "Женский" },
  { value: Gender.None, label: "Не указан" },
] as const;

export default function ProfileRegisterPage() {
  const router = useRouter();
  const [serverError, setServerError] = useState<string | null>(null);

  const registerMutation = useRegisterProfile({
    onSuccess: () => {
      router.push("/");
    },
    onError: (error) => {
      const axiosError = error as {
        response?: { status?: number; data?: string };
      };

      if (axiosError.response?.status === 409) {
        setServerError("Профиль уже существует. Перенаправление...");
        router.push("/");
        return;
      }

      if (axiosError.response?.status === 400) {
        setServerError(
          axiosError.response.data ?? "Проверьте правильность заполнения полей",
        );
        return;
      }

      setServerError("Произошла ошибка при регистрации. Попробуйте ещё раз.");
    },
  });

  const form = useForm<RegistrationFormData>({
    resolver: zodResolver(registrationSchema),
    defaultValues: {
      lastName: "",
      firstName: "",
      middleName: "",
      birthDate: "",
      gender: undefined,
    },
  });

  function onSubmit(data: RegistrationFormData) {
    setServerError(null);
    registerMutation.mutate({
      lastName: data.lastName,
      firstName: data.firstName,
      middleName: data.middleName || null,
      birthDate: data.birthDate,
      gender: data.gender,
    });
  }

  return (
    <div className="from-background to-muted/20 flex min-h-screen items-center justify-center bg-linear-to-br p-4">
      <Card className="border-muted/50 w-full max-w-lg shadow-lg">
        <CardHeader className="text-center">
          <CardTitle className="text-2xl">Регистрация профиля</CardTitle>
          <CardDescription>
            Заполните информацию для создания вашего профиля
          </CardDescription>
        </CardHeader>
        <CardContent>
          <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-5">
              <FormField
                control={form.control}
                name="lastName"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Фамилия</FormLabel>
                    <FormControl>
                      <Input placeholder="Иванов" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="firstName"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Имя</FormLabel>
                    <FormControl>
                      <Input placeholder="Иван" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="middleName"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>
                      Отчество{" "}
                      <span className="text-muted-foreground font-normal">
                        (необязательно)
                      </span>
                    </FormLabel>
                    <FormControl>
                      <Input placeholder="Иванович" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="birthDate"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Дата рождения</FormLabel>
                    <FormControl>
                      <Input type="date" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="gender"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Пол</FormLabel>
                    <FormControl>
                      <RadioGroup
                        onValueChange={(value) => field.onChange(Number(value))}
                        value={field.value?.toString()}
                        className="flex gap-6"
                      >
                        {genderOptions.map((option) => (
                          <div
                            key={option.value}
                            className="flex items-center gap-2"
                          >
                            <RadioGroupItem
                              value={option.value.toString()}
                              id={`gender-${option.value}`}
                            />
                            <Label htmlFor={`gender-${option.value}`}>
                              {option.label}
                            </Label>
                          </div>
                        ))}
                      </RadioGroup>
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              {serverError && (
                <p className="text-destructive text-sm">{serverError}</p>
              )}

              <Button
                type="submit"
                className="w-full"
                size="lg"
                disabled={registerMutation.isPending}
              >
                {registerMutation.isPending
                  ? "Регистрация..."
                  : "Зарегистрировать профиль"}
              </Button>
            </form>
          </Form>
        </CardContent>
      </Card>
    </div>
  );
}
