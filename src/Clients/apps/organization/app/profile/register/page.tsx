"use client";

import { useState } from "react";

import { zodResolver } from "@hookform/resolvers/zod";
import { useRouter } from "next/navigation";
import { useForm } from "react-hook-form";

import useRegisterProfile from "@workspace/api-hooks/profiles/useRegisterProfile";
import { Button } from "@workspace/ui/components/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@workspace/ui/components/card";
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from "@workspace/ui/components/form";
import { Input } from "@workspace/ui/components/input";
import { RadioGroup, RadioGroupItem } from "@workspace/ui/components/radio-group";
import { Gender } from "@workspace/types/profile";
import { type RegisterProfileInput, registerProfileSchema } from "@workspace/validations/profile/registration";

const STEPS = [
  { id: 1, title: "Основная информация", description: "Имя и фамилия" },
  { id: 2, title: "Дополнительно", description: "Дата рождения и пол" },
] as const;

export default function ProfileRegisterPage() {
  const router = useRouter();
  const [currentStep, setCurrentStep] = useState(1);
  const registerMutation = useRegisterProfile();

  const form = useForm<RegisterProfileInput>({
    resolver: zodResolver(registerProfileSchema),
    defaultValues: {
      firstName: "",
      lastName: "",
      middleName: "",
      birthDate: "",
      gender: Gender.Male,
    },
  });

  const onSubmit = async (data: RegisterProfileInput) => {
    registerMutation.mutate(
      {
        gender: data.gender,
        profile: {
          firstName: data.firstName,
          lastName: data.lastName,
          middleName: data.middleName,
          birthDate: data.birthDate,
        },
      },
      {
        onSuccess: () => {
          router.push("/");
        },
      },
    );
  };

  const handleNext = async () => {
    const fieldsToValidate = currentStep === 1 ? (["firstName", "lastName", "middleName"] as const) : (["birthDate", "gender"] as const);

    const isValid = await form.trigger(fieldsToValidate);

    if (isValid) {
      if (currentStep < STEPS.length) {
        setCurrentStep(currentStep + 1);
      } else {
        form.handleSubmit(onSubmit)();
      }
    }
  };

  const handleBack = () => {
    if (currentStep > 1) {
      setCurrentStep(currentStep - 1);
    }
  };

  const isLoading = registerMutation.isPending;

  return (
    <div className="min-h-screen flex items-center justify-center p-4 bg-gradient-to-br from-background to-muted/20">
      <Card className="w-full max-w-lg shadow-2xl border-muted/50">
        <CardHeader className="space-y-4">
          <div className="flex justify-between items-center">
            {STEPS.map((step, index) => (
              <div key={step.id} className="flex items-center flex-1">
                <div className="flex flex-col items-center flex-1">
                  <div
                    className={`w-10 h-10 rounded-full flex items-center justify-center font-semibold transition-colors ${
                      currentStep >= step.id
                        ? "bg-primary text-primary-foreground"
                        : "bg-muted text-muted-foreground"
                    }`}
                  >
                    {step.id}
                  </div>
                  <div className="mt-2 text-xs text-center hidden sm:block">
                    <div className={currentStep >= step.id ? "font-semibold" : "text-muted-foreground"}>
                      {step.title}
                    </div>
                  </div>
                </div>
                {index < STEPS.length - 1 && (
                  <div
                    className={`h-1 flex-1 mx-2 rounded transition-colors ${
                      currentStep > step.id ? "bg-primary" : "bg-muted"
                    }`}
                  />
                )}
              </div>
            ))}
          </div>
          <div className="text-center">
            <CardTitle className="text-2xl">Регистрация профиля</CardTitle>
            <CardDescription>{STEPS[currentStep - 1].description}</CardDescription>
          </div>
        </CardHeader>

        <CardContent>
          <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
              {currentStep === 1 && (
                <div className="space-y-4 animate-in fade-in slide-in-from-right-4 duration-300">
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
                        <FormLabel>Отчество (необязательно)</FormLabel>
                        <FormControl>
                          <Input placeholder="Иванович" {...field} value={field.value ?? ""} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </div>
              )}

              {currentStep === 2 && (
                <div className="space-y-4 animate-in fade-in slide-in-from-right-4 duration-300">
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
                            className="flex flex-col space-y-2"
                          >
                            <FormItem className="flex items-center space-x-3 space-y-0">
                              <FormControl>
                                <RadioGroupItem value={Gender.Male.toString()} />
                              </FormControl>
                              <FormLabel className="font-normal cursor-pointer">Мужской</FormLabel>
                            </FormItem>
                            <FormItem className="flex items-center space-x-3 space-y-0">
                              <FormControl>
                                <RadioGroupItem value={Gender.Female.toString()} />
                              </FormControl>
                              <FormLabel className="font-normal cursor-pointer">Женский</FormLabel>
                            </FormItem>
                            <FormItem className="flex items-center space-x-3 space-y-0">
                              <FormControl>
                                <RadioGroupItem value={Gender.Other.toString()} />
                              </FormControl>
                              <FormLabel className="font-normal cursor-pointer">Другое</FormLabel>
                            </FormItem>
                          </RadioGroup>
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </div>
              )}

              <div className="flex gap-3 pt-4">
                {currentStep > 1 && (
                  <Button type="button" variant="outline" onClick={handleBack} className="flex-1">
                    Назад
                  </Button>
                )}
                <Button
                  type="button"
                  onClick={handleNext}
                  disabled={isLoading}
                  className="flex-1"
                >
                  {isLoading ? "Сохранение..." : currentStep === STEPS.length ? "Завершить" : "Далее"}
                </Button>
              </div>
            </form>
          </Form>
        </CardContent>
      </Card>
    </div>
  );
}
