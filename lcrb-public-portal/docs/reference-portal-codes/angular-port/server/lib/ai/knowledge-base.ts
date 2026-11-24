/** BM25 retriever over knowledge-base.json (LCRB policy chunks); prototype stand-in for Azure AI Search RAG. Server-only. */

import rawChunks from "@server-lib/ai/knowledge-base.json"

export interface KnowledgeChunk {
  id: string
  /** Friendly document title, e.g. "Special Event Permit Policy Manual". */
  doc: string
  /** Licence/topic tag, e.g. "sep", "food-primary", "compliance". */
  tag: string
  /** Detected section heading (may be empty). */
  heading: string
  /** The chunk body text. */
  text: string
}

export interface RetrievedChunk extends KnowledgeChunk {
  score: number
  /** Official gov.bc.ca page for this document, for citation. */
  source?: { label: string; url: string }
}

const CHUNKS = rawChunks as KnowledgeChunk[]

/** Authoritative gov.bc.ca pages per document tag, so the assistant can point users to the official source. */
export const DOC_SOURCES: Record<string, { label: string; url: string }> = {
  sep: {
    label: "Apply for a Liquor Special Event Permit",
    url: "https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/liquor-licence-permits/apply-for-liquor-licence-permit/apply-for-liquor-special-event-permit",
  },
  "food-primary": {
    label: "Apply for a Food Primary Licence",
    url: "https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/liquor-licence-permits/apply-for-liquor-licence-permit/apply-for-a-food-primary-licence",
  },
  "liquor-primary": {
    label: "Apply for a Liquor Primary Licence",
    url: "https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/liquor-licence-permits/apply-for-liquor-licence-permit/apply-for-a-liquor-primary-licence",
  },
  manufacturer: {
    label: "Apply for a Manufacturer Licence",
    url: "https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/liquor-licence-permits/apply-for-liquor-licence-permit/apply-for-a-manufacturer-licence",
  },
  agent: {
    label: "Apply for an Agent Licence",
    url: "https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/liquor-licence-permits/apply-for-liquor-licence-permit/apply-for-an-agent-licence",
  },
  catering: {
    label: "Apply for a Catering Licence",
    url: "https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/liquor-licence-permits/apply-for-liquor-licence-permit/apply-for-a-catering-licence",
  },
  "licensee-retail-store": {
    label: "Apply for a Rural Licensee Retail Store Licence",
    url: "https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/liquor-licence-permits/apply-for-liquor-licence-permit/apply-for-a-rural-licensee-retail-store-licence",
  },
  "ubrew-uvin": {
    label: "Apply for a UBrew or UVin Licence",
    url: "https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/liquor-licence-permits/apply-for-liquor-licence-permit/apply-for-a-ubrew-or-uvin-licence",
  },
  compliance: {
    label: "Inspections and Enforcement",
    url: "https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/inspections-and-enforcement",
  },
  general: {
    label: "Liquor Licence Terms & Conditions Manuals",
    url: "https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/liquor-licence-permits/liquor-law-policy/liquor-licence-terms-conditions-manuals",
  },
}

/** Curated official links for common topics, returned alongside answers. */
export const RESOURCE_LINKS: { keywords: string[]; label: string; url: string }[] = [
  {
    keywords: ["serving it right", "sir", "responsible service", "server certificate", "certification"],
    label: "Serving It Right",
    url: "https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/liquor-licence-permits/liquor-resources-information/serving-it-right",
  },
  {
    keywords: ["special event server", "ses", "special event serve"],
    label: "Special Event Server certification",
    url: "https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/liquor-licence-permits/liquor-resources-information/special-event-serve",
  },
  {
    keywords: ["selling it right", "cannabis worker", "cannabis certification"],
    label: "Selling It Right",
    url: "https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/cannabis-licences/cannabis-resources-information/selling-it-right",
  },
  {
    keywords: ["form", "forms", "application form", "pdf"],
    label: "BC Liquor Licensing Forms",
    url: "https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/liquor-licence-permits/liquor-resources-information/bc-liquor-licensing-forms",
  },
  {
    keywords: ["terms and conditions", "terms & conditions", "manual", "handbook"],
    label: "Liquor Licence Terms & Conditions Manuals",
    url: "https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/liquor-licence-permits/liquor-law-policy/liquor-licence-terms-conditions-manuals",
  },
  {
    keywords: ["legislation", "regulation", "act", "law"],
    label: "Liquor Legislation & Regulations",
    url: "https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/liquor-licence-permits/liquor-law-policy/liquor-legislation-regulations",
  },
  {
    keywords: ["screening", "background", "associate", "financial integrity"],
    label: "Liquor Licensee and Applicant Screening",
    url: "https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/liquor-licence-permits/liquor-resources-information/liquor-licensee-and-applicant-screening",
  },
  {
    keywords: ["cannabis retail", "cannabis store", "crs"],
    label: "Apply for a Cannabis Retail Store Licence",
    url: "https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/cannabis-licences/apply-cannabis-licence/apply-for-a-cannabis-retail-store-licence",
  },
  {
    keywords: ["inspection", "enforcement", "contravention", "penalty", "compliance"],
    label: "Inspections and Enforcement",
    url: "https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/inspections-and-enforcement",
  },
]


const STOPWORDS = new Set(
  "a an and are as at be by for from has have he her his i in is it its of on or that the their them they this to was were will with you your our we us if not no may must can could should would shall about into out over under than then so such may any all each other more most some these those which who whom whose what when where why how".split(
    " ",
  ),
)

// Light domain expansion so abbreviations match the source documents.
const SYNONYMS: Record<string, string[]> = {
  sep: ["special", "event", "permit"],
  sir: ["serving", "it", "right"],
  ses: ["special", "event", "server"],
  lp: ["liquor", "primary"],
  fp: ["food", "primary"],
  lrs: ["licensee", "retail", "store"],
  crs: ["cannabis", "retail", "store"],
  tesa: ["temporary", "expanded", "service", "area"],
  ubrew: ["brew", "ferment"],
  uvin: ["wine", "ferment"],
  patio: ["outdoor", "patio", "area"],
  minors: ["minor", "minors"],
}

function tokenize(text: string): string[] {
  const out: string[] = []
  for (const raw of text.toLowerCase().split(/[^a-z0-9]+/)) {
    if (!raw) continue
    if (STOPWORDS.has(raw)) continue
    if (raw.length < 2 && !/[0-9]/.test(raw)) continue
    out.push(raw)
    if (SYNONYMS[raw]) out.push(...SYNONYMS[raw])
  }
  return out
}

interface Index {
  docFreq: Map<string, number>
  postings: { tokens: Map<string, number>; len: number }[]
  avgLen: number
  N: number
}

let _index: Index | null = null

function buildIndex(): Index {
  const docFreq = new Map<string, number>()
  const postings: { tokens: Map<string, number>; len: number }[] = []
  let totalLen = 0

  for (const chunk of CHUNKS) {
    const tokens = tokenize(`${chunk.heading} ${chunk.heading} ${chunk.text}`)
    const tf = new Map<string, number>()
    for (const t of tokens) tf.set(t, (tf.get(t) ?? 0) + 1)
    for (const t of tf.keys()) docFreq.set(t, (docFreq.get(t) ?? 0) + 1)
    postings.push({ tokens: tf, len: tokens.length })
    totalLen += tokens.length
  }

  return { docFreq, postings, avgLen: totalLen / Math.max(1, CHUNKS.length), N: CHUNKS.length }
}

function getIndex(): Index {
  if (!_index) _index = buildIndex()
  return _index
}

export interface RetrieveOptions {
  /** Boost chunks whose tag matches the active licence/flow. */
  boostTag?: string
  /** Max chunks to return. */
  limit?: number
  /** Minimum BM25 score to include a chunk. */
  minScore?: number
}

/** Retrieve the most relevant chunks for a query using BM25, with an optional boost for the licence type in use. */
export function retrieveKnowledge(query: string, opts: RetrieveOptions = {}): RetrievedChunk[] {
  const { boostTag, limit = 6, minScore = 1.5 } = opts
  const qTokens = tokenize(query)
  if (qTokens.length === 0) return []

  const { docFreq, postings, avgLen, N } = getIndex()
  const k1 = 1.5
  const b = 0.75

  const qSet = new Map<string, number>()
  for (const t of qTokens) qSet.set(t, (qSet.get(t) ?? 0) + 1)

  const scored: RetrievedChunk[] = []
  for (let i = 0; i < CHUNKS.length; i++) {
    const post = postings[i]
    let score = 0
    for (const [term] of qSet) {
      const tf = post.tokens.get(term)
      if (!tf) continue
      const df = docFreq.get(term) ?? 0
      const idf = Math.log(1 + (N - df + 0.5) / (df + 0.5))
      const denom = tf + k1 * (1 - b + (b * post.len) / avgLen)
      score += idf * ((tf * (k1 + 1)) / denom)
    }
    if (score <= 0) continue
    const chunk = CHUNKS[i]
    // Boost chunks from the licence type the user is working with.
    if (boostTag && chunk.tag === boostTag) score *= 1.6
    // The C&E Operational Procedures Manual is internal; keep it from drowning out applicant handbooks unless the user asks about compliance/enforcement.
    if (chunk.tag === "compliance" && boostTag !== "compliance") score *= 0.55
    scored.push({ ...chunk, score, source: DOC_SOURCES[chunk.tag] })
  }

  scored.sort((a, b) => b.score - a.score)

  // De-duplicate near-identical chunks from the same doc/heading.
  const seen = new Set<string>()
  const result: RetrievedChunk[] = []
  for (const c of scored) {
    if (c.score < minScore) break
    const key = `${c.doc}|${c.heading}`
    if (seen.has(key) && result.length > 2) continue
    seen.add(key)
    result.push(c)
    if (result.length >= limit) break
  }
  return result
}

/** Suggest official resource links relevant to a query. */
export function relevantLinks(query: string, max = 3): { label: string; url: string }[] {
  const q = query.toLowerCase()
  const hits: { label: string; url: string; score: number }[] = []
  for (const link of RESOURCE_LINKS) {
    let score = 0
    for (const kw of link.keywords) if (q.includes(kw)) score += kw.length
    if (score > 0) hits.push({ label: link.label, url: link.url, score })
  }
  hits.sort((a, b) => b.score - a.score)
  return hits.slice(0, max).map(({ label, url }) => ({ label, url }))
}

/** Map a flow title / pathname to a knowledge tag for retrieval boosting. */
export function tagFromContext(flowTitle?: string, pathname?: string): string | undefined {
  const hay = `${flowTitle ?? ""} ${pathname ?? ""}`.toLowerCase()
  if (/special.?event|\bsep\b/.test(hay)) return "sep"
  if (/food.?primary/.test(hay)) return "food-primary"
  if (/liquor.?primary/.test(hay)) return "liquor-primary"
  if (/manufactur/.test(hay)) return "manufacturer"
  if (/agent/.test(hay)) return "agent"
  if (/cater/.test(hay)) return "catering"
  if (/rural|licensee.?retail|\blrs\b/.test(hay)) return "licensee-retail-store"
  if (/ubrew|uvin|ferment/.test(hay)) return "ubrew-uvin"
  if (/inspect|enforce|complian|contravention/.test(hay)) return "compliance"
  return undefined
}
