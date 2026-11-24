import { list } from '@vercel/blob'

/** GET /api/files lists uploaded documents as metadata for a debug view. TODO: it returns the whole store to any caller; require auth and scope it. */
export async function GET() {
  try {
    const { blobs } = await list({ prefix: 'documents/' })

    return Response.json({
      files: blobs.map((blob) => ({
        pathname: blob.pathname,
        filename: blob.pathname.split('/').pop() || 'unknown',
        size: blob.size,
        uploadedAt: blob.uploadedAt,
      })),
    })
  } catch (error) {
    console.error('Error listing files:', error)
    return Response.json({ error: 'Failed to list files' }, { status: 500 })
  }
}
