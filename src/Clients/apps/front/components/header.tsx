"use client"

import { LogOut, User } from "lucide-react"
import {
  SidebarTrigger,
} from "@workspace/ui/components/sidebar"
import { Separator } from "@workspace/ui/components/separator"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@workspace/ui/components/dropdown-menu"
import { Button } from "@workspace/ui/components/button"
import { ThemeToggle } from "./theme-toggle"
import { signOut, useSession } from "@/lib/auth-client"

export function Header() {
  const { data: session } = useSession()

  const handleSignOut = async () => {
    await signOut({
      fetchOptions: {
        onSuccess: () => {
          window.location.href = "/login"
        },
      },
    })
  }

  return (
      <header className="flex h-(--header-height) shrink-0 items-center gap-2 border-b transition-[width,height] ease-linear group-has-data-[collapsible=icon]/sidebar-wrapper:h-(--header-height)">
          <div className="flex w-full items-center gap-1 px-4 lg:gap-2 lg:px-6">
              <SidebarTrigger className="-ml-1" />
              <Separator
                  orientation="vertical"
                  className="mx-2 data-[orientation=vertical]:h-4"
              />
              <h1 className="text-base font-medium">Documents</h1>
              <div className="ml-auto flex items-center gap-2">
                  <ThemeToggle></ThemeToggle>
              </div>
          </div>
      </header>
  )
}
