import type React from "react";
import { SidebarProvider, SidebarInset } from "@workspace/ui/components/sidebar";
import { AppSidebar } from "@/components/app-sidebar";
import { Header } from "@/components/header";

export default function MainLayout({
                                       children,
                                   }: {
    children: React.ReactNode;
}) {
    return (
        <SidebarProvider>
            <AppSidebar variant="inset" />
            <SidebarInset>
                <Header />
                <main className="flex flex-1 flex-col gap-4 p-4" id="main-content">
                    {children}
                </main>
            </SidebarInset>
        </SidebarProvider>
    );
}
