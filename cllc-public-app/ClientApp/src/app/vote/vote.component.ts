import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CookieService } from 'ngx-cookie-service';
import { Validators, FormControl, FormGroup, FormArray, FormBuilder } from '@angular/forms';

import { VoteDataService } from "./vote-data.service"

// simple voting feature, based on https://github.com/cjsheets/angular-voting-app

@Component({
    selector: 'app-vote',
    templateUrl: './vote.component.html',
    styleUrls: ['./vote.component.scss']
})
/** vote component*/
export class VoteComponent implements OnInit {
  @Input('slug') slug: string;

  private options;
  private question: string;
  private title: string;
  private id: string;
  private alreadyVoted: boolean = false;
  private showVoteResults: boolean = false;

    /** vote constructor */
  constructor(private cookieService: CookieService, private voteDataService: VoteDataService) {     
  } 

  ngOnInit(): void {    
    
    if (this.slug != null) {
      this.voteDataService.getQuestion(this.slug)
        .then((voteQuestion) => {
          this.question = voteQuestion.question;
          this.options = voteQuestion.options;
          this.title = voteQuestion.title;
          this.id = voteQuestion.id;

          var cookieValue = this.cookieService.get('HasVoted' + this.id);
          if (cookieValue != null && cookieValue == 'Y') {
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
    this.cookieService.set('HasVoted' + this.id, "Y");
      this.alreadyVoted = true;
      // send the vote in.
      this.voteDataService.postVote(this.slug, option).then((voteQuestion) => {
        console.log (voteQuestion.options);
        this.options = voteQuestion.options;
      });
    }
}
