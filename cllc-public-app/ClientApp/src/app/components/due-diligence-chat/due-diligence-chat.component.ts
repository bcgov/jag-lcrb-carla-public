import { Component } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';

@Component({
  selector: 'app-due-diligence-chat',
  templateUrl: './due-diligence-chat.component.html',
  styleUrls: ['./due-diligence-chat.component.scss']
})
/**
 * Slide-out panel hosting the LCRB due diligence assistant.
 *
 * The assistant is a separate single-page application, served as static files from this same
 * origin under /chat/. Same-origin matters: the chat calls /api/agent-framework/... which this
 * portal proxies to the agentic platform (AgenticPlatformProxyMiddleware), attaching a token
 * minted from the signed-in session. Served from another origin the browser would not send the
 * session cookie, and every call would come back unauthenticated.
 *
 * Mounted once in the app shell and toggled from the button beside "AI Assistant", so it is
 * reachable from any page rather than being somewhere the user has to navigate to.
 */
export class DueDiligenceChatComponent {
  /** Whether the panel is showing. */
  public isOpen = false;

  /**
   * The frame is created on first open and then kept, only slid out of view when closed.
   *
   * This is deliberate. The assistant stores no conversation server-side — the transcript lives
   * in the frame's own memory — so destroying the frame would silently discard the conversation
   * every time the user closed the panel.
   */
  public hasOpened = false;

  public chatUrl: SafeResourceUrl | null = null;


  constructor(private sanitizer: DomSanitizer) { }

  public open(): void {
    if (!this.hasOpened) {
      // Resolved against the document base href rather than hard-coded, so one build works both
      // where the portal is served under /lcrb/ (dev and test) and at the root (production).
      //
      // index.html is named explicitly rather than relying on a directory index: served locally
      // by ASP.NET static files, which does not serve a default document for a bare directory.
      // nginx matches the /chat/ prefix either way.
      const resolved = new URL('chat/index.html', document.baseURI).toString();

      // Built here and never from user input, so it is safe to trust as a frame source. Angular
      // blocks iframe src bindings without this.
      this.chatUrl = this.sanitizer.bypassSecurityTrustResourceUrl(resolved);
      this.hasOpened = true;
    }

    this.isOpen = true;
  }

  public close(): void {
    this.isOpen = false;
  }

  public toggle(): void {
    if (this.isOpen) {
      this.close();
    } else {
      this.open();
    }
  }
}
