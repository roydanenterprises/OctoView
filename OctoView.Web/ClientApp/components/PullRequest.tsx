import * as React from 'react';
import {ICodeReview, CodeReview} from './CodeReview';

export interface IPullRequest {
		name: string;
		number : number;
	status : string;
	reviews: ICodeReview[];
}

export class PullRequest extends
React.Component<IPullRequest, any> {
	render() {
		return <div></div>;
	}
}