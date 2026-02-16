"use client";

import type React from "react";

import { usePathname } from "next/navigation";

import {
  ContentArea,
  GridLayout,
  MainArea,
} from "@workspace/ui/components/grid-layout";
import { PageBase } from "@workspace/ui/components/page-base";

import { AppSidebarIsland } from "@/components/app-sidebar-island";
import { AuthGuard } from "@/components/auth-guard";
import { Header } from "@/components/header";
import { OrganizationProvider } from "@/components/organization-provider";
import { ProfileGuard } from "@/components/profile-guard";
import { SidebarProvider } from "@/components/sidebar-context";
import { VerticalNavIsland } from "@/components/vertical-nav-island";

export default function MainLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const pathname = usePathname();
  const isOrganizationPage = pathname.startsWith("/organization");

  return (
    <AuthGuard>
      <ProfileGuard>
        <OrganizationProvider>
          <SidebarProvider>
            <PageBase padding="md">
              <GridLayout gap="md">
                <AppSidebarIsland />
                {isOrganizationPage && <VerticalNavIsland />}
                <MainArea gap="sm">
                  <Header />
                  <ContentArea>{children}</ContentArea>
                </MainArea>
              </GridLayout>
            </PageBase>
          </SidebarProvider>
        </OrganizationProvider>
      </ProfileGuard>
    </AuthGuard>
  );
}
