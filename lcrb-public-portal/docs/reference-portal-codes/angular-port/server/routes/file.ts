import { get } from '@vercel/blob'

/** GET /api/file streams a private blob uploaded via /api/upload. TODO: any caller who knows the pathname can read it; add an authorization check. */
export async function GET(request: Request) {
  try {
    const pathname = new URL(request.url).searchParams.get('pathname')

    if (!pathname) {
      return Response.json({ error: 'Missing pathname' }, { status: 400 })
    }

    const result = await get(pathname, {
      access: 'private',
      ifNoneMatch: request.headers.get('if-none-match') ?? undefined,
    })

    if (!result) {
      return new Response('Not found', { status: 404 })
    }

    if (result.statusCode === 304) {
      return new Response(null, {
        status: 304,
        headers: {
          ETag: result.blob.etag,
          'Cache-Control': 'private, no-cache',
        },
      })
    }

    return new Response(result.stream, {
      headers: {
        'Content-Type': result.blob.contentType,
        ETag: result.blob.etag,
        'Cache-Control': 'private, no-cache',
      },
    })
  } catch (error) {
    console.error('Error serving file:', error)
    return Response.json({ error: 'Failed to serve file' }, { status: 500 })
  }
}
