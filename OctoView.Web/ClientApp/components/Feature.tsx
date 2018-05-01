import * as React from 'react';
import {IPullRequest, PullRequest} from './PullRequest'

export interface IFeature {
	repo: string;
	branchName: string;
	renderType : string;
	pulls: IPullRequest[];
}

export class Feature extends
React.Component<IFeature, any> {
	render() {
		return <div></div>;
	}
}