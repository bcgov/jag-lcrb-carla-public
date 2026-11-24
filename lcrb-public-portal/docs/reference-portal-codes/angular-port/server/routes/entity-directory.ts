import {
  searchDirectory,
  type DirectoryScope,
  type DirectorySource,
} from "@server-lib/data/entity-directory"

/** GET /api/entity-directory: simulated BC Registry and Dynamics lookups for the Legal Entity Builder, privacy-limited fields only. Swap in real API calls. */
export async function GET(request: Request) {
  const { searchParams } = new URL(request.url)
  const query = searchParams.get("q") ?? ""
  const scope = (searchParams.get("scope") as DirectoryScope | null) ?? "all"
  const source = (searchParams.get("source") as DirectorySource | "all" | null) ?? "all"

  // Simulate network latency of the upstream registries.
  await new Promise((resolve) => setTimeout(resolve, 250))

  const results = searchDirectory({ query, scope, source })
  return Response.json({ results })
}
