import { Component, Input, OnInit } from '@angular/core';
import { VoteDataService } from '../services/vote-data.service';

// simple voting feature, based on https://github.com/cjsheets/angular-voting-app

@Component({
  selector: 'app-vote',
  templateUrl: './vote.component.html',
  styleUrls: ['./vote.component.scss']
})
/** vote component*/
export class VoteComponent implements OnInit {
  @Input() slug: string;
  @Input() vote_value: string;

  public options;
  public question: string;
  public title: string;
  private id: string;
  public alreadyVoted = false;
  public showVoteResults = false;
  localStorage = window.localStorage;

  /** vote constructor */
  constructor(private voteDataService: VoteDataService) {
  }

  ngOnInit(): void {

    if (this.slug != null) {
      this.voteDataService.getQuestion(this.slug)
        .subscribe((voteQuestion) => {
          this.question = voteQuestion.question;
          this.options = voteQuestion.options;
          this.title = voteQuestion.title;
          this.id = voteQuestion.id;

          const cookieValue = this.localStorage.getItem('HasVoted' + this.id);
          if (cookieValue != null && cookieValue === 'Y') {
            this.alreadyVoted = true;
            this.showVoteResults = true;
          }
        });
    }
  }

  setShowVoteResults(value) {
    this.showVoteResults = value;
  }
  sendVote(option) {
    this.localStorage.setItem('HasVoted' + this.id, 'Y');
    this.alreadyVoted = true;
    // send the vote in.
    this.voteDataService.postVote(this.slug, option)
    .subscribe((voteQuestion) => {
      console.log(voteQuestion.options);
      this.options = voteQuestion.options;
    });
  }
}
