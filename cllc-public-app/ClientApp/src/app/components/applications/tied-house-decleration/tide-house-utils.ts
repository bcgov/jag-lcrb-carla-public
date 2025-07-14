export function formatDate(dateString: string | null | undefined): string | null {
  if (!dateString?.trim()) {
    return null;
  }

  const date = new Date(dateString);

  return isNaN(date.getTime()) ? null : date.toISOString().split('T')[0];
}
