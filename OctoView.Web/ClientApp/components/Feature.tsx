import * as React from 'react';
import {IPullRequest } from './PullRequest'

export interface IFeature {
	repo: string;
	branchName: string;
	layout : string;
	pulls: IPullRequest[];
}

export const Feature: React.SFC<IFeature> = (props) => {
	if (props.layout === "table") {
		return <div>
			       <div>{props.repo}</div>
		       </div>;
	}
	return <div>
		       <div>{props.repo}</div>
	       </div>;
};

/*export class Feature extends
React.Component<IFeature, any> {
	render() {
		return <div>
			       <div>{this.props.repo}</div>
		       </div>;
	}
}*/