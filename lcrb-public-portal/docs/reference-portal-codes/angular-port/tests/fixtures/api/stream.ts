/** Builds canned AI SDK v6 UI message streams (SSE), as the streaming routes return. */
import type { ApiFixture } from '../../support/api-fixtures';

export const UI_STREAM_HEADERS = {
  'cache-control': 'no-cache',
  connection: 'keep-alive',
  'x-vercel-ai-ui-message-stream': 'v1',
  'x-accel-buffering': 'no',
};

export interface StreamTool {
  toolName: string;
  input: unknown;
  /** Output for server-executed tools (e.g. suggestReplies). Omit for client-side tools (e.g. setField). */
  output?: unknown;
}

let callCounter = 0;

/** A text reply, optionally followed by tool calls. */
export function uiStream(text: string, tools: StreamTool[] = []): ApiFixture {
  const chunks: unknown[] = [
    { type: 'start' },
    { type: 'start-step' },
    { type: 'text-start', id: 't1' },
    { type: 'text-delta', id: 't1', delta: text },
    { type: 'text-end', id: 't1' },
  ];
  for (const tool of tools) {
    const toolCallId = `call_fixture_${++callCounter}`;
    chunks.push({ type: 'tool-input-available', toolCallId, toolName: tool.toolName, input: tool.input });
    if (tool.output !== undefined) chunks.push({ type: 'tool-output-available', toolCallId, output: tool.output });
  }
  chunks.push({ type: 'finish-step' }, { type: 'finish', finishReason: tools.some((t) => t.output === undefined) ? 'tool-calls' : 'stop' });
  const body = chunks.map((c) => `data: ${JSON.stringify(c)}\n\n`).join('') + 'data: [DONE]\n\n';
  return { body, contentType: 'text/event-stream', headers: UI_STREAM_HEADERS };
}

/** A text reply ending with two quick-reply chips (server-executed suggestReplies tool). */
export function replyWithChips(text: string, replies: string[]): ApiFixture {
  return uiStream(text, [{ toolName: 'suggestReplies', input: { replies }, output: { replies } }]);
}
