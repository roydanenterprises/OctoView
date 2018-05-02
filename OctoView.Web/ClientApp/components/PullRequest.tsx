import * as React from 'react';
import {ICodeReview, CodeReview} from './CodeReview';

export interface IPullRequest {
	name: string;
	number: number;
	status: string;
	url: string;
	assignedTo: string;
	assignee: string;
	url:string;
	reviews: ICodeReview[];
}

export class PullRequest extends
React.Component<IPullRequest, any> {

	render() {
		console.log('rendered pull request.');

		return <div className="ov-c-pull-request">
				   <div className="ov-c-pull-request__name">
						<a href={this.props.url}>
					   		{this.props.name} <span className="ov-c-pull-request__number">{this.props.number}</span>
						</a>
					</div>
			       <div className="ov-c-pull-request__review-container">
				       {this.props.reviews.map(review => <CodeReview {...review}/>)}
			       </div>
		       </div>;
	}
}