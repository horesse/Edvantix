"use client";

import { useCallback, useRef, useState } from "react";

import { usePathname, useRouter, useSearchParams } from "next/navigation";

import { Bell, Briefcase, GraduationCap, Mail, User } from "lucide-react";

import useProfileDetails from "@workspace/api-hooks/profiles/useProfileDetails";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@workspace/ui/components/alert-dialog";
import {
  Tabs,
  TabsContent,
  TabsList,
  TabsTrigger,
} from "@workspace/ui/components/tabs";
import { cn } from "@workspace/ui/lib/utils";

import { PageHeader } from "@/components/page-header";

import { AvatarBlock } from "./profile-avatar";
import { ProfileSettingsSkeleton } from "./profile-settings-ui";
import { TabContacts } from "./tab-contacts";
import { TabEducation } from "./tab-education";
import { TabEmployment } from "./tab-employment";
import { TabGeneral } from "./tab-general";
import { TabNotifications } from "./tab-notifications";

const TABS = [
  { value: "general", label: "Основная", icon: User },
  { value: "contacts", label: "Контакты", icon: Mail },
  { value: "education", label: "Образование", icon: GraduationCap },
  { value: "employment", label: "Опыт работы", icon: Briefcase },
  { value: "notifications", label: "Уведомления", icon: Bell },
] as const;

export function ProfileSettings() {
  const { data: profile, isLoading } = useProfileDetails();
  const router = useRouter();
  const pathname = usePathname();
  const searchParams = useSearchParams();
  const currentTab = searchParams.get("tab") ?? "general";

  const dirtyRef = useRef(false);
  const [pendingTab, setPendingTab] = useState<string | null>(null);

  const navigateToTab = useCallback(
    (value: string) => {
      const params = new URLSearchParams(searchParams.toString());
      if (value === "general") {
        params.delete("tab");
      } else {
        params.set("tab", value);
      }
      const qs = params.toString();
      router.replace(qs ? `${pathname}?${qs}` : pathname, { scroll: false });
    },
    [router, pathname, searchParams],
  );

  const handleTabChange = useCallback(
    (value: string) => {
      if (dirtyRef.current) {
        setPendingTab(value);
      } else {
        navigateToTab(value);
      }
    },
    [navigateToTab],
  );

  const handleDirtyChange = useCallback((dirty: boolean) => {
    dirtyRef.current = dirty;
  }, []);

  function handleConfirmLeave() {
    if (pendingTab) {
      dirtyRef.current = false;
      navigateToTab(pendingTab);
      setPendingTab(null);
    }
  }

  function handleCancelLeave() {
    setPendingTab(null);
  }

  if (isLoading || !profile) {
    return (
      <div className="space-y-6">
        <PageHeader title="Настройки профиля" />
        <ProfileSettingsSkeleton />
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <PageHeader
        title="Настройки профиля"
        description="Управляйте своей личной информацией"
      />

      <AvatarBlock profile={profile} />

      <Tabs
        value={currentTab}
        onValueChange={handleTabChange}
        orientation="vertical"
        className="flex flex-col gap-6 md:flex-row md:gap-8"
      >
        {/* Vertical sidebar navigation */}
        <TabsList className="border-border/40 md:bg-muted/30 flex h-auto w-full shrink-0 flex-row gap-0.5 overflow-x-auto rounded-none border-b bg-transparent p-0 md:sticky md:top-4 md:w-48 md:flex-col md:items-stretch md:gap-0.5 md:self-start md:overflow-x-visible md:rounded-lg md:border-0 md:p-1.5">
          {TABS.map((tab) => (
            <TabsTrigger
              key={tab.value}
              value={tab.value}
              className={cn(
                "text-muted-foreground relative justify-start gap-2 rounded-none border-0 bg-transparent px-3 py-2 shadow-none transition-colors",
                "hover:text-foreground",
                "data-[state=active]:text-foreground data-[state=active]:bg-transparent data-[state=active]:shadow-none",
                // Mobile: underline style
                "data-[state=active]:after:bg-primary after:absolute after:inset-x-0 after:bottom-0 after:h-0.5 after:bg-transparent",
                // Desktop: left border + background
                "md:rounded-md md:after:inset-x-auto md:after:inset-y-0 md:after:bottom-auto md:after:left-0 md:after:h-full md:after:w-0.5",
                "md:data-[state=active]:bg-background md:data-[state=active]:shadow-sm",
              )}
            >
              <tab.icon className="size-3.5 shrink-0" />
              <span className="text-xs font-medium whitespace-nowrap">
                {tab.label}
              </span>
            </TabsTrigger>
          ))}
        </TabsList>

        {/* Tab content */}
        <div className="min-w-0 flex-1">
          <TabsContent value="general" className="mt-0">
            <TabGeneral profile={profile} onDirtyChange={handleDirtyChange} />
          </TabsContent>
          <TabsContent value="contacts" className="mt-0">
            <TabContacts profile={profile} onDirtyChange={handleDirtyChange} />
          </TabsContent>
          <TabsContent value="education" className="mt-0">
            <TabEducation profile={profile} onDirtyChange={handleDirtyChange} />
          </TabsContent>
          <TabsContent value="employment" className="mt-0">
            <TabEmployment
              profile={profile}
              onDirtyChange={handleDirtyChange}
            />
          </TabsContent>
          <TabsContent value="notifications" className="mt-0">
            <TabNotifications />
          </TabsContent>
        </div>
      </Tabs>

      <AlertDialog
        open={pendingTab !== null}
        onOpenChange={(open) => {
          if (!open) setPendingTab(null);
        }}
      >
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Несохранённые изменения</AlertDialogTitle>
            <AlertDialogDescription>
              У вас есть несохранённые изменения. Если вы покинете вкладку, они
              будут потеряны.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel
              onClick={handleConfirmLeave}
              className="border-destructive/30 text-destructive hover:bg-destructive/10 hover:text-destructive"
            >
              Уйти без сохранения
            </AlertDialogCancel>
            <AlertDialogAction onClick={handleCancelLeave}>
              Остаться
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  );
}
