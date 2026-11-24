import { type AnnouncementCategory } from "@domain/models/notifications"

export const CATEGORY_LABELS: Record<AnnouncementCategory, string> = {
  policy: "Policy",
  regulatory: "Regulatory",
  release: "Portal Update",
  service: "Service Notice",
}
