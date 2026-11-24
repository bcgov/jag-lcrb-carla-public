import { put } from '@vercel/blob'

/** POST /api/upload stores a document in private Vercel Blob, readable only via /api/file. TODO: no auth, file-type or size checks, no malware scan. */
export async function POST(request: Request) {
  try {
    const formData = await request.formData()
    const file = formData.get('file') as File

    if (!file) {
      return Response.json({ error: 'No file provided' }, { status: 400 })
    }

    // Upload to private blob storage. addRandomSuffix avoids collisions when
    // two applicants upload files with the same name.
    const subjectId = (formData.get('subjectId') as string) || 'general'
    const blob = await put(`documents/${subjectId}/${file.name}`, file, {
      access: 'private',
      addRandomSuffix: true,
    })

    return Response.json({
      url: blob.url,
      pathname: blob.pathname,
      filename: file.name,
      contentType: file.type,
      size: file.size,
    })
  } catch (error) {
    console.error('Upload error:', error)
    return Response.json({ error: 'Upload failed' }, { status: 500 })
  }
}
