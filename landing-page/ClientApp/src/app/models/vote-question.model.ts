import { VoteOption } from "./vote-option.model";
export class VoteQuestion {
  id: string;
  title: string;
  question: string;
  slug: string;  
  options: VoteOption[];
  constructor() { }
}
