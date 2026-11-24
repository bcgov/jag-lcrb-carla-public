/** Page titles: 15 pages set their own title, the rest inherit the root title (no template), as in the prototype. */
import { inject, Injectable } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { type RouterStateSnapshot, TitleStrategy } from '@angular/router';

export const ROOT_TITLE = 'Liquor and Cannabis Licensing Portal | British Columbia';

@Injectable()
export class PortalTitleStrategy extends TitleStrategy {
  private readonly title = inject(Title);
  override updateTitle(snapshot: RouterStateSnapshot): void {
    this.title.setTitle(this.buildTitle(snapshot) ?? ROOT_TITLE);
  }
}
