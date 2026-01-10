import type React from "react";

export default function MainLayout({
                                       children,
                                   }: {
    children: React.ReactNode;
}) {
    return (
        <div className="bg-background flex min-h-screen flex-col">
            <main className="flex-1" id="main-content">
                {children}
            </main>
        </div>
    );
}
