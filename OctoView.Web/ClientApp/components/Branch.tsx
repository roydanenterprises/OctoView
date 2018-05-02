import * as React from 'react';
import {IPullRequest, PullRequest } from './PullRequest'

export interface IBranch {
	repo: string;
	branchName: string;
	layout: string;
	pulls: IPullRequest[];
}

/*export const Feature: React.SFC<IFeature> = (props) => {
	if (props.layout === "table") {
		return <div>
			       <div>{props.repo}</div>
		       </div>;
	}
	return <div>
		       <div>{props.repo}</div>
	       </div>;
};*/

export class Branch extends
React.Component<IBranch, any> {
	render() {
		if (this.props.layout === 'table') {
			return <div>
				       <div>{this.props.repo}</div>
			       </div>;
		}
		return <div>
			       <div>{this.props.branchName}</div>
			       <div>
				       {this.props.pulls.map(pulls => <PullRequest {...pulls}/>)}
			       </div>;
		       </div>;
	}
}