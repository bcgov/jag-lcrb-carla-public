/** Stub for the server route tests: the real `generateText` and `streamText` run with the model swapped for a `MockLanguageModelV3` from `ai/test`. */
import { MockLanguageModelV3, simulateReadableStream } from 'ai/test';

/** The provider's stream part type, taken from the mock (`@ai-sdk/provider` is not a direct dependency). */
type LanguageModelV3StreamPart =
  Awaited<ReturnType<MockLanguageModelV3['doStream']>>['stream'] extends ReadableStream<infer P> ? P : never;

const usage = {
  inputTokens: { total: 10, noCache: 10, cacheRead: undefined, cacheWrite: undefined },
  outputTokens: { total: 20, text: 20, reasoning: undefined },
};

/** The model every stubbed `generateText`/`streamText` call uses; set per test. */
export const aiStub: { model: MockLanguageModelV3 | undefined } = { model: undefined };

/** A model that answers `generateText` with this value as JSON text (structured output). */
export function jsonModel(value: unknown): MockLanguageModelV3 {
  return textModel(JSON.stringify(value));
}

/** A model that answers `generateText` with plain text. */
export function textModel(text: string): MockLanguageModelV3 {
  return new MockLanguageModelV3({
    doGenerate: async () => ({
      content: [{ type: 'text', text }],
      finishReason: { unified: 'stop', raw: 'stop' },
      usage,
      warnings: [],
    }),
  });
}

/** A model whose generate and stream calls both throw this error. */
export function failingModel(error: unknown): MockLanguageModelV3 {
  return new MockLanguageModelV3({
    doGenerate: async () => {
      throw error;
    },
    doStream: async () => {
      throw error;
    },
  });
}

/** One streamed text turn, optionally followed by tool calls. */
export interface StreamTurn {
  text?: string;
  toolCalls?: { name: string; input: unknown }[];
}

/** A model that streams one turn per `streamText` step, in order. */
export function streamModel(...turns: StreamTurn[]): MockLanguageModelV3 {
  let step = 0;
  return new MockLanguageModelV3({
    doStream: async () => {
      const turn = turns[Math.min(step, turns.length - 1)];
      const n = step++;
      const chunks: LanguageModelV3StreamPart[] = [{ type: 'stream-start', warnings: [] }];
      if (turn.text) {
        const id = `text-${n}`;
        chunks.push({ type: 'text-start', id }, { type: 'text-delta', id, delta: turn.text }, { type: 'text-end', id });
      }
      (turn.toolCalls ?? []).forEach((call, i) => {
        chunks.push({
          type: 'tool-call',
          toolCallId: `call-${n}-${i}`,
          toolName: call.name,
          input: JSON.stringify(call.input),
        });
      });
      const toolTurn = (turn.toolCalls ?? []).length > 0;
      chunks.push({
        type: 'finish',
        finishReason: { unified: toolTurn ? 'tool-calls' : 'stop', raw: toolTurn ? 'tool_calls' : 'stop' },
        usage,
      });
      return { stream: simulateReadableStream({ chunks, initialDelayInMs: null, chunkDelayInMs: null }) };
    },
  });
}

type AiModule = typeof import('ai');

/** The `ai` module with `generateText` and `streamText` bound to `aiStub.model`. */
export function mockAiModule(ai: AiModule): AiModule {
  const model = () => {
    if (!aiStub.model) throw new Error('aiStub.model is not set for this test');
    return aiStub.model;
  };
  return {
    ...ai,
    generateText: ((opts: Parameters<AiModule['generateText']>[0]) =>
      ai.generateText({ ...opts, model: model() })) as AiModule['generateText'],
    streamText: ((opts: Parameters<AiModule['streamText']>[0]) =>
      ai.streamText({ ...opts, model: model() })) as AiModule['streamText'],
  };
}
