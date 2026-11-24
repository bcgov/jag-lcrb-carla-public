/** Scanner for secrets in the browser bundle. Pure function; it never opens .env files, values come from the process environment. */

/** Variables that hold secrets or private endpoints. Plain config (PORT, NODE_ENV, test URLs) is not secret. */
export function isSecretName(name: string): boolean {
  return /(_KEY|_TOKEN|_SECRET|_PASSWORD|^ACCESS_CODE|_API_URL)$/.test(name);
}

/** Known credential formats and generic "key = long token" assignments. */
export const KEY_PATTERNS: RegExp[] = [
  /\bvck_[A-Za-z0-9]{20,}/, // Vercel AI Gateway key
  /\bvercel_blob_rw_[A-Za-z0-9_]{10,}/, // Vercel Blob read-write token
  /\bsk-[A-Za-z0-9_-]{20,}/, // OpenAI-style secret key
  /(api[_-]?key|access[_-]?token|secret)["']?\s*[:=]\s*["'][A-Za-z0-9_\-]{24,}["']/i,
];

export interface ScanInput {
  text: string;
  /** Secret variable names (from .env.example). */
  names: string[];
  /** Their values, when set in the environment. Short values are ignored to avoid noise. */
  values: string[];
}

export function findSecrets({ text, names, values }: ScanInput): string[] {
  const hits: string[] = [];
  for (const name of names) {
    if (new RegExp(`\\b${name}\\b`).test(text)) hits.push(`variable name ${name}`);
  }
  for (const value of values) {
    if (value.length >= 8 && text.includes(value)) hits.push('a secret value from the environment');
  }
  for (const pattern of KEY_PATTERNS) {
    if (pattern.test(text)) hits.push(`key pattern ${pattern.source}`);
  }
  return hits;
}
