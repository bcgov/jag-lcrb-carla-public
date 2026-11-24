/** Legal Entity (LE) build - data model. */

import { MOCK_LICENCES } from "../../licences/mock/licences"
import { type LENode, type LEEdge, type LegalEntityBuild } from "@domain/models/legal-entity"


/** Build a realistic sample LE structure for a business. */
export function createSampleBuild(businessLegalName: string, businessNumber: string): LegalEntityBuild {
  const applicantId = "node_applicant"
  const holdcoAId = "node_holdco_a"
  const holdcoBId = "node_holdco_b"
  const trustId = "node_trust"
  const dbaId = "node_dba"
  const familyId = "node_family"

  const nodes: LENode[] = [
    {
      id: applicantId,
      type: "applicant",
      legalName: businessLegalName,
      businessNumber,
      incorporationNumber: "BC1188002",
      jurisdiction: "British Columbia",
      linkedLicenceIds: MOCK_LICENCES.map((l) => l.id),
      people: [
        { id: "p_a1", name: "Jordan Avery", role: "director", locked: true },
        { id: "p_a2", name: "Morgan Blake", role: "director", locked: true },
        { id: "p_a3", name: "Jordan Avery", role: "shareholder", locked: true },
        { id: "p_a4", name: "Priya Desai", role: "key-personnel" },
      ],
      position: { x: 480, y: 40 },
      source: "record",
      locked: true,
    },
    {
      id: holdcoAId,
      type: "private-corporation",
      legalName: "Avery Holdings Ltd.",
      businessNumber: "BC0774521",
      incorporationNumber: "BC0774521",
      jurisdiction: "British Columbia",
      linkedLicenceIds: [],
      people: [
        { id: "p_ha1", name: "Jordan Avery", role: "director", locked: true },
        { id: "p_ha2", name: "Jordan Avery", role: "shareholder", locked: true },
      ],
      position: { x: 140, y: 360 },
      source: "record",
      locked: true,
    },
    {
      id: holdcoBId,
      type: "private-corporation",
      legalName: "Blake Family Investments Inc.",
      businessNumber: "BC0912345",
      incorporationNumber: "BC0912345",
      jurisdiction: "British Columbia",
      linkedLicenceIds: [],
      people: [
        { id: "p_hb1", name: "Morgan Blake", role: "director", locked: true },
        { id: "p_hb2", name: "Casey Blake", role: "shareholder" },
      ],
      position: { x: 820, y: 360 },
      source: "record",
      locked: true,
    },
    {
      id: trustId,
      type: "trust",
      legalName: "Blake Family Trust",
      jurisdiction: "British Columbia",
      linkedLicenceIds: [],
      people: [
        { id: "p_t1", name: "Morgan Blake", role: "trustee" },
        { id: "p_t2", name: "Casey Blake", role: "beneficiary" },
      ],
      position: { x: 820, y: 680 },
      source: "portal",
      locked: false,
    },
    {
      id: dbaId,
      type: "sole-proprietor",
      legalName: "Downtown Bistro (DBA)",
      linkedLicenceIds: [MOCK_LICENCES[1]?.id].filter(Boolean) as string[],
      people: [],
      position: { x: 140, y: 700 },
      source: "portal",
      locked: false,
      note: "Trading name used at the Government St. location.",
    },
    {
      id: familyId,
      type: "individual",
      legalName: "Casey Blake",
      linkedLicenceIds: [],
      people: [],
      position: { x: 480, y: 700 },
      source: "portal",
      locked: false,
      note: "Spouse of Morgan Blake — family connection disclosed.",
    },
  ]

  const edges: LEEdge[] = [
    {
      id: "edge_a_holdcoA",
      source: holdcoAId,
      target: applicantId,
      type: "shareholding",
      sharePercent: 50,
      locked: true,
    },
    {
      id: "edge_a_holdcoB",
      source: holdcoBId,
      target: applicantId,
      type: "shareholding",
      sharePercent: 50,
      locked: true,
    },
    {
      id: "edge_trust_holdcoB",
      source: trustId,
      target: holdcoBId,
      type: "shareholding",
      sharePercent: 100,
      locked: false,
    },
    {
      id: "edge_dba",
      source: applicantId,
      target: dbaId,
      type: "dba",
      locked: false,
    },
    {
      id: "edge_family",
      source: applicantId,
      target: familyId,
      type: "family",
      locked: false,
    },
  ]

  return {
    id: `build_${businessNumber}`,
    businessLegalName,
    businessNumber,
    status: "draft",
    lockedByLCRB: false,
    version: 1,
    nodes,
    edges,
    suggestions: [],
    reviewNotes: [],
    updatedAt: new Date().toISOString(),
  }
}
