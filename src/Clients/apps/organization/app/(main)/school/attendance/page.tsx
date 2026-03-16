import type { Metadata } from "next";

import { AttendancePage } from "@/features/school/attendance/attendance-page";

export const metadata: Metadata = {
  title: "Edvantix — Посещаемость",
};

export default function Page() {
  return <AttendancePage />;
}
