import * as React from 'react';
import {ICodeReview, CodeReview} from './CodeReview';

export interface IPullRequest {
	name: string;
	number: number;
	status: string;
	url: string;
	assignedTo: string;
	reviews: ICodeReview[];
}

export class PullRequest extends
React.Component<IPullRequest, any> {

	render() {
		console.log('rendered pull request.');

		return <div className="ov-c-pull-request">
				   <div>
						<a href={this.props.url}>
					   		{this.props.name} <span className="ov-c-pull-request__number">{this.props.number}</span>
						</a>
					</div>
			       <div>
				       {this.props.reviews.map(review => <CodeReview {...review}/>)}
			       </div>
		       </div>;
	}
}