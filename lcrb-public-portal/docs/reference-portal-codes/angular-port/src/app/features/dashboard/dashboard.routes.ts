/** dashboard routes: loaded by app.routes.ts with loadChildren. Paths come from route-registry.ts. */
import type { Routes } from '@angular/router';
import { page } from '@core/navigation/route-page';
import { routePath } from '@/app/route-registry';

export const routes: Routes = [
  page(routePath('dashboard.home'), () => import('@features/dashboard/pages/dashboard.page').then((m) => m.DashboardPage)),
  page(routePath('dashboard.announcements'), () => import('@features/dashboard/pages/announcements.page').then((m) => m.AnnouncementsPage), {
    title: 'Announcements | Liquor and Cannabis Licensing',
  }),
  page(routePath('dashboard.notices'), () => import('@features/dashboard/pages/notices.page').then((m) => m.NoticesPage)),
  page(routePath('dashboard.activity'), () => import('@features/dashboard/pages/activity.page').then((m) => m.ActivityPage)),
  page(routePath('dashboard.calendar'), () => import('@features/dashboard/pages/calendar.page').then((m) => m.CalendarPage), {
    title: 'Renewals & Compliance Calendar | LCRB Portal',
  }),
];
