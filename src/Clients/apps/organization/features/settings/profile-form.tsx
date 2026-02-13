"use client";

import { useEffect } from "react";

import { zodResolver } from "@hookform/resolvers/zod";
import { Loader2 } from "lucide-react";
import { useForm } from "react-hook-form";
import { toast } from "sonner";

import useUpdateProfile from "@workspace/api-hooks/profiles/useUpdateProfile";
import type { OwnProfileDetails } from "@workspace/types/profile";
import { Gender } from "@workspace/types/profile";
import { Button } from "@workspace/ui/components/button";
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
  ProfileSettingsInput,
  profileSettingsSchema,
} from "@workspace/validations/profile";

const genderOptions = [
  { value: Gender.Male, label: "Мужской" },
  { value: Gender.Female, label: "Женский" },
  { value: Gender.None, label: "Не указан" },
] as const;

type ProfileFormProps = {
  profile: OwnProfileDetails;
};

export function ProfileForm({ profile }: ProfileFormProps) {
  const updateMutation = useUpdateProfile({
    onSuccess: () => {
      toast.success("Профиль обновлён");
    },
    onError: () => {
      toast.error("Не удалось обновить профиль");
    },
  });

  const form = useForm<ProfileSettingsInput>({
    resolver: zodResolver(profileSettingsSchema),
    defaultValues: {
      lastName: profile.lastName,
      firstName: profile.firstName,
      middleName: profile.middleName ?? "",
      birthDate: profile.birthDate,
      gender: profile.gender,
    },
  });

  useEffect(() => {
    form.reset({
      lastName: profile.lastName,
      firstName: profile.firstName,
      middleName: profile.middleName ?? "",
      birthDate: profile.birthDate,
      gender: profile.gender,
    });
  }, [profile, form]);

  function onSubmit(data: ProfileSettingsInput) {
    updateMutation.mutate({
      firstName: data.firstName,
      lastName: data.lastName,
      middleName: data.middleName || null,
      birthDate: data.birthDate,
      gender: data.gender,
    });
  }

  const isLoading = updateMutation.isPending;

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-5">
        <div className="grid gap-4 sm:grid-cols-2">
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
        </div>

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

        <div className="grid gap-4 sm:grid-cols-2">
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
                    className="flex gap-3 pt-2"
                  >
                    {genderOptions.map((option) => (
                      <div
                        key={option.value}
                        className="flex items-center gap-1.5"
                      >
                        <RadioGroupItem
                          value={option.value.toString()}
                          id={`gender-${option.value}`}
                        />
                        <Label
                          htmlFor={`gender-${option.value}`}
                          className="text-sm"
                        >
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
        </div>

        <div className="flex justify-end pt-2">
          <Button type="submit" disabled={isLoading}>
            {isLoading && <Loader2 className="size-4 animate-spin" />}
            <span>Сохранить</span>
          </Button>
        </div>
      </form>
    </Form>
  );
}
