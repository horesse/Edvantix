"use client";

import { useRef, useState } from "react";

import Image from "next/image";
import { useRouter } from "next/navigation";

import { zodResolver } from "@hookform/resolvers/zod";
import { UserCircle, X } from "lucide-react";
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
  RegistrationFormData,
  registrationSchema,
} from "@workspace/validations/profile";

const genderOptions = [
  { value: Gender.Male, label: "Мужской" },
  { value: Gender.Female, label: "Женский" },
  { value: Gender.None, label: "Не указан" },
] as const;

export default function ProfileRegisterPage() {
  const router = useRouter();
  const [serverError, setServerError] = useState<string | null>(null);
  const [avatarPreview, setAvatarPreview] = useState<string | null>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);

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
      avatar: null,
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
      avatar: data.avatar ?? null,
    });
  }

  function handleAvatarChange(file: File | null) {
    if (avatarPreview) {
      URL.revokeObjectURL(avatarPreview);
    }

    if (file) {
      form.setValue("avatar", file, { shouldValidate: true });
      setAvatarPreview(URL.createObjectURL(file));
    } else {
      form.setValue("avatar", null, { shouldValidate: true });
      setAvatarPreview(null);
    }
  }

  function removeAvatar() {
    handleAvatarChange(null);
    if (fileInputRef.current) {
      fileInputRef.current.value = "";
    }
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
                name="avatar"
                render={() => (
                  <FormItem className="flex flex-col items-center">
                    <FormLabel>
                      Аватар{" "}
                      <span className="text-muted-foreground font-normal">
                        (необязательно)
                      </span>
                    </FormLabel>
                    <FormControl>
                      <div className="flex flex-col items-center gap-3">
                        <div className="relative">
                          <button
                            type="button"
                            onClick={() => fileInputRef.current?.click()}
                            className="bg-muted hover:bg-muted/80 focus-visible:ring-ring flex h-24 w-24 items-center justify-center overflow-hidden rounded-full border-2 border-dashed transition-colors focus-visible:ring-2 focus-visible:ring-offset-2 focus-visible:outline-none"
                          >
                            {avatarPreview ? (
                              <Image
                                src={avatarPreview}
                                alt="Аватар"
                                fill
                                className="object-cover"
                                unoptimized
                              />
                            ) : (
                              <UserCircle className="text-muted-foreground h-12 w-12" />
                            )}
                          </button>
                          {avatarPreview && (
                            <button
                              type="button"
                              onClick={removeAvatar}
                              className="bg-destructive text-destructive-foreground hover:bg-destructive/90 absolute -top-1 -right-1 flex h-6 w-6 items-center justify-center rounded-full"
                            >
                              <X className="h-3.5 w-3.5" />
                            </button>
                          )}
                        </div>
                        <Input
                          ref={fileInputRef}
                          type="file"
                          accept="image/jpeg,image/png,image/gif,image/webp"
                          className="hidden"
                          onChange={(e) => {
                            const file = e.target.files?.[0] ?? null;
                            handleAvatarChange(file);
                          }}
                        />
                        <p className="text-muted-foreground text-xs">
                          JPEG, PNG, GIF или WebP. Макс. 5 МБ
                        </p>
                      </div>
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

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
